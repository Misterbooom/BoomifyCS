using System;
using System.Collections.Generic;
using BoomifyCS.Assembly.NodeHandlers.ClassHandler;
using BoomifyCS.Exceptions;
using LLVMSharp;
using LLVMSharp.Interop;

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

    class ClassMethod : ClassMember
    {
        public ClassMethod(string name, BifyFunction value)
            : base(name, value) { }
        public BifyFunction GetBifyFunction()
        {
            if (Value is not BifyFunction function)
            {
                throw new InvalidOperationException(
                    $"Expected BifyFunction, but got {Value.GetType().Name} for method '{Name}'"
                );
            }
            return function;
        }
        public bool HasSameParameters(ClassMethod other)
        {
            int paramCount1 = this.GetBifyFunction().FunctionArgs.ArgsNames.Length;
            int paramCount2 = other.GetBifyFunction().FunctionArgs.ArgsNames.Length;
            if (paramCount1 != paramCount2)
                return false;
           
            return true;
        }
    }

    class ClassValue : BifyValue
    {
        public ClassValue(LLVMValueRef value, BifyType type) : base(value, type) { }

        public override BifyValue GetAttribute(string name, BifyType other, LLVMBuilderRef builder)
        {
            var classType = (ClassType)GetBifyType();

            for (int i = 0; i < classType.ClassAttributes.Length; i++)
            {
                var attribute = classType.ClassAttributes[i];
                if (attribute.Name == name)
                {
                    if (!HasAccess(attribute.Value.ValueFlag, other))
                    {
                        Traceback.Instance.ThrowException(new BifyAttributeError(
                            $"Access to attribute '{attribute.Name}' is denied because it is not accessible."
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

            foreach (var method in classType.ClassMethods)
            {
                if (method.Name == name)
                {
                    if (!HasAccess(method.Value.ValueFlag, other))
                    {
                        Traceback.Instance.ThrowException(new BifyAttributeError(
                            $"Access to method '{method.Name}' is denied because it is not accessible."
                        ));
                    }

                    ((BifyFunction)method.Value).ParentClass = this;
                    return method.Value;
                }
            }

            Traceback.Instance.ThrowException(new BifyAttributeError(
                $"{GetTypeName()} doesn't have attribute or method '{name}'"
            ));
            return null;
        }
        private bool HasAccess(ValueFlag flags, BifyType accessorType)
        {
            var access = flags & ValueFlag.AccessMask;

            return access switch
            {
                ValueFlag.Private => accessorType == null ?  false: accessorType.CompareType(GetBifyType()),
                ValueFlag.Public => true,
                ValueFlag.Protected => accessorType == null ? false : accessorType.CompareType(GetBifyType()),
                _ => false
            };
        }


        public BifyValue GetMethod(string name)
        {
            ClassType classType = (ClassType)GetBifyType();
            foreach (var method in classType.ClassMethods)
            {
                if (method.Name == name)
                {
                    return method.Value;
                }
            }

            Traceback.Instance.ThrowException(new BifyAttributeError($"{GetTypeName()} doesn't have method {name}"));
            return null;
        }
    }

    class ClassType : BifyType
    {
        public ClassAttribute[] ClassAttributes = Array.Empty<ClassAttribute>();
        public ClassMethod[] ClassMethods = Array.Empty<ClassMethod>();
        public ClassMethod[] Constructor = Array.Empty<ClassMethod>();
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
            if (other is not ClassType classType)
            {
                return false;
            }
            return classType.ClassName == this.ClassName;
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
                AssemblyCompiler.Instance.Builder.BuildCall2(classInitFunctionType, classInitFunction, new LLVMValueRef[] { }, "classInit")

                );
            return classValue;
        }

        protected override BifyValue CreateByValueRef(LLVMValueRef value)
        {
            return new ClassValue(value, this);
        }
    }
}
