using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomifyCS.Ast;
using BoomifyCS.Exceptions;

namespace BoomifyCS.Assembly.NodeHandlers
{
    class FunctionDeclarationNodeHandler : NodeHandler
    {
        public FunctionDeclarationNodeHandler(AssemblyCompiler compiler) : base(compiler) { }
        public override void HandleNode(AstNode node)
        {
            AstFunctionDecl functionDecl = (AstFunctionDecl)node;
            string functionName = functionDecl.functionNameNode.Token.Value;
            AddInitialFunctionCode(functionName);
            compiler.Visit(functionDecl.blockNode);
            compiler.variableManager.ClearLocals();
            if (!compiler.ReturnStatement)
            {
                Traceback.Instance.line = node.LineNumber;
                Traceback.Instance.ThrowException(new BifyParsingError("Not all function path return value", "", functionName));
            }
        }
        private void AddInitialFunctionCode(string functionName)
        {
            compiler.assemblerCode.AddInstruction($"{functionName}:",0);
            compiler.assemblerCode.AddInstruction("push rbp");
            compiler.assemblerCode.AddInstruction("mov rbp, rsp");
            compiler.assemblerCode.AddInstruction("sub rsp, 40");
        }
    }
}