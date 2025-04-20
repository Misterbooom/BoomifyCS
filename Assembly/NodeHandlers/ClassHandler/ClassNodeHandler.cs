using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomifyCS.Assembly.BifyObject;
using BoomifyCS.Ast;
using LLVMSharp.Interop;

namespace BoomifyCS.Assembly.NodeHandlers.ClassHandler
{
    class ClassNodeHandler : NodeHandler
    {
        public ClassNodeHandler(AssemblyCompiler compiler) : base(compiler)
        {

        }
        public override void HandleNode(AstNode node)
        {
            AstClass classNode = (AstClass)node;

            ClassAttributeManager attributeManager = new(classNode);
            ClassAttribute[] attributes = attributeManager.GetAttributes();
            LLVMTypeRef named = compiler.Context.Handle.CreateNamedStruct(classNode.NameNode.Token.Value);
            LLVMTypeRef[] elementTypes = attributes.Select(i => i.Type.LLVMType).ToArray();
            named.StructSetBody(elementTypes, false);
            ClassType classValue = new ClassType(classNode.NameNode.Token.Value, LLVMTypeRef.CreatePointer(named,0));
            ClassMethodManager methodManager = new(classNode, classValue);
            foreach (ClassAttribute attribute in attributes)
            {
                Console.WriteLine("Attribute: " + attribute.ToString());
            }
            ClassMethod[] methods = methodManager.GetMethods();

        }
    }
}
