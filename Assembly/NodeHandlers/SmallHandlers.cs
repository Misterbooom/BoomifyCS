using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            string identifier = node.Token.Value;
            compiler.variableManager.IsExists(identifier);
            var variable = compiler.variableManager.GetVariable(identifier);
            if (variable.BifyObject == null)
            {
                throw new NullReferenceException($"{identifier} has null bifyValue.");
            }
            compiler.stack.Push(variable.ToBifyValue());
            BifyDebug.Log("Identifier: " + node.LlvmValue.ToString());

        }
    }
    class ConstantNodeHandler : NodeHandler
    {
        public ConstantNodeHandler(AssemblyCompiler compiler) : base(compiler) { }

        public override void HandleNode(AstNode node)
        {
            if (node is AstConstant astConstant)
            {
                BifyValue bifyValue = new BifyValue(astConstant.BifyValue,astConstant.BifyValue.ToLLVM());
                compiler.stack.Push(bifyValue);
            }

        }
    }
}
