using System;
using System.Collections.Generic;
using BoomifyCS.Ast.Validators;
using BoomifyCS.Exceptions;
using BoomifyCS.Lexer;
using BoomifyCS.Parser;
using BoomifyCS.Ast.Handlers;
using System.Linq;
namespace BoomifyCS.Ast
{
    class AstBuilder
    {
        public int tokenIndex = 0;
        public List<Token> tokens;
        public readonly Stack<AstNode> operatorStack = new();
        public readonly Stack<AstNode> operandStack = new();

        public AstBuilder(List<Token> tokens)
        {
            this.tokens = tokens;
        }

        public AstNode BuildNode()
        {
  
            while (tokenIndex < tokens.Count)
            {
                Token token = tokens[tokenIndex];
                //BifyDebug.Log($"Token: {token}");
                TokenHandler handler = TokenHandlerFactory.CreateHandler(token, this);
                handler.HandleToken(token);

                tokenIndex++;
            }
           
            while (operandStack.Count > 1)
            {

                AstNode right = operandStack.Pop();
                AstNode left = operandStack.Pop();

                AstNode combinedNode = new AstBinaryOp(new Token(TokenType.ADD, "concat"));

                OperandValidator.Validate(left, right, combinedNode);

                combinedNode.Left = left;
                combinedNode.Right = right;

                AddOperand(combinedNode);
            }
            return operandStack.Count > 0 ? operandStack.Pop() : null;
        }
        public AstNode[] BuildWithoutConnecting()
        {
            while (tokenIndex < tokens.Count)
            {
                Token token = tokens[tokenIndex];
                TokenHandler handler = TokenHandlerFactory.CreateHandler(token, this);
                handler.HandleToken(token);

                tokenIndex++;
            }
            return operandStack.ToArray();
        }

        public void AddOperand(AstNode node)
        {
            if (node != null)
            {
                node.LineNumber = Traceback.Instance.Line;
                operandStack.Push(node);
            }

        }

        public void AddOperator(AstNode node)
        {
            node.LineNumber = Traceback.Instance.Line;
            operatorStack.Push(node);
        }

        



        

        public List<Token> GetConditionTokens() => TokensFormatter.GetTokensBetween(tokens, ref tokenIndex, TokenType.LPAREN, TokenType.RPAREN);

        public List<Token> GetBlockTokens() => TokensFormatter.GetTokensBetween(tokens, ref tokenIndex, TokenType.LCUR, TokenType.RCUR);

        public AstNode ParseTokens(List<Token> conditionTokens) => new AstBuilder(conditionTokens).BuildNode();

        public AstNode ParseBlock(List<Token> blockTokens) => new AstBlock(((AstModule)new AstTree(Traceback.Instance.source).ParseTokens(blockTokens)).ChildNodes);
    }
}
