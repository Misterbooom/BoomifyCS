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
            BifyValue variable = compiler.variableManager.GetBifyValue(node.Token.Value);
            var value = compiler.builder.BuildLoad2(variable.GetBifyType().LLVMType,variable.GetLLVMValue());
            compiler.StackPush(variable.GetBifyType().CreateByValueRef(value));
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
