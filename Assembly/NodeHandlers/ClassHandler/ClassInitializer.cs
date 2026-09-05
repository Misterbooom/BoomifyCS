using BoomifyCS.Assembly.BifyObject;
using LLVMSharp.Interop;

namespace BoomifyCS.Assembly.NodeHandlers.ClassHandler
{
    class ClassInitializer(ClassType classType)
    {
        public (LLVMValueRef, LLVMTypeRef) InitClass()
        {
            LLVMTypeRef initFunctionType = LLVMTypeRef.CreateFunction(LLVMTypeRef.CreatePointer(classType.LlvmType, 0), []);

            LLVMValueRef initFunction = AssemblyCompiler.Instance.Module.AddFunction($"{classType.Name}_init", initFunctionType);
            var entry = initFunction.AppendBasicBlock("entry");
            AssemblyCompiler.Instance.Builder.PositionAtEnd(entry);

            ClassValue classValue = (ClassValue)classType.CreateValueRef(CreateStruct());
            SetAttributesValue(classValue);

            AssemblyCompiler.Instance.Builder.BuildRet(classValue.GetLlvmValue());
            return (initFunction, initFunctionType);
        }
        private void SetAttributesValue(ClassValue classValue)
        {
            foreach (var classAttribute in classType.ClassAttributes)
            {
                BifyValue attributePointer = classValue.GetAttribute(classAttribute.Name, AssemblyCompiler.Instance.CurrentClass, AssemblyCompiler.Instance.Builder);
                var variableHandler = new VariableDeclarationNodeHandler(AssemblyCompiler.Instance);
                var value = variableHandler.GetVariableValue(classType.GetAttributeValueNode(classAttribute),
                    classAttribute.Name, classAttribute.Type);
                
                AssemblyCompiler.Instance.Builder.BuildStore(value.GetLlvmValue(), attributePointer.GetLlvmValue());
            }
        }
        private LLVMValueRef CreateStruct()
        {
            BifyFunction mallocFunction = AssemblyCompiler.Instance.VariableManager.GetBifyValue("malloc") as BifyFunction;
            BifyValue raw = mallocFunction.Call([new IntegerType().Create(classType.Size())]);
            LLVMValueRef rawValueRef = raw.GetLlvmValue();
            rawValueRef.Name = $"{classType.Name}_raw";


            return rawValueRef;
        }

    }
}
