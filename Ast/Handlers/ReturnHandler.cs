using BoomifyCS.Lexer;

namespace BoomifyCS.Ast.Handlers
{
    internal class ReturnHandler(AstBuilder builder):TokenHandler(builder)
    {
        public override void HandleToken(Token token)
        {

            Builder.NextToken();
            AstNode valueNode = Builder.ParseTokens(Builder.Tokens[(Builder.TokenIndex)..]);
            Builder.MoveToEnd();
            Builder.CurrentNode = new AstReturn(token, valueNode);
        }
    }
}
