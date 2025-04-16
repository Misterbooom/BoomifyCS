using BoomifyCS.Lexer;
using BoomifyCS.Exceptions;
using System.Collections.Generic;

namespace BoomifyCS.Ast.Handlers
{
    class BinaryOperatorHandler : TokenHandler
    {
        private Token binaryOpToken = null;
        bool isUnary = false;

        public BinaryOperatorHandler(AstBuilder builder) : base(builder) { }

        public override void HandleToken(Token token)
        {
            binaryOpToken = token;

            if (builder.Nodes.Count == 0)
            {
                isUnary = true;
            }
            else
            {
                Token previousToken = builder.GetPreviousToken();
                if (previousToken != null)
                {
                    isUnary = previousToken.Type == TokenType.LPAREN || TokenConfig.binaryOperators.ContainsValue(previousToken.Type);
                }
            }

            if (isUnary)
            {
                builder.tokenIndex++;
                AstNode operand = ParsePrimary();
                if (token.Type == TokenType.MUL)
                {
                    token.Type = TokenType.POINTER;
                }
                builder.CurrentNode = new AstUnaryOperator(token, operand,true);
                return;
            }

            AstNode left = builder.Nodes[^1];
            builder.Nodes.RemoveAt(builder.Nodes.Count - 1);

            int precedence = AstConfig.Precedence[token.Type];
            builder.tokenIndex++;

            AstNode right = ParseRight(precedence);
            if (isUnary)
            {
                builder.Nodes.Add(left);
                Token newToken = token;
                if (token.Type == TokenType.MUL)
                {
                    newToken.Type = TokenType.POINTER;
                }
                builder.CurrentNode = new AstUnaryOperator(newToken, right,true);
            }
            else
            {
                builder.CurrentNode = new AstBinaryOp(token, left, right);

            }
        }

        private AstNode ParseRight(int minPrecedence)
        {
            AstNode left = ParsePrimary();
            while (!builder.IsAtEnd())
            {
                Token next = builder.Peek();
                BifyDebug.Log($"Next token: {next}");
                if (binaryOpToken.Type == TokenType.MUL && next.Type == TokenType.ASSIGN )
                {
                    isUnary = true;
                    break;
                }

                if (!AstConfig.Precedence.TryGetValue(next.Type, out int prec) || prec < minPrecedence)
                    break;
                Token op = builder.NextToken();
                AstNode right = ParseRight(prec + 1);
                left = new AstBinaryOp(op, left, right);
            }
            return left;
        }
        private bool CheckFunctionDeclaration()
        {
            return false;
        }


        public AstNode ParsePrimary()
        {
            if (builder.IsAtEnd())
            {
                new BifySyntaxError($"Unexpected end of expression. Please check that your expression is complete.: {builder.GetPreviousToken()}").Throw();
            }

            Token token = builder.NextToken();

            AstNode baseNode = token.Type switch
            {
                TokenType.IDENTIFIER or TokenType.CONST => new IdentifierHandler(builder).ParseIdentfier(token, true),
                TokenType.NUMBER => NodeConventer.TokenToNode(token),
                TokenType.LPAREN => ParseParenthesizedExpression(),
                TokenType.SUB or TokenType.MUL => HandleUnaryOperator(token),
                TokenType.LBRACKET => new ArrayHandler(builder).GetArrayNode(token),
                TokenType.STRING => NodeConventer.TokenToNode(token),
                _ => new BifySyntaxError($"Unexpected token '{token.Value}' found in expression. Verify your syntax and try again.").Throw<AstNode>()
            };

            return ParsePostfix(baseNode);
        }

        public AstNode ParsePostfix(AstNode expr)
        {
            while (!builder.IsAtEnd())
            {
                Token next = builder.Peek();
                BifyDebug.Log($"Next postfix token: {next}");
                if (next.Type == TokenType.LPAREN)
                {
                    var args = builder.ParseTokens(builder.GetConditionTokens());
                    expr = new AstCall(next, expr, args);
                }
                else if (next.Type == TokenType.LBRACKET)
                {
                    var indexTokens = TokensFormatter.GetTokensBetween(builder.tokens, ref builder.tokenIndex,
                                TokenType.LBRACKET, TokenType.RBRACKET);

                    var indexNode = builder.ParseTokens(indexTokens);

                    expr = new AstIndexOperator(expr, indexNode);
                }
                else if (next.Type == TokenType.INCREMENT || next.Type == TokenType.DECREMENT)
                {
                    expr = new AstUnaryOperator(next, expr);
                }
                else
                {
                    break;
                }
                builder.tokenIndex++;
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
            builder.tokenIndex--;
            List<Token> innerTokens = builder.GetConditionTokens();
            AstNode node = builder.ParseTokens(innerTokens);
            builder.tokenIndex++;
            return node;
        }
    }
}
