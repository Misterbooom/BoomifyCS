using BoomifyCS.Assembly.NodeHandlers.ClassHandler;
using BoomifyCS.Exceptions;
using LLVMSharp;
using LLVMSharp.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace BoomifyCS.Assembly.BifyObject
{
#nullable enable

    class ClassMember
    {
        public string Name;
        public BifyValue Value;

        public ClassMember(string name, BifyValue value)
        {
            Name = name;
            Value = value;
        }

        public override string ToString()
        {
            return $"{Name}(value:{Value})";
        }
    }

    class ClassAttribute : ClassMember
    {
        public ClassAttribute(string name, BifyValue value)
            : base(name, value) { }
    }

    class BifyMethodRef : BifyValue
    {
        public string MethodName { get; }
        public ClassValue ClassValue { get; }
        private List<BifyFunction> overloads = new();

        public BifyMethodRef(string methodName, ClassValue classType, List<BifyFunction> overloads)
            : base(null, overloads.First().GetBifyType())
        {
            MethodName = methodName;
            ClassValue = classType;
            this.overloads = overloads;
        }
        public BifyFunction Resolve(BifyType[] types)
        {
            foreach (var overload in overloads)
            {
                BifyDebug.Log($"Overload: {overload}");

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
    class ClassMethod : ClassMember
    {
        private List<BifyFunction> overloads = new();
        private ValueFlag methodAccessFlag;
        public ClassMethod(string name, BifyFunction firstOverload)
            : base(name, firstOverload)
        {
            overloads.Add(firstOverload);
            methodAccessFlag = firstOverload.ValueFlag & ValueFlag.AccessMask;
        }
        public void AddOverload(BifyFunction overload)
        {
            if (overload.ValueFlag != methodAccessFlag)
            {
                Traceback.Instance.ThrowException(new BifyAttributeError(
                    $"Overload for method '{Name}' has different access level than the first overload."
                ));
            }

            if (overloads.Any(x => x.FunctionArgs.HasSameTypes(overload.FunctionArgs)))
            {
                Traceback.Instance.ThrowException(new BifyAttributeError(
                    $"Overload for method with same parameters '{Name}' already exists."
                ));
            }
            overloads.Add(overload);
        }
        public BifyMethodRef GetMethodRef(ClassValue classValue)
        {
            return new BifyMethodRef(Name, classValue, overloads);
        }
        public IEnumerable<BifyFunction> GetOverloads() => overloads;

        public override string ToString()
        {
            return $"{Name}(overloads: {overloads.Count})";
        }
    }


    class ClassValue : BifyValue
    {
        public ClassValue(LLVMValueRef value, BifyType type) : base(value, type) { }

        public override BifyValue GetAttribute(string name, BifyType other, LLVMBuilderRef builder)
        {
            ClassType classType = (ClassType)GetBifyType();

            for (int i = 0; i < classType.ClassAttributes.Length; i++)
            {
                var attribute = classType.ClassAttributes[i];
                if (attribute.Name == name)
                {
                    if (!classType.HasAccess(attribute.Value.ValueFlag, other))
                    {
                        Traceback.Instance.ThrowException(new BifyAttributeError(
                            $"Access to attribute '{attribute.Name}' is denied because it is {ClassType.GetAccessLevel(attribute.Value.ValueFlag)}"
                        ));
                    }

                    var gepResult = builder.BuildGEP2(classType.StructType, GetLLVMValue(), new LLVMValueRef[]
                    {
                        new IntegerType().Create(0).GetLLVMValue(),
                        new IntegerType().Create(i).GetLLVMValue()
                    });

                    return new AllocaType(attribute.Value.GetBifyType()).CreateValueRef(gepResult);
                }
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

    class ClassType : BifyType
    {
        public ClassAttribute[] ClassAttributes = Array.Empty<ClassAttribute>();
        public Dictionary<string, ClassMethod> ClassMethods = new();
        public Dictionary<string, List<ClassMethod>> Constructor = new();
        public LLVMTypeRef StructType;
        private LLVMValueRef classInitFunction;
        private LLVMTypeRef classInitFunctionType;
        public string ClassName { get; private set; }

        public ClassType(string name, LLVMTypeRef structType)
            : base(name, LLVMTypeRef.CreatePointer(structType, 0))
        {
            StructType = structType;
            ClassName = name;
        }

        public void AddMethod(ClassMethod method)
        {
            if (!ClassMethods.TryGetValue(method.Name, out var classMethod))
            {
                classMethod = new ClassMethod(method.Name, method.Value as BifyFunction ?? throw new InvalidOperationException("Method value is not bifyFunction."));
                ClassMethods[method.Name] = classMethod;
            }
            else
            {
                classMethod.AddOverload(method.Value as BifyFunction ?? throw new InvalidOperationException("Method value is not bifyFunction."));
            }
        }

        public override uint Size()
        {
            uint res = 0;
            foreach (var attribute in ClassAttributes)
            {
                res += attribute.Value.GetBifyType().Size();
            }
            return res;
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
            if (classInitFunction == null)
            {
                AssemblyCompiler.Instance.CurrentClass = this;
                var (initFunction, initFunctionType) = new ClassInitializer(this).InitClass();
                classInitFunction = initFunction;
                classInitFunctionType = initFunctionType;
                AssemblyCompiler.Instance.CurrentClass = null;
            }
            AssemblyCompiler.Instance.Builder.PositionAtEnd(entryBlock);
            var classValue = (ClassValue)CreateValueRef(
                AssemblyCompiler.Instance.Builder.BuildCall2(classInitFunctionType, classInitFunction, Array.Empty<LLVMValueRef>(), "classInit")
            );
            return classValue;
        }

        protected override BifyValue CreateByValueRef(LLVMValueRef value)
        {
            return new ClassValue(value, this);
        }
        public ClassMethod? GetMethod(string name, BifyType other)
        {
            if (ClassMethods.TryGetValue(name, out var method))
            {
                if (!HasAccess(method.Value.ValueFlag, other))
                {
                    Traceback.Instance.ThrowException(new BifyAttributeError(
                        $"Access to method '{method.Name}' is denied because it is {ClassType.GetAccessLevel(method.Value.ValueFlag)}"
                    ));
                }

                return method;
            }

            return null;
        }
        public bool HasAccess(ValueFlag flags, BifyType accessorType)
        {
            var access = flags & ValueFlag.AccessMask;

            return access switch
            {
                ValueFlag.Private => accessorType != null && accessorType.CompareType(this),
                ValueFlag.Public => true,
                ValueFlag.Protected => accessorType != null && accessorType.CompareType(this),
                _ => false
            };
        }

        public static string GetAccessLevel(ValueFlag flags)
        {
            var access = flags & ValueFlag.AccessMask;
            return access switch
            {
                ValueFlag.Private => "private",
                ValueFlag.Public => "public",
                ValueFlag.Protected => "protected",
                _ => "unknown"
            };
        }
    }
}
