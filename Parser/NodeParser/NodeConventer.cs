using BoomifyCS.Ast;
using BoomifyCS.Exceptions;
using BoomifyCS.Lexer;
using BoomifyCS.Objects;
using BoomifyCS.Parser;
using System;

public static class NodeConverter
{
    public static AstNode TokenToNode(Token token, AstTree astTree = null)
    {
        switch (token.Type)
        {
            case var type when TokensParser.IsOperator(type):
                return new AstBinaryOp(token);

            case TokenType.NUMBER:
                return new AstInt(token, new BifyInteger(token, int.Parse(token.Value)));

            case TokenType.STRING:
                return new AstString(token, new BifyString(token, token.Value));

            case TokenType.TRUE:
                return new AstBoolean(token, new BifyBoolean(token, true));

            case TokenType.FALSE:
                return new AstBoolean(token, new BifyBoolean(token, false));

            case TokenType.NULL:
                return new AstNull(token);

            case TokenType.IDENTIFIER:
                return new AstIdentifier(token, token.Value);

            case TokenType.LPAREN:
            case TokenType.RPAREN:
                return new AstBracket(token);

            case TokenType.BLOCK when astTree != null:
                try
                {
                    var result = AstBuilder.TokenToAst(token.Tokens, astTree.modulePath);
                    var node = (AstModule)result;

                    return new AstBlock(node.ChildNode);
                }
                catch (BifyError e)
                {
                    e.CurrentLine += astTree.lineCount + 1;
                    e.LineTokensString = astTree.sourceCode[e.CurrentLine - 1];
                    throw e;
                }

            case TokenType.ASSIGN:
                return new AstAssignment(token);

            default:
                throw new Exception("Unsupported token - " + token.Type);
        }
    }
}
