using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomifyCS.Assembly.BifyObject;
using LLVMSharp.Interop;

namespace BoomifyCS.Assembly.NodeHandlers.ClassHandler
{
    class ClassInitializer
    {
        private ClassType classType;
        public ClassInitializer(ClassType classType)
        {
            this.classType = classType;
        }
        public (LLVMValueRef,LLVMTypeRef) InitClass()
        {
            LLVMTypeRef initFunctionType = LLVMTypeRef.CreateFunction(LLVMTypeRef.CreatePointer(classType.LLVMType, 0), []);

            LLVMValueRef initFunction = AssemblyCompiler.Instance.Module.AddFunction($"{classType.Name}_init", initFunctionType);
            var entry = initFunction.AppendBasicBlock("entry");
            AssemblyCompiler.Instance.Builder.PositionAtEnd(entry);

            ClassValue classValue = (ClassValue)classType.CreateValueRef(CreateStruct());
            SetAttributesValue(classValue);

            AssemblyCompiler.Instance.Builder.BuildRet(classValue.GetLLVMValue());
            return (initFunction,initFunctionType);
        }
        private void SetAttributesValue(ClassValue classValue)
        {
            foreach (var classAttribute in classType.ClassAttributes)
            {
                BifyValue attributePointer = classValue.GetAttribute(classAttribute.Name, AssemblyCompiler.Instance.Builder);
                AssemblyCompiler.Instance.Builder.BuildStore(classAttribute.Value.GetLLVMValue(), attributePointer.GetLLVMValue());
            }
        }
        private LLVMValueRef CreateStruct()
        {
            BifyFunction mallocFunction = AssemblyCompiler.Instance.VariableManager.GetBifyValue("malloc") as BifyFunction;
            BifyValue raw = mallocFunction.Call([new IntegerType().Create(classType.Size())]);
            LLVMValueRef rawValueRef = raw.GetLLVMValue();
            rawValueRef.Name = $"{classType.Name}_raw";


            return rawValueRef;
        }

    }
}
