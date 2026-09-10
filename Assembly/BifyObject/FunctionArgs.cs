using System;
using System.Collections.Generic;
using System.Linq;
using BoomifyCS.Ast;
using BoomifyCS.Exceptions;
using BoomifyCS.Lexer;
using LLVMSharp.Interop;

namespace BoomifyCS.Assembly.BifyObject
{
    internal class FunctionArgs
    {
        private Dictionary<string, BifyType> _arguments = new Dictionary<string, BifyType>();

        public LLVMTypeRef[] LlvmTypes => _arguments.Values
            .Select(type => type.LlvmType)
            .ToArray();

        public BifyType[] BifyTypes => _arguments.Values.ToArray();

        public string[] ArgsNames => _arguments.Keys.ToArray();

        public FunctionArgs(AstNode argNode)
        {
            ExtractArgs(argNode);
        }

        public void SetArguments(Dictionary<string, BifyType> newArguments)
        {
            if (newArguments == null)
                throw new ArgumentNullException(nameof(newArguments), "Arguments cannot be null.");

            _arguments = new Dictionary<string, BifyType>(newArguments);
        }
        public void PrependArgument(string name, BifyType argument)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Argument name cannot be null or empty.", nameof(name));
            if (argument == null)
                throw new ArgumentNullException(nameof(argument), "Argument cannot be null.");

            var newArguments = new Dictionary<string, BifyType> { { name, argument } };
            foreach (var kvp in _arguments)
            {
                newArguments.Add(kvp.Key, kvp.Value);
            }

            _arguments = newArguments;
        }
        public bool HasSameTypes(FunctionArgs other)
        {
            
            return HasSameTypes(other.BifyTypes);
        }
        public bool HasSameTypes(BifyType[]? other, int start = 0)
        {

            if (other == null)
            {
                return false;
            }

            var thisTypes = this.BifyTypes;

            if (start < 0 || start > thisTypes.Length)
            {
                return false;
            }

            int sliceLength = thisTypes.Length - start;

            if (sliceLength != other.Length)
            {
                return false;
            }

            for (int i = 0; i < other.Length; i++)
            {
                bool compareResult = thisTypes[i + start].CompareType(other[i]);
                if (!compareResult)
                {
                    return false;
                }
            }

            return true;
        }
        public override string ToString()
        {
            return string.Join(", ", _arguments.Select(kvp => $"{kvp.Key}: {kvp.Value}"));
        }

        private void ExtractArgs(AstNode? node)
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
                if (_arguments.ContainsKey(name))
                {
                    Traceback.Instance.ThrowException(new BifyArgumentError($"Duplicate argument '{name}' found."));
                    return;
                }
                if (param.Flag != null)
                {
                    if (param.Flag.Token.Type == TokenType.CONST)
                    {
                        type.ValueFlag |= ValueFlag.CONSTANT;
                    }
                }
                _arguments.Add(name, type);
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
