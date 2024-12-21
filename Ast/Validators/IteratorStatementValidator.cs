using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomifyCS.Exceptions;
using BoomifyCS.Lexer;

namespace BoomifyCS.Ast.Validators
{
    class IteratorStatementValidator
    {
        public static void ValidateWhileStatement(List<Token> conditionTokens, AstWhile astWhile)
        {
            if (conditionTokens.Count == 0)
            {
                BifySyntaxError error = new(ErrorMessage.ConditionIsRequired(),"",astWhile.Token.Value);
                Traceback.Traceback.Instance.ThrowException(error, astWhile.Token.Column);
            }
        }
    }
}
