using BoomifyCS.Lexer;

namespace BoomifyCS.Ast.Handlers
{
    class DefaultTokenHandler : TokenHandler
    {
        public DefaultTokenHandler(AstBuilder builder) : base(builder) { }

        public override void HandleToken(Token token)
        {
            builder.AddOperand(NodeConventer.TokenToNode(token));
        }
    }
}