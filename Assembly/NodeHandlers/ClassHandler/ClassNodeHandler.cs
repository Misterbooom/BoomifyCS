using System.Linq;
using BoomifyCS.Assembly.BifyObject;
using BoomifyCS.Ast;
using LLVMSharp.Interop;

namespace BoomifyCS.Assembly.NodeHandlers.ClassHandler
{
    internal class ClassNodeHandler(AssemblyCompiler compiler) : NodeHandler(compiler)
    {
        public override void HandleNode(AstNode node)
        {
            AstClass classNode = (AstClass)node;

            ClassAttributeManager attributeManager = new(classNode);
            var attributes = attributeManager.GetAttributes();
            LLVMTypeRef named = Compiler.Context.Handle.CreateNamedStruct(classNode.NameNode.Token.Value);
            LLVMTypeRef[] elementTypes = [.. attributes.Select(i => i.Key.Type.LlvmType)];
            named.StructSetBody(elementTypes, false);
            ClassType classType = new(classNode.NameNode.Token.Value, named);
            Compiler.CurrentClass = classType;
            ClassMethodManager methodManager = new(classNode, classType);
            classType.SetClassAttributes(attributes);
            methodManager.AddMethodsToClass(ref classType);
           
            Compiler.VariableManager.RegisterGlobalVariable(classNode.NameNode.Token.Value, classType);
            Compiler.CurrentClass = null;
        }

    }
    
}
