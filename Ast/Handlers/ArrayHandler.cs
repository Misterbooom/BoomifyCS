using BoomifyCS.Lexer;

namespace BoomifyCS.Ast.Handlers
{
    class ArrayHandler(AstBuilder builder):TokenHandler(builder)
    {
        public override void HandleToken(Token token)
        {
            Builder.CurrentNode = GetArrayNode(token);
        }
        public AstNode GetArrayNode(Token token)
        {
            Builder.TokenIndex -= 1; //idk some shit that works otherwise you get error
            var tokensInBrackets = TokensFormatter.GetTokensBetween(Builder.Tokens, ref Builder.TokenIndex,
               TokenType.LBRACKET, TokenType.RBRACKET);

            AstNode valueNode = Builder.ParseTokens(tokensInBrackets);

            Builder.TokenIndex++;
            return new AstArray(token, valueNode);
        }
    }
}
