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
        public bool HasSameTypes(FunctionArgs other)
        {
            
            return HasSameTypes(other.BifyTypes);
        }
        public bool HasSameTypes(BifyType[] other, int start = 0)
        {
            Console.WriteLine($"[HasSameTypes] Called with start={start}, other.Length={(other == null ? "null" : other.Length.ToString())}");

            if (other == null)
            {
                Console.WriteLine("[HasSameTypes] Other array is null.");
                return false;
            }

            var thisTypes = this.BifyTypes;
            Console.WriteLine($"[HasSameTypes] thisTypes.Length={thisTypes.Length}");

            if (start < 0 || start > thisTypes.Length)
            {
                Console.WriteLine($"[HasSameTypes] Invalid start index: {start}");
                return false;
            }

            int sliceLength = thisTypes.Length - start;
            Console.WriteLine($"[HasSameTypes] sliceLength={sliceLength}");

            if (sliceLength != other.Length)
            {
                Console.WriteLine($"[HasSameTypes] Length mismatch: sliceLength={sliceLength}, other.Length={other.Length}");
                return false;
            }

            for (int i = 0; i < other.Length; i++)
            {
                bool compareResult = thisTypes[i + start].CompareType(other[i]);
                Console.WriteLine($"[HasSameTypes] Comparing thisTypes[{i + start}] ({thisTypes[i + start]}) with other[{i}] ({other[i]}): {compareResult}");
                if (!compareResult)
                {
                    Console.WriteLine($"[HasSameTypes] Type mismatch at index {i}: {thisTypes[i + start]} vs {other[i]}");
                    return false;
                }
            }

            Console.WriteLine("[HasSameTypes] All types match.");
            return true;
        }
        public override string ToString()
        {
            return string.Join(", ", arguments.Select(kvp => $"{kvp.Key}: {kvp.Value}"));
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
