using System.Collections.Generic;
using BoomifyCS.Assembly.BifyObject;
using BoomifyCS.Ast;
using BoomifyCS.Exceptions;

namespace BoomifyCS.Assembly
{
    public enum FlagContext
    {
        CLASS_ATTRIBUTE,
        METHOD,
        FUNCTION,
        VARIABLE,
    }
    class FlagProcessor
    {
       
        public static void SetFlags(FlagContext flagContext, BifyType type, List<AstNode> flags)
        {
            foreach (var flag in flags)
            {
                if (flag.Token.Value == "const")
                {
                    if (flagContext is not (FlagContext.VARIABLE or FlagContext.CLASS_ATTRIBUTE))
                        new BifyArgumentError($"Invalid flag 'const' for {flagContext.ToString().ToLower()}.").Throw();
                    type.ValueFlag |= ValueFlag.CONSTANT;
                    continue;
                }
                var accessLevel = GetAccesLevel(flagContext, flag.Token.Value);
                type.AccessLevel = accessLevel;
            }
        }
        private  static AccessLevel GetAccesLevel(FlagContext flagContext, string name)
        {
            if (string.IsNullOrEmpty(name))
                return AccessLevel.PRIVATE;

            return flagContext switch
            {
                FlagContext.CLASS_ATTRIBUTE => GetAttributeAccessLevel(name),
                FlagContext.METHOD => GetMethodAccessLevel(name),
                FlagContext.FUNCTION => GetFunctionAccessLevel(name),
                _ => new BifyArgumentError($"Unsupported flag context: {flagContext}").Throw<AccessLevel>()
            };
        }

        private static AccessLevel GetAttributeAccessLevel(string name) => name switch
        {
            "public" => AccessLevel.PUBLIC,
            "private" => AccessLevel.PRIVATE,
            "protected" => AccessLevel.PROTECTED,
            _ => new BifyArgumentError($"Invalid flag '{name}' for class attribute.").Throw<AccessLevel>()
        };

        private static AccessLevel GetMethodAccessLevel(string name) => name switch
        {
            "public" => AccessLevel.PUBLIC,
            "private" => AccessLevel.PRIVATE,
            "protected" => AccessLevel.PROTECTED,
            _ => new BifyArgumentError($"Invalid flag '{name}' for method.").Throw<AccessLevel>()
        };

        private static AccessLevel GetFunctionAccessLevel(string name) => name switch
        {
            
            _ => new BifyArgumentError($"Invalid flag '{name}' for function.").Throw<AccessLevel>()
        };

    }

    
}
