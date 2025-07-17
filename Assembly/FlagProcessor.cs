using System;
using System.Collections.Generic;
using BoomifyCS.Assembly.BifyObject;
using BoomifyCS.Ast;
using BoomifyCS.Exceptions;

namespace BoomifyCS.Assembly
{
    public enum FlagContext
    {
        ClassAttribute,
        Method,
        Function,
        Variable,
    }
    class FlagProcessor
    {
       
        public static void SetFlags(FlagContext flagContext,BifyType type,List<AstNode> flags)
        {
          
            foreach (var flag in flags)
            {
                var valueFlag = GetFlag(flagContext, flag.Token.Value);
                if (valueFlag != ValueFlag.None)
                {
                    type.ValueFlag |= valueFlag;
                }
            }
        }
        private  static ValueFlag GetFlag(FlagContext flagContext, string name)
        {
            if (string.IsNullOrEmpty(name))
                return ValueFlag.None;

            return flagContext switch
            {
                FlagContext.ClassAttribute => GetClassAttributeFlag(name),
                FlagContext.Method => GetMethodFlag(name),
                FlagContext.Function => GetFunctionFlag(name),
                FlagContext.Variable => GetVariableFlag(name),
                _ => new BifyArgumentError($"Unsupported flag context: {flagContext}").Throw<ValueFlag>()
            };
        }

        private static ValueFlag GetClassAttributeFlag(string name) => name switch
        {
            "public" => ValueFlag.Public,
            "private" => ValueFlag.Private,
            "protected" => ValueFlag.Protected,
            _ => new BifyArgumentError($"Invalid flag '{name}' for class attribute.").Throw<ValueFlag>()
        };

        private static ValueFlag GetMethodFlag(string name) => name switch
        {
            "public" => ValueFlag.Public,
            "private" => ValueFlag.Private,
            "protected" => ValueFlag.Protected,
            "const" => ValueFlag.Constant,
            _ => new BifyArgumentError($"Invalid flag '{name}' for method.").Throw<ValueFlag>()
        };

        private static ValueFlag GetFunctionFlag(string name) => name switch
        {
            
            _ => new BifyArgumentError($"Invalid flag '{name}' for function.").Throw<ValueFlag>()
        };

        private static ValueFlag GetVariableFlag(string name) => name switch
        {
            "const" => ValueFlag.Constant,
            _ => new BifyArgumentError($"Invalid flag '{name}' for Variable context").Throw<ValueFlag>()
        };
    }

    
}
