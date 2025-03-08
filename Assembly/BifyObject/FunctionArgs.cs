using System;
using System.Collections.Generic;
using System.Linq;
using BoomifyCS.Ast;
using BoomifyCS.Lexer;
using LLVMSharp.Interop;

namespace BoomifyCS.Assembly.BifyObject
{
    class FunctionArgs
    {
        public LLVMTypeRef[] LLVMTypes
        {
            get
            {
                return arguments
                    .Select(item => AssemblyCompiler.Instance.variableManager.GetLLVMType(item.Value))
                    .ToArray();
            }
            private set
            {
            }
        }
        public BifyType[] BifyTypes
        {
            get
            {
                return arguments
                    .Select(item => AssemblyCompiler.Instance.variableManager.GetBifyType(item.Value))
                    .ToArray();
            }
        }
        public string[] ArgsNames
        {
            get
            {
                return arguments
                    .Select(item => item.Key)
                    .ToArray();
            }
        }

        private Dictionary<string, string> arguments = new Dictionary<string, string>();

        public FunctionArgs(AstNode argNode)
        {
            ExtractArgs(argNode);
        }
        public void SetArguments(Dictionary<string, string> arguments)
        {
            this.arguments = arguments;
        }
        private void ExtractArgs(AstNode node)
        {
            if (node is AstBinaryOp astBinaryOp)
            {
                if (astBinaryOp.Token.Type == TokenType.COMMA)
                {
                    ExtractArgs(astBinaryOp.Left);
                    ExtractArgs(astBinaryOp.Right);
                }
                else
                {
                    string name = astBinaryOp.Right.Token.Value;
                    string type = astBinaryOp.Left.Token.Value;
                    arguments[name] = type;
                }
            }
        }
    }
}
