using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using BoomifyCS.Ast;
using BoomifyCS.Lexer;
using BoomifyCS.Objects;

namespace BoomifyCS.Parser
{
    public class NodeParser
    {
        public static AstNode TokenToNode(Token token)
        {
            if (TokensParser.IsOperator(token.Type))
            {
                return new AstBinaryOp(token);
            }
            else if (token.Type == TokenType.NUMBER)
            {
                BifyInteger integer = new BifyInteger(token, int.Parse(token.Value));
                return new AstInt(token, integer);
            }
            else if (token.Type == TokenType.STRING)
            {
                BifyString bifyString = new BifyString(token, token.Value);
                return new AstString(token, bifyString);
            }
            else if (token.Type == TokenType.TRUE)
            {
                return new AstBoolean(token, new BifyBoolean(token, true));
            }
            else if (token.Type == TokenType.FALSE)
            {
                return new AstBoolean(token, new BifyBoolean(token, false));
            }
            else if (token.Type == TokenType.NULL)
            {
                return new AstNull(token);
            }
            else if (token.Type == TokenType.IDENTIFIER)
            {
                return new AstIdentifier(token, token.Value);
            }
            else if (token.Type == TokenType.LPAREN || token.Type == TokenType.RPAREN)
            {
                return new AstBracket(token);
            }
            else if (token.Type == TokenType.OBJECT)
            {
                AstNode node = TokenToAst(token.Tokens);
                return new AstBlock(node);
            }
            else if (token.Type == TokenType.ASSIGN)
            {
                return new AstAssignment(token);
            }
            throw new Exception("Unsupported token - " + token.Type);
        }

        public static (AstNode, int) MultiTokenStatement(Token token, List<Token> tokens,int currentPos,AstTree astParser)
        {

            if (token.Type == TokenType.VARDECL)
            {
                return StatementParser.ParseVarDecl(token, tokens, currentPos,astParser);
            }
            else if (token.Type == TokenType.IF)
            {
                return StatementParser.ParseIf(token, tokens, currentPos, astParser);
            }
            else if (token.Type == TokenType.ELSE)
            {
                return StatementParser.ParseElse(token, tokens, currentPos,astParser);
            }
            else if (token.Type == TokenType.WHILE)
            {
                return StatementParser.ParseWhile(token, tokens, currentPos,astParser);
            }
            else if (token.Type == TokenType.FOR)
            {
                return StatementParser.ParseFor(token, tokens, currentPos,astParser);
            }
            else if (token.Type == TokenType.INCREMENT || token.Type == TokenType.DECREMENT)
            {
                return StatementParser.ParseUnaryOp(token, tokens, currentPos,astParser);
            }
            else if (token.Type == TokenType.FUNCTIONDECL)
            {
                return StatementParser.ParseFunctionDecl(token, tokens, currentPos,astParser);
            }
            else if (token.Type == TokenType.IDENTIFIER)
            {
                return StatementParser.ParseIdentifier(token, tokens, currentPos,astParser);
            }
            throw new NotImplementedException($"Not implemented token to parse multitoken statement - {token.Type}");
        }

        public static AstNode ConnectNodes(Stack<AstNode> operatorStack, Stack<AstNode> operandStack)
        {
            while (operatorStack.Count > 0)
            {
                AstNode right = operandStack.Pop();
                AstNode left = operandStack.Pop();
                AstNode op = operatorStack.Pop();
                op.Left = left;
                op.Right = right;
                operandStack.Push(op);
            }

            return operandStack.First();
        }

        public static AstNode TokenToAst(List<Token> tokens)
        {
            AstTree ast = new AstTree();
            return ast.ParseTokens(tokens);
        }
        public static AstNode TokenToAst(Token token)
        {
            AstTree ast = new AstTree();
            return ast.ParseTokens(new List<Token> {token});
        }
        public static AstNode BuiltTokensToAst(Token token)
        {
            AstTree ast = new AstTree();
            return ast.BuildAstTree(new List<Token> {token});
        }
        public static AstNode BuiltTokensToAst(List<Token> tokens)
        {
            AstTree ast = new AstTree();
            return ast.BuildAstTree(tokens);
        }
        public static AstNode SetMaxRightNode(AstNode node, AstNode setNode)
        {
            if (node == null) throw new ArgumentNullException(nameof(node));

            AstNode current = node;

            while (current.Right != null)
            {
                current = current.Right;
            }

            current.Right = setNode;

            return node;
        }

    }
}
