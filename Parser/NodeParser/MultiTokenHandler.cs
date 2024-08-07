using BoomifyCS.Ast;
using BoomifyCS.Lexer;
using BoomifyCS.Parser;
using System.Collections.Generic;
using System;

public static class MultiTokenHandler
{
    public static (AstNode, int) ProcessMultiTokenStatement(Token token, List<Token> tokens, int currentPos, AstTree astParser)
    {
        if (TokenConfig.assignmentOperators.ContainsValue(token.Type))
        {
            return StatementParser.ParseAssignmentOperator(token, tokens, currentPos, astParser);
        }
        switch (token.Type)
        {
            case TokenType.VARDECL:
                return StatementParser.ParseVarDecl(token, tokens, currentPos, astParser);

            case TokenType.IF:
                return StatementParser.ParseIf(token, tokens, currentPos, astParser);

            case TokenType.ELSE:
                return StatementParser.ParseElse(token, tokens, currentPos, astParser);

            case TokenType.WHILE:
                return StatementParser.ParseWhile(token, tokens, currentPos, astParser);

            case TokenType.FOR:
                return StatementParser.ParseFor(token, tokens, currentPos, astParser);

            case TokenType.INCREMENT:
            case TokenType.DECREMENT:
                return StatementParser.ParseUnaryOp(token, tokens, currentPos, astParser);

            case TokenType.FUNCTIONDECL:
                return StatementParser.ParseFunctionDecl(token, tokens, currentPos, astParser);

            case TokenType.IDENTIFIER:
                return StatementParser.ParseIdentifier(token, tokens, currentPos, astParser);
            default:
                throw new NotImplementedException($"Not implemented token to parse multitoken statement - {token.Type}");
        }
    }
}
