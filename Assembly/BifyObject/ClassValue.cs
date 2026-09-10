using BoomifyCS.Assembly.NodeHandlers.ClassHandler;
using BoomifyCS.Exceptions;
using LLVMSharp.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using BoomifyCS.Ast;

namespace BoomifyCS.Assembly.BifyObject
{
#nullable enable

    internal class ClassMember
    {
        public readonly string Name;
        public readonly BifyValue? Value;
        public readonly BifyType Type;

        protected ClassMember(string name, BifyValue? value, BifyType type)
        {
            Name = name;
            Value = value;
            Type = type;

        }

        public override string ToString()
        {
            return $"{Name}(value:{Value})";
        }
    }

    internal class ClassAttribute(string name, BifyType type) : ClassMember(name, null, type);

    internal class BifyMethodRef(string methodName, ClassValue classType, List<BifyFunction> overloads)
        : BifyValue(null, overloads.First().GetBifyType())
    {
        private string MethodName { get; } = methodName;
        private ClassValue ClassValue { get; } = classType;

        public BifyFunction Resolve(BifyType[] types)
        {
            foreach (var overload in overloads)
            {

                if (overload.FunctionArgs.HasSameTypes(types, 1))
                {
                    overload.IsMethod = true;
                    overload.ParentClass = ClassValue;

                    return overload;
                }
            }
            if (types.Length < 1 || types[0] is ClassType)
            {
                throw new ArgumentException("First type can't be a ClassType here");
            }
            return new BifyTypeError(
                $"No matching overload for method '{MethodName}' with types: {string.Join(", ", types[0..].Select(t => t.Name))}."
            ).Throw<BifyFunction>();
        }

    }

    internal class ClassMethod : ClassMember
    {
        private readonly List<BifyFunction> _overloads = new();
        private readonly AccessLevel _methodAccessFlag;
        public ClassMethod(string name, BifyFunction firstOverload)
            : base(name, firstOverload, firstOverload.GetBifyType())
        {
            _overloads.Add(firstOverload);
            _methodAccessFlag = firstOverload.AccessLevel;
        }
        public void AddOverload(BifyFunction overload)
        {
            if (overload.AccessLevel != _methodAccessFlag)
            {
                Traceback.Instance.ThrowException(new BifyAttributeError(
                    $"Overload for method '{Name}' has different access level than the first overload."
                ));
            }

            if (_overloads.Any(x => x.FunctionArgs.HasSameTypes(overload.FunctionArgs)))
            {
                Traceback.Instance.ThrowException(new BifyAttributeError(
                    $"Overload for method with same parameters '{Name}' already exists."
                ));
            }
            _overloads.Add(overload);
        }
        public BifyMethodRef GetMethodRef(ClassValue classValue)
        {
            return new BifyMethodRef(Name, classValue, _overloads);
        }
        public IEnumerable<BifyFunction> GetOverloads() => _overloads;

        public override string ToString()
        {
            return $"{Name}(overloads: {_overloads.Count})";
        }
    }


    internal class ClassValue(LLVMValueRef value, BifyType type) : BifyValue(value, type)
    {
        public override BifyValue GetAttribute(string name, BifyType other, LLVMBuilderRef builder)
        {
            ClassType classType = (ClassType)GetBifyType();

            for (int i = 0; i < classType.ClassAttributes.Length; i++)
            {
                var attribute = classType.ClassAttributes[i];
                BifyDebug.Log(attribute.ToString());
                if (attribute.Name != name) continue;
                if (!classType.HasAccess(attribute.Type.AccessLevel, other))
                {
                    Traceback.Instance.ThrowException(new BifyAttributeError(
                        $"Access to attribute '{attribute.Name}' is denied because it is {ClassType.GetAccessLevel(attribute.Type.AccessLevel)}"
                    ));
                }

                var gepResult = builder.BuildGEP2(classType.StructType, GetLlvmValue(), [
                    new IntegerType().Create(0).GetLlvmValue(),
                    new IntegerType().Create(i).GetLlvmValue()
                ]);

                return new AllocaType(attribute.Type).CreateValueRef(gepResult);
            }

            var method = classType.GetMethod(name, other);
            if (method != null)
            {
                return method.GetMethodRef(this);
            }

            return new BifyAttributeError(
                $"{GetTypeName()} doesn't have attribute or method '{name}'").Throw<BifyValue>();
        }



    }

