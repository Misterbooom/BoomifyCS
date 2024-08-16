using BoomifyCS.Ast;
using BoomifyCS.Lexer;
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
                return StatementParser.Parser.ParseAssignmentOperator(token, tokens, currentPos, astParser);
            }
            return token.Type switch
            {
                TokenType.VARDECL => StatementParser.Parser.ParseVarDecl(token, tokens, currentPos, astParser),
                TokenType.IF => StatementParser.Parser.ParseIf(token, tokens, currentPos, astParser),
                TokenType.ELSE => StatementParser.Parser.ParseElse(token, tokens, currentPos, astParser),
                TokenType.WHILE => StatementParser.Parser.ParseWhile(token, tokens, currentPos, astParser),
                TokenType.FOR => StatementParser.Parser.ParseFor(token, tokens, currentPos, astParser),
                TokenType.FUNCTIONDECL => StatementParser.Parser.ParseFunctionDecl(token, tokens, currentPos, astParser),
                TokenType.IDENTIFIER => StatementParser.Parser.ParseIdentifier(token, tokens, currentPos, astParser),
                TokenType.ASSIGN => StatementParser.Parser.ParseAssignmentOperator(token, tokens, currentPos, astParser),
                TokenType.LBRACKET => StatementParser.Parser.ParseBracket(token, tokens, currentPos,astParser),
                _ => throw new NotImplementedException($"Not implemented token to parse multitoken statement - {token.Type}"),
            };
        }
    }
}