using BoomifyCS.Ast;
using BoomifyCS.Exceptions;
using BoomifyCS.Lexer;
using BoomifyCS.Objects;
using BoomifyCS.Parser;
using System;

namespace BoomifyCS.Parser.NodeParser
{
    public static class NodeConverter
    {
        public static AstNode TokenToNode(Token token, AstTree astTree = null)
        {
            // Проверка на допустимые типы токенов
            if (token == null)
            {
                throw new ArgumentNullException(nameof(token), "Token cannot be null or empty.");
            }

            AstNode returnNode;

            // Используем switch с pattern matching для определения типа токена
            switch (token.Type)
            {
                case var type when TokensParser.IsOperator(type):
                    returnNode = new AstBinaryOp(token);
                    break;

                case TokenType.NUMBER:
                    if (!int.TryParse(token.Value, out int parsedValue))
                    {
                        throw new FormatException($"Token value '{token.Value}' is not a valid integer.");
                    }
                    returnNode = new AstInt(token, new BifyInteger(token, parsedValue));
                    break;

                case TokenType.STRING:
                    returnNode = new AstString(token, new BifyString(token, token.Value));
                    break;

                case TokenType.TRUE:
                    returnNode = new AstBoolean(token, new BifyBoolean(token, true));
                    break;

                case TokenType.FALSE:
                    returnNode = new AstBoolean(token, new BifyBoolean(token, false));
                    break;

                case TokenType.NULL:
                    returnNode = new AstNull(token);
                    break;

                case TokenType.IDENTIFIER:
                    returnNode = new AstIdentifier(token, token.Value);
                    break;

                case TokenType.LPAREN:
                case TokenType.RPAREN:
                    returnNode = new AstBracket(token);
                    break;

                case TokenType.BLOCK when astTree != null:
                    try
                    {
                        var result = AstBuilder.TokenToAst(token.Tokens, astTree.modulePath, ref astTree.lineCount);

                        var node = (AstModule)result;

                        returnNode = new AstBlock(node.ChildNode);
                    }
                    catch (BifyError e)
                    {
                        e.CurrentLine += astTree.lineCount + 1;
                        e.LineTokensString = astTree.sourceCode[e.CurrentLine - 1];
                        throw e;
                    }
                    break;

                case TokenType.ASSIGN:
                    returnNode = new AstAssignment(token);
                    break;
                case TokenType.BREAK:
                    returnNode = new AstBreak(token);
                    break;
                case TokenType.CONTINUE:
                    returnNode = new AstContinue(token);
                    break;

                default:
                    throw new NotSupportedException($"Unsupported token type: {token.Type}");
            }

            if (returnNode != null)
            {
                returnNode.LineNumber = astTree?.lineCount ?? 0;
                return returnNode;
            }

            throw new Exception("Unsupported token - " + token.Type);
        }

    }
}