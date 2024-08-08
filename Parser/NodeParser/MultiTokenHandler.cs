using BoomifyCS.Ast;
using BoomifyCS.Lexer;
using BoomifyCS.Parser;
using System.Collections.Generic;
using System;

namespace BoomifyCS.Parser.NodeParser
{
    public static class MultiTokenHandler
    {
        public static (AstNode, int) ProcessMultiTokenStatement(Token token, List<Token> tokens, int currentPos, AstTree astParser)
        {
            if (TokenConfig.assignmentOperators.ContainsValue(token.Type))
            {
                return StatementParser.ParseAssignmentOperator(token, tokens, currentPos, astParser);
            }
            return token.Type switch
            {
                TokenType.VARDECL => StatementParser.ParseVarDecl(token, tokens, currentPos, astParser),
                TokenType.IF => StatementParser.ParseIf(token, tokens, currentPos, astParser),
                TokenType.ELSE => StatementParser.ParseElse(token, tokens, currentPos, astParser),
                TokenType.WHILE => StatementParser.ParseWhile(token, tokens, currentPos, astParser),
                TokenType.FOR => StatementParser.ParseFor(token, tokens, currentPos, astParser),
                TokenType.INCREMENT or TokenType.DECREMENT => StatementParser.ParseUnaryOp(token, tokens, currentPos, astParser),
                TokenType.FUNCTIONDECL => StatementParser.ParseFunctionDecl(token, tokens, currentPos, astParser),
                TokenType.IDENTIFIER => StatementParser.ParseIdentifier(token, tokens, currentPos, astParser),
                _ => throw new NotImplementedException($"Not implemented token to parse multitoken statement - {token.Type}"),
            };
        }
    }
}