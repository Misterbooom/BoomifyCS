using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using BoomifyCS.Ast;
using BoomifyCS.Lexer;
using BoomifyCS.Objects;

namespace BoomifyCS.Parser
{
    public class AstParser
    {
        public static AstNode TokenToNode(Token token)
        {
            if (TokensParser.IsOperator(token.Type))
            {
                return new AstBinaryOp(token);
            }
            else if (token.Type == TokenType.NUMBER)
            {
                BifyInteger integer = new BifyInteger(token,int.Parse(token.Value));
                return new AstInt(token,integer);
            }
            else if (token.Type == TokenType.STRING)
            {
                BifyString bifyString = new BifyString(token, token.Value);
                return new AstString(token, bifyString);
            }     
            else  if (token.Type == TokenType.TRUE)
            {
                return new AstBoolean(token, new BifyBoolean(token,true));
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
            else if (token.Type == TokenType.EOL)
            {
            }
            throw new Exception("Unsupported token - " + token.Type);
        }
        public static Tuple<AstNode,int> MultiTokenStatement(Token token,List<Token> tokens,int currentPos)
        {
            if (token.Type == TokenType.VARDECL)
            {
                Tuple<AstNode, int> result = StatementParser.ParseVarDecl(token,tokens,currentPos);
                return new Tuple<AstNode, int> (result.Item1,result.Item2);
            }
            throw new NotImplementedException($"Not implemented token - {token.Type}");
        }
        public static AstNode ConnectNodes(Stack<AstNode> operatorStack,Stack<AstNode> operandStack)
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
         
    }
}
