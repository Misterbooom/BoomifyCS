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
            LLVMTypeRef[] elementTypes = attributes.Select(i => i.Value.GetBifyType().LLVMType).ToArray();
            named.StructSetBody(elementTypes, false);
            ClassType classType = new ClassType(classNode.NameNode.Token.Value, named);
            ClassMethodManager methodManager = new(classNode, classType);
            classType.ClassAttributes = attributes;
            ClassMethod[] methods = methodManager.GetMethods();
            classType.ClassMethods = methods;
            classType.Constructor = methods.Where(i => i.Name == "constructor").ToArray();
            compiler.VariableManager.RegisterGlobalVariable(classNode.NameNode.Token.Value, classType);
        }

    }
    class ClassInitFunction : BifyFunction
    {
        public ClassInitFunction(ClassType classType) : base(null, null, classType, null)
        {
            Init();
        }
        public void Init()
        {
            var compiler = AssemblyCompiler.Instance;
            TypeRef = LLVMTypeRef.CreateFunction(ReturnType.LLVMType, []);
            llvmValue = compiler.Module.AddFunction($"{ReturnType.Name}.Init", TypeRef);
            var entry = llvmValue.AppendBasicBlock("entry");
            compiler.Builder.PositionAtEnd(entry);

            compiler.Builder.BuildRetVoid();


        }

    }
}
