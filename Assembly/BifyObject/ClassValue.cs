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
        public ClassMethod(string name, BifyValue value)
            : base(name, value) { }
    }

    class ClassValue : BifyValue
    {
        public ClassValue(LLVMValueRef value, BifyType type) : base(value, type) { }

        public override BifyValue GetAttribute(string name, LLVMBuilderRef builder)
        {
            ClassType classType = (ClassType)GetBifyType();
            foreach (var attribute in classType.ClassAttributes)
            {
                if (attribute.Name == name)
                {
                    LLVMValueRef gepResult = builder.BuildGEP2(classType.StructType, GetLLVMValue(),
                        [
                                   new IntegerType().Create(0).GetLLVMValue(),
                                   new IntegerType().Create(Array.IndexOf(classType.ClassAttributes, attribute)).GetLLVMValue()
                       ]);
                    return new BifyPointerType(attribute.Value.GetBifyType()).CreateValueRef(gepResult);
                }
            }
            foreach (var method in classType.ClassMethods)
            {
                if (method.Name == name)
                {
                    return method.Value;
                }
            }

            Traceback.Instance.ThrowException(new BifyAttributeError($"{GetTypeName()} doesn't have attribute '{name}'"));
            return null;
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

        public ClassType(string name, LLVMTypeRef structType)
            : base(name, LLVMTypeRef.CreatePointer(structType, 0))
        {
            StructType = structType;
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

        public ClassValue InitClass()
        {
            var entryBlock = AssemblyCompiler.Instance.Builder.InsertBlock;
            if (classInitFunction == null)
            {
                var (initFunction, initFunctionType) = new ClassInitializer(this).InitClass();
                classInitFunction = initFunction;
                classInitFunctionType = initFunctionType;
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
