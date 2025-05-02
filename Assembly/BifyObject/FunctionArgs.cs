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
        public void PrependArgument(string name, BifyType argument)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Argument name cannot be null or empty.", nameof(name));
            if (argument == null)
                throw new ArgumentNullException(nameof(argument), "Argument cannot be null.");

            var newArguments = new Dictionary<string, BifyType> { { name, argument } };
            foreach (var kvp in arguments)
            {
                newArguments.Add(kvp.Key, kvp.Value);
            }

            arguments = newArguments;
        }

        private void ExtractArgs(AstNode node)
        {
            if (node == null)
            {
                return;
            }
            if (node is AstParam)
            {
                AstParam param = (AstParam)node;
                string name = param.Name.Token.Value;
                AssemblyCompiler.Instance.Visit(param.Type);
                IValue value = AssemblyCompiler.Instance.StackIValuePop();
                if (value is not BifyType)
                {
                    Traceback.Instance.ThrowException(new BifyTypeError($"{value.GetType().Name.ToLower()} cannot be used as type."));
                    return;
                }
                BifyType type = (BifyType)value;
                if (arguments.ContainsKey(name))
                {
                    Traceback.Instance.ThrowException(new BifyArgumentError($"Duplicate argument '{name}' found."));
                    return;
                }
                if (param.Flag != null)
                {
                    if (param.Flag.Token.Type == TokenType.CONST)
                    {
                        type.ValueFlag |= ValueFlag.Constant;
                    }
                }
                arguments.Add(name, type);
            }
            else if (node.Token.Type == TokenType.COMMA)
            {
                ExtractArgs(node.Left);
                ExtractArgs(node.Right);
            }
            else
            {
                throw new ArgumentException("Expected an AstParam node.");
            }
        }

    }
}
