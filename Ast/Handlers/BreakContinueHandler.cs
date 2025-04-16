using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomifyCS.Exceptions;
using BoomifyCS.Lexer;

namespace BoomifyCS.Ast.Handlers
{
    class BreakContinueHandler(AstBuilder builder):TokenHandler(builder)
    {
        public override void HandleToken(Token token)
        {
            if (builder.tokens.Count > 1)
            {
                new BifySyntaxError($"Invalid syntax of {token.Value} statement!").Throw();
            }
            if (token.Type == TokenType.BREAK)
            {
                builder.CurrentNode = new AstBreak(token);
            }
            else if (token.Type == TokenType.CONTINUE)
            {
                builder.CurrentNode = new AstContinue(token);
            }

            builder.MoveToEnd();

        }
    }
}
