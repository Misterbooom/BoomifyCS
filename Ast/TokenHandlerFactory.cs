using BoomifyCS.Ast;
using BoomifyCS.Ast.Handlers;
using BoomifyCS.Exceptions;
using BoomifyCS.Lexer;

static class TokenHandlerFactory
{
    public static TokenHandler CreateHandler(Token token, AstBuilder builder)
    {
        Traceback.Instance.SetCurrentLine(token.Line);
        if (builder.IsHandlingLine)
        {
            if (TokenConfig.binaryOperators.ContainsValue(token.Type))
            {
                new BifySyntaxError($"Only a assignment, call, increment, decrement and new object expressions can be used as a statement.").Throw();
            }
        }

        return token.Type switch
        {
            _ when TokenConfig.binaryOperators.ContainsValue(token.Type) => new BinaryOperatorHandler(builder),
            _ when TokenConfig.assignmentOperators.ContainsValue(token.Type) => new AssignmentOperatorHandler(builder),
            TokenType.IDENTIFIER or TokenType.CONSTRUCTOR => new IdentifierHandler(builder),
            TokenType.WHILE => new WhileHandler(builder),
            TokenType.FOR => new ForHandler(builder),
            TokenType.BREAK or TokenType.CONTINUE => new BreakContinueHandler(builder),
            TokenType.RETURN => new ReturnHandler(builder),
            TokenType.IF => new ConditionHandler(builder),
            TokenType.CLASS => new ClassHandler(builder),
            TokenType.NEW => new NewHandler(builder),
            _ => new DefaultTokenHandler(builder),
        };
    }

}
class DefaultTokenHandler : TokenHandler { 
    public DefaultTokenHandler(AstBuilder builder): base(builder) { }
    public override void HandleToken(Token token)
    {
        builder.CurrentNode = new BinaryOperatorHandler(builder).ParsePrimary();
    }

}

