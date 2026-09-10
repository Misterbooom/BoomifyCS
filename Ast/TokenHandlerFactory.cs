using BoomifyCS.Ast;
using BoomifyCS.Ast.Handlers;
using BoomifyCS.Exceptions;
using BoomifyCS.Lexer;

internal static class TokenHandlerFactory
{
    public static TokenHandler CreateHandler(Token token, AstBuilder builder)
    {
        Traceback.Instance.SetCurrentLine(token.Line);
        if (builder.IsHandlingLine)
        {
            if (TokenConfig.BinaryOperators.ContainsValue(token.Type))
            {
                new BifySyntaxError($"Only a assignment, call, increment, decrement and new object expressions can be used as a statement.").Throw();
            }
        }

        return token.Type switch
        {
            _ when TokenConfig.BinaryOperators.ContainsValue(token.Type) => new BinaryOperatorHandler(builder),
            _ when TokenConfig.AssignmentOperators.ContainsValue(token.Type) => new AssignmentOperatorHandler(builder),
            TokenType.IDENTIFIER or TokenType.CONSTRUCTOR  => new IdentifierHandler(builder),
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

internal class DefaultTokenHandler(AstBuilder builder) : TokenHandler(builder)
{
    public override void HandleToken(Token token)
    {
        Builder.CurrentNode = new BinaryOperatorHandler(Builder).ParsePrimary();
    }

}

