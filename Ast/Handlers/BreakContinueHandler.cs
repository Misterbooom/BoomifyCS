using BoomifyCS.Exceptions;
using BoomifyCS.Lexer;

namespace BoomifyCS.Ast.Handlers
{
    internal class BreakContinueHandler(AstBuilder builder):TokenHandler(builder)
    {
        public override void HandleToken(Token token)
        {
            if (Builder.Tokens.Count > 1)
            {
                new BifySyntaxError($"Invalid syntax of {token.Value} statement!").Throw();
            }
            if (token.Type == TokenType.BREAK)
            {
                Builder.CurrentNode = new AstBreak(token);
            }
            else if (token.Type == TokenType.CONTINUE)
            {
                Builder.CurrentNode = new AstContinue(token);
            }

            Builder.MoveToEnd();

        }
    }
}
