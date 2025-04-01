using System;
using System.Collections.Generic;
using System.Linq;
using BoomifyCS.Ast;
using BoomifyCS.Exceptions;
using BoomifyCS.Lexer;
using LLVMSharp;
using LLVMSharp.Interop;

namespace BoomifyCS.Assembly.BifyObject
{
    class FunctionArgs
    {
        private Dictionary<string, BifyType> arguments = new Dictionary<string, BifyType>();

        public LLVMTypeRef[] LLVMTypes => arguments.Values
            .Select(type => type.LLVMType)
            .ToArray();

        public BifyType[] BifyTypes => arguments.Values.ToArray();

        public string[] ArgsNames => arguments.Keys.ToArray();

        public FunctionArgs(AstNode argNode)
        {
            ExtractArgs(argNode);
        }

        public void SetArguments(Dictionary<string, BifyType> newArguments)
        {
            if (newArguments == null)
                throw new ArgumentNullException(nameof(newArguments), "Arguments cannot be null.");

            arguments = new Dictionary<string, BifyType>(newArguments);
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
                    AssemblyCompiler.Instance.Visit(astBinaryOp.Left);
                    IValue poppedValue = AssemblyCompiler.Instance.StackIValuePop();
                    if (poppedValue is not BifyType)
                    {
                        Traceback.Instance.ThrowException(new BifyTypeError($"{((BifyValue)poppedValue).GetTypeName()} cannot be used as type."));

                    }
                    BifyDebug.Log($"Argument name: {astBinaryOp.Right.Token.Value}");
                    arguments[astBinaryOp.Right.Token.Value] = (BifyType)poppedValue;
                }
            }
        }
        
    }
}