    internal class ClassType(string name, LLVMTypeRef structType) : BifyType(name, LLVMTypeRef.CreatePointer(structType, 0))
    {
        private Dictionary<string, ClassMethod> _classMethods = new();
        public Dictionary<string, List<ClassMethod>> Constructor = new();
        public LLVMTypeRef StructType = structType;
        public ClassAttribute[] ClassAttributes => [.. _classAttributes.Keys];
        private LLVMValueRef _classInitFunction;
        private LLVMTypeRef _classInitFunctionType;
        private Dictionary<ClassAttribute, AstNode> _classAttributes { get; set; } = [];
        private string ClassName { get; set; } = name;

        public void AddMethod(ClassMethod method)
        {
            if (!_classMethods.TryGetValue(method.Name, out var classMethod))
            {
                classMethod = new ClassMethod(method.Name, method.Value as BifyFunction ?? throw new InvalidOperationException("Method value is not bifyFunction."));
                _classMethods[method.Name] = classMethod;
            }
            else
            {
                classMethod.AddOverload(method.Value as BifyFunction ?? throw new InvalidOperationException("Method value is not bifyFunction."));
            }
        }

        public AstVarDecl? GetAttributeValueNode(ClassAttribute attribute)
        {
            return _classAttributes
                .FirstOrDefault(i => i.Key.Name == attribute.Name)
                .Value as AstVarDecl;
        }

        public void SetClassAttributes(Dictionary<ClassAttribute, AstNode> att)
        {
            _classAttributes = att;
        }
        public override uint Size()
        {
            return ClassAttributes.Aggregate<ClassAttribute, uint>(0, (current, attribute) => current + attribute.Type.Size());
        }

        public override BifyValue Create(object value)
        {
            throw new NotImplementedException();
        }

        public override bool CompareType(BifyType other)
        {
            return other is ClassType classType && classType.ClassName == this.ClassName;
        }

        public ClassValue InitClass()
        {
            var entryBlock = AssemblyCompiler.Instance.Builder.InsertBlock;
            if (_classInitFunction == null)
            {
                AssemblyCompiler.Instance.CurrentClass = this;
                var (initFunction, initFunctionType) = new ClassInitializer(this).InitClass();
                _classInitFunction = initFunction;
                _classInitFunctionType = initFunctionType;
                AssemblyCompiler.Instance.CurrentClass = null;
            }
            AssemblyCompiler.Instance.Builder.PositionAtEnd(entryBlock);
            var classValue = (ClassValue)CreateValueRef(
                AssemblyCompiler.Instance.Builder.BuildCall2(_classInitFunctionType, _classInitFunction, Array.Empty<LLVMValueRef>(), "classInit")
            );
            return classValue;
        }

        protected override BifyValue CreateByValueRef(LLVMValueRef value)
        {
            return new ClassValue(value, this);
        }
        public ClassMethod? GetMethod(string name, BifyType other)
        {
            if (_classMethods.TryGetValue(name, out var method))
            {
                if (!HasAccess(method.Type.AccessLevel, other))
                {
                    Traceback.Instance.ThrowException(new BifyAttributeError(
                        $"Access to method '{method.Name}' is denied because it is {ClassType.GetAccessLevel(method.Type.AccessLevel)}"
                    ));
                }

                return method;
            }

            return null;
        }
        public bool HasAccess(AccessLevel level, BifyType? accessorType)
        {
            return level switch
            {
                AccessLevel.PRIVATE => accessorType != null && accessorType.CompareType(this),
                AccessLevel.PUBLIC => true,
                AccessLevel.PROTECTED => accessorType != null && accessorType.CompareType(this),
                _ => false
            };
        }

        public static string GetAccessLevel(AccessLevel accessLevel)
        {
            return accessLevel switch
            {
                AccessLevel.PRIVATE => "private",
                AccessLevel.PUBLIC => "public",
                AccessLevel.PROTECTED => "protected",
                _ => "unknown"
            };
        }
    }
}
