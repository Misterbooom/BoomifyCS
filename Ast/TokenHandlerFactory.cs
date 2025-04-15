using BoomifyCS.Ast;
using BoomifyCS.Ast.Handlers;
using BoomifyCS.Exceptions;
using BoomifyCS.Lexer;

static class TokenHandlerFactory
{
    public static TokenHandler CreateHandler(Token token, AstBuilder builder)
    {
        Traceback.Instance.SetCurrentLine(token.Line);
        return token.Type switch
        {
            _ when TokenConfig.binaryOperators.ContainsValue(token.Type) => new BinaryOperatorHandler(builder),
            _ when TokenConfig.assignmentOperators.ContainsValue(token.Type) => new AssignmentOperatorHandler(builder),
            TokenType.IDENTIFIER => new IdentifierHandler(builder),
            _ => new DefaultTokenHandler(builder),

        };

    }

}
class DefaultTokenHandler : TokenHandler { 
    public DefaultTokenHandler(AstBuilder builder): base(builder) { }
    public override void HandleToken(Token token)
    {
        builder.CurrentNode = NodeConventer.TokenToNode(token);
        builder.NextToken();
    }

}

