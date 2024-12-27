using System;
using System.Collections.Generic;
using BoomifyCS.Ast.Validators;
using BoomifyCS.Exceptions;
using BoomifyCS.Lexer;
using BoomifyCS.Parser;
using BoomifyCS.Ast.Handlers;
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
                Console.WriteLine(tokens[tokenIndex]);
                TokenHandler handler = TokenHandlerFactory.CreateHandler(token, this);
                handler.HandleToken(token);
                tokenIndex++;
            }
            while (operatorStack.Count > 0)
            {
                PopOperator();
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

        public void AddOperand(AstNode node)
        {
            if (node != null)
            {
                node.LineNumber = Traceback.Instance.line;
                operandStack.Push(node);
            }
        }

        public void AddOperator(AstNode node)
        {
            node.LineNumber = Traceback.Instance.line;
            operatorStack.Push(node);
        }

        public void PopOperator()
        {
            AstBinaryOp operatorNode = (AstBinaryOp)operatorStack.Pop();
            if (operatorNode.Token.Type == TokenType.NOT)
            {
                AstNode operand = operandStack.Pop();
                OperandValidator.Validate(operand, operand, operatorNode);
                operatorNode.Left = operand;
                AddOperand(operatorNode);
                return;
            }
            else if (operandStack.Count < 2)
            {
                BifySyntaxError error = new BifySyntaxError(ErrorMessage.NotEnoughOperands(operatorNode.Token.Value), "", operatorNode.Token.Value);
                Traceback.Instance.ThrowException(error, operatorNode.Token.Column);
            }
            AstNode right = operandStack.Pop();
            AstNode left = operandStack.Pop();
            OperandValidator.Validate(left, right, operatorNode);
            operatorNode.Left = left;
            operatorNode.Right = right;
            AddOperand(operatorNode);
        }

        public bool ShouldPopOperator(Token token)
        {
            if (operatorStack.Count == 0)
                return false;

            AstNode topOperator = operatorStack.Peek();
            int currentPrecedence = AstConfig.Precedence[token.Type];
            int topPrecedence = AstConfig.Precedence[topOperator.Token.Type];

            return currentPrecedence <= topPrecedence;
        }

        public List<Token> GetConditionTokens() => TokensFormatter.GetTokensBetween(tokens, ref tokenIndex, TokenType.LPAREN, TokenType.RPAREN);

        public List<Token> GetBlockTokens() => TokensFormatter.GetTokensBetween(tokens, ref tokenIndex, TokenType.LCUR, TokenType.RCUR);

        public AstNode ParseCondition(List<Token> conditionTokens) => new AstBuilder(conditionTokens).BuildNode();

        public AstNode ParseBlock(List<Token> blockTokens) => new AstBlock(((AstModule)new AstTree(Traceback.Instance.source).ParseTokens(blockTokens)).ChildNodes);
    }
}
