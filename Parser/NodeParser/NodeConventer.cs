using BoomifyCS.Ast;
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

            case TokenType.OBJECT when astTree != null:
                var result = astTree.ParseTokens(token.Tokens);
                Console.WriteLine(result.ToString());
                var node = (AstModule)result;
                
                return new AstBlock(node.ChildNode);

            case TokenType.ASSIGN:
                return new AstAssignment(token);

            default:
                throw new Exception("Unsupported token - " + token.Type);
        }
    }
}
