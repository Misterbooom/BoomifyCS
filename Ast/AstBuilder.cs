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
            AstBinaryOp opNode = (AstBinaryOp)operatorStack.Pop();

            if (opNode.Token.Type == TokenType.NOT)
            {
                if (operandStack.Count == 0)
                    throw new InvalidOperationException("Not enough operands for the NOT operation.");

                AstNode operand = operandStack.Pop();
                OperandValidator.Validate(operand, operand, opNode);
                opNode.Left = operand;
                AddOperand(opNode);
                return;
            }
            else if (opNode.Token.Type == TokenType.MUL)
            {
                if (operandStack.Count == 0)
                    throw new InvalidOperationException("Not enough operands for the pointer operation.");

                AstNode operand = operandStack.Pop();
                BifyDebug.Log("Creating pointer");
                OperandValidator.Validate(operand, operand, opNode);

                Token pointerToken = new Token(TokenType.POINTER, "*Pointer");

                AstUnaryOperator pointerNode = new AstUnaryOperator(pointerToken, operand);
                AddOperand(pointerNode);
                return;
            }
            else if (operandStack.Count < 2)
            {
                BifySyntaxError error = new(ErrorMessage.NotEnoughOperands(opNode.Token.Value), "", opNode.Token.Value);
                Traceback.Instance.ThrowException(error, opNode.Token.Column);
                return;
            }

            // For other binary operations, pop two operands.
            AstNode right = operandStack.Pop();
            AstNode left = operandStack.Pop();
            OperandValidator.Validate(left, right, opNode);
            opNode.Left = left;
            opNode.Right = right;
            AddOperand(opNode);
        }


        public bool ShouldPopOperator(Token token)
        {
            if (operatorStack.Count == 0)
            {
                return false;
            }

            AstNode topOperator = operatorStack.Peek();
            int currentPrecedence = AstConfig.Precedence[token.Type];
            int topPrecedence = AstConfig.Precedence[topOperator.Token.Type];

            if (token.Type == TokenType.NOT )
            {
                return false;
            }

            bool shouldPop = currentPrecedence <= topPrecedence;
            return shouldPop;
        }

        public List<Token> GetConditionTokens() => TokensFormatter.GetTokensBetween(tokens, ref tokenIndex, TokenType.LPAREN, TokenType.RPAREN);

        public List<Token> GetBlockTokens() => TokensFormatter.GetTokensBetween(tokens, ref tokenIndex, TokenType.LCUR, TokenType.RCUR);

        public AstNode ParseCondition(List<Token> conditionTokens) => new AstBuilder(conditionTokens).BuildNode();

        public AstNode ParseBlock(List<Token> blockTokens) => new AstBlock(((AstModule)new AstTree(Traceback.Instance.source).ParseTokens(blockTokens)).ChildNodes);
    }
}
