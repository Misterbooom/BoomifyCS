﻿using BoomifyCS.Lexer;
using BoomifyCS.Exceptions;
using System.Collections.Generic;

namespace BoomifyCS.Ast.Handlers
{
    internal class BinaryOperatorHandler(AstBuilder builder) : TokenHandler(builder)
    {
        private Token _binaryOpToken = null;
        bool _isUnary = false;

        public override void HandleToken(Token token)
        {
            
            _binaryOpToken = token;

            if (Builder.Nodes.Count == 0)
            {
                _isUnary = true;
            }
            else
            {
                Token previousToken = Builder.GetPreviousToken();
                if (previousToken != null)
                {
                    _isUnary = previousToken.Type == TokenType.LPAREN || TokenConfig.BinaryOperators.ContainsValue(previousToken.Type);
                }
            }

            if (_isUnary)
            {
                Builder.TokenIndex++;
                AstNode operand = ParsePrimary();
                if (token.Type == TokenType.MUL)
                {
                    token.Type = TokenType.POINTER;
                }
                Builder.CurrentNode = new AstUnaryOperator(token, operand,true);
                return;
            }

            AstNode left = Builder.Nodes[^1];
            Builder.Nodes.RemoveAt(Builder.Nodes.Count - 1);

            int precedence = AstConfig.Precedence[token.Type];
            Builder.TokenIndex++;

            AstNode right = ParseRight(precedence);
            if (_isUnary)
            {
                Builder.Nodes.Add(left);
                Token newToken = token;
                if (token.Type == TokenType.MUL)
                {
                    newToken.Type = TokenType.POINTER;
                }
                Builder.CurrentNode = new AstUnaryOperator(newToken, right,true);
            }
            else
            {
                Builder.CurrentNode = new AstBinaryOp(token, left, right);

            }
        }

        private AstNode ParseRight(int minPrecedence)
        {
            AstNode left = ParsePrimary();
            while (!Builder.IsAtEnd())
            {
                Token next = Builder.Peek();
                
                if (_binaryOpToken.Type == TokenType.MUL && next.Type == TokenType.ASSIGN )
                {
                    _isUnary = true;
                    break;
                }

                if (!AstConfig.Precedence.TryGetValue(next.Type, out int prec) || prec < minPrecedence)
                    break;
                Token op = Builder.NextToken();
                AstNode right = ParseRight(prec + 1);
                left = new AstBinaryOp(op, left, right);
            }
            return left;
        }
        private bool CheckFunctionDeclaration()
        {
            return false;
        }


        public AstNode ParsePrimary(bool handlePostfix = true)
        {
            if (Builder.IsAtEnd())
            {
                new BifySyntaxError($"Unexpected end of expression. Please check that your expression is complete.: {Builder.GetPreviousToken()}").Throw();
            }

            Token token = Builder.NextToken();

            AstNode baseNode = token.Type switch
            {
                TokenType.IDENTIFIER or TokenType.CONST => new IdentifierHandler(Builder).ParseIdentifier(token, true),
                TokenType.NUMBER => NodeConventer.TokenToNode(token),
                TokenType.LPAREN => ParseParenthesizedExpression(),
                TokenType.SUB or TokenType.MUL or TokenType.INCREMENT or TokenType.DECREMENT or TokenType.NOT=> HandleUnaryOperator(token),
                TokenType.LBRACKET => new ArrayHandler(Builder).GetArrayNode(token),
                TokenType.STRING or TokenType.CHAR => NodeConventer.TokenToNode(token),
                TokenType.NEW => new NewHandler(Builder).ParseNewExpression(token),
                _ => new BifySyntaxError($"Unexpected token '{token.Value}' found in expression. Verify your syntax and try again.").Throw<AstNode>()
            };
            baseNode.LineNumber = token.Line;
            if (handlePostfix)
                return ParsePostfix(baseNode);
            else
                return baseNode;
        }

        private AstNode ParsePostfix(AstNode expr)
        {
            while (!Builder.IsAtEnd())
            {
                Token next = Builder.Peek();
                if (next.Type == TokenType.LPAREN)
                {
                    var args = Builder.ParseTokens(Builder.GetConditionTokens());
                    expr = new AstCall(next, expr, args);
                }
                else if (next.Type == TokenType.LBRACKET)
                {
                    var indexTokens = TokensFormatter.GetTokensBetween(Builder.Tokens, ref Builder.TokenIndex,
                                TokenType.LBRACKET, TokenType.RBRACKET);
                    
                    var indexNode = Builder.ParseTokens(indexTokens);
                    expr = new AstIndexOperator(indexNode, expr);
                }
                else if (next.Type is TokenType.INCREMENT or TokenType.DECREMENT)
                {
                    expr = new AstUnaryOperator(next, expr);
                }
                else if (next.Type == TokenType.DOT)
                {
                    Builder.NextToken();
                    expr = new AstMemberAccess(next, expr,ParsePrimary(false));
                    Builder.TokenIndex--;
                }
                else
                {
                    break;
                }
                expr.LineNumber = next.Line;
                Builder.TokenIndex++;
            }
            return expr;
        }

        private AstNode  HandleUnaryOperator(Token opToken)
        {
            AstNode operand = ParsePrimary();
            if (opToken.Type == TokenType.MUL)
            {
                opToken.Type = TokenType.POINTER;
            }
            return new AstUnaryOperator(opToken, operand,true);
        }

        private AstNode ParseParenthesizedExpression()
        {
            Builder.TokenIndex--;
            List<Token> innerTokens = Builder.GetConditionTokens();
            AstNode node = Builder.ParseTokens(innerTokens);
            Builder.TokenIndex++;
            if (node?.Token.Type == TokenType.POINTER || node is AstIdentifier)
            {
                AstNode valueNode = ParsePrimary();
                AstCast castNode = new(node.Token, node, valueNode);
                return castNode;
            }
            return node;
        }
    }
}
