using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomifyCS.Assembly.BifyObject;
using BoomifyCS.Ast;
using BoomifyCS.Exceptions;
using LLVMSharp.Interop;

namespace BoomifyCS.Assembly.NodeHandlers
{
    class ModuleHandler(AssemblyCompiler compiler) : NodeHandler(compiler)
    {
        public override void HandleNode(AstNode node)
        {
            AstModule moduleNode = (AstModule)node;
            foreach (AstNode child in moduleNode.ChildNodes)
            {
                compiler.Visit(child);
            }
        }
    }
    class BlockNodeHandler(AssemblyCompiler compiler) : NodeHandler(compiler)
    {
        public override void HandleNode(AstNode node)
        {
            AstBlock blockNode = (AstBlock)node;
            foreach (AstNode child in blockNode.ChildNodes)
            {
                compiler.Visit(child);
            }
        }
    }
    class IdentifierNodeHandler(AssemblyCompiler compiler) : NodeHandler(compiler)
    {
        public override void HandleNode(AstNode node)
        {
            IValue variable = compiler.variableManager.GetVariable(node.Token.Value);
            if (variable is BifyType bifyType)
            {
                compiler.StackPush(bifyType);
                return;
            }
            BifyValue bifyValue = (BifyValue)variable;
            var loadedValue = compiler.builder.BuildLoad2(bifyValue.GetBifyType().LLVMType, bifyValue.GetLLVMValue());
            compiler.StackPush(bifyValue.GetBifyType().CreateByValueRef(loadedValue));
        }
    }
    class ConstantNodeHandler : NodeHandler
    {
        public ConstantNodeHandler(AssemblyCompiler compiler) : base(compiler) { }

        public override void HandleNode(AstNode node)
        {
            unsafe
            {
                if (node is AstNumber astNumber)
                {


                    compiler.StackPush(new IntegerType().Create(astNumber.Value));

                }
                else if (node is AstFloat astFloat)
                {
                    compiler.StackPush(new FloatType().Create(astFloat.Value));
                }
                else if (node is AstString astString)
                {
                    compiler.StackPush(new ConstStringType().Create((string)astString.Value));
                }
            }
        }
    }
}
