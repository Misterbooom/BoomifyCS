using BoomifyCS.Lexer;
using BoomifyCS.Ast.Validators;
using BoomifyCS.Ast.Handlers;
using BoomifyCS.Exceptions;
namespace BoomifyCS.Ast.Handlers
{
    static class TokenHandlerFactory
    {
        public static TokenHandler CreateHandler(Token token, AstBuilder builder)
        {
            Traceback.Instance.SetCurrentLine(token.Line);
            return token.Type switch
            {
                TokenType.VARDECL => new VariableDeclarationHandler(builder),
                TokenType.IF => new IfHandler(builder),
                TokenType.ELSE => new ElseStatementHandler(builder),
                TokenType.WHILE => new IteratorHandlers(builder),
                TokenType.IDENTIFIER => new IdentifierHandler(builder),
                TokenType.DOT => new DotHandler(builder),
                TokenType.LBRACKET => new BracketHandler(builder),
                TokenType.RETURN => new ReturnHandler(builder),
                _ when TokenConfig.assignmentOperators.ContainsValue(token.Type) => new AssignmentOperatorHandler(builder),
                _ when TokenConfig.binaryOperators.ContainsValue(token.Type) => new BinaryOperatorHandler(builder),
                TokenType.INCREMENT or TokenType.DECREMENT => new UnaryOperatorHandler(builder),
                TokenType.LPAREN => new ParenthesisHandler(builder),
                TokenType.FOR => new ForHandler(builder),

                TokenType.FUNCTIONDECL => new FunctionDeclarationHandler(builder),
                _ => new DefaultTokenHandler(builder),
            };
        }
    }
}
