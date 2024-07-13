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
        public static AstNode ConnectNodes(List<AstNode> operatorStack,List<AstNode> operandStack)
        {
            while (operatorStack.Count > 0)
            {
                AstNode right = operandStack.Pop();
                AstNode left = operandStack.Pop();
                AstNode op = operatorStack.Pop();
                op.Left = left;
                op.Right = right;
                operandStack.Add(op);
            }
            


            return operandStack[0];
        }
        public static object SimpleEval(AstNode node)
        {
            if (node == null)
            {
                return null;
            }
            if (node is AstBinaryOp binaryOpNode)
            {
                object leftValue = SimpleEval(binaryOpNode.Left);   
                object rightValue = SimpleEval(binaryOpNode.Right);
                if (leftValue != null && rightValue != null)
                {
                    if (leftValue is BifyObject bifyLeft && rightValue is BifyObject bifyRight)
                    {
                        return CalculateBifyObjects(bifyLeft,bifyRight,binaryOpNode.Token.Type);
                    }
                }



            }
            else if (node is AstInt astIntNode)
            {
                var bifyValue = astIntNode.BifyValue;
                return bifyValue;
            }
            throw new InvalidOperationException($"Unsupported node type: {node.GetType().Name}");
        }
        public static BifyObject CalculateBifyObjects(BifyObject a,BifyObject b,TokenType op)
        {
            if (op == TokenType.ADD)
            {
                return a.Add(b);
            }
            else if (op == TokenType.SUB)
            {
                return a.Sub(b);
            }
            else if (op == TokenType.MUL)
            {
                return a.Mul(b);
            }
            else if (op == TokenType.DIV)
            {
                return a.Div(b);
            }
            else if (op == TokenType.MOD)
            {
                return a.Mod(b);
            }
            else if (op == TokenType.POW)
            {
                return a.Pow(b);
            }
            else if (op == TokenType.FLOORDIV)
            {
                return a.FloorDiv(b);
            }
            throw new InvalidOperationException($"Unsupported operator: {op}");
        }
        public static AstNode TokenToAst(List<Token> tokens) 
        {
            AstTree ast = new AstTree();

            return ast.ParseTokens(tokens);
        }
         
    }
}
