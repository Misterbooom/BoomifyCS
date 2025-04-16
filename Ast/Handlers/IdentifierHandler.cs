using BoomifyCS.Lexer;
using BoomifyCS.Exceptions;
using System.Collections.Generic;
using BoomifyCS.Parser;
using System.Linq;

namespace BoomifyCS.Ast.Handlers
{
    class IdentifierHandler : TokenHandler
    {
        public IdentifierHandler(AstBuilder builder) : base(builder)
        {
        }

        public override void HandleToken(Token token)
        {
            if (builder.IsType(token))
            {
                HandleTypeDeclaration(token);
                return;
            }
            if (IsNextTokenFunctionCall() && builder.Nodes.Count != 0)
            {
                HandleFunctionCall(token);
                return;
            }
            HandleExpression(token);
        }

        private void HandleTypeDeclaration(Token token)
        {
            builder.Nodes.Add(new AstIdentifier(token, token.Value));
            builder.tokenIndex++;
            TokenType stopTokenType = TokenType.NULL;
            List<Token> tokens = [];
            while (!builder.IsAtEnd())
            {
                Token t = builder.NextToken();
                if (t.Type == TokenType.ASSIGN || t.Type == TokenType.LPAREN)
                {
                    stopTokenType = t.Type;
                    break;
                }
                tokens.Add(t);
            }
            if (tokens.Count == 0)
            {
                new BifySyntaxError($"Invalid function declaration.").Throw();
            } 
            builder.Nodes.Add(builder.ParseTokens(tokens));

            if (stopTokenType == TokenType.LPAREN)
            {
                builder.tokenIndex--;
                new FunctionHandler(builder).HandleToken(token);
            }
            else
            {
                new VariableDeclarationHandler(builder).HandleToken(new Token(TokenType.ASSIGN,"="));
            }


        }

        

       

        private bool IsNextTokenFunctionCall()
        {
            Token next = TokensFormatter.GetTokenOrNull(builder.tokens, builder.tokenIndex + 1);
            return next != null && next.Type == TokenType.LPAREN;
        }

        private void HandleFunctionCall(Token token)
        {
            builder.Nodes.Add(new AstIdentifier(token, token.Value));
            new FunctionHandler(builder).HandleToken(token);
        }

        private void HandleExpression(Token token)
        {
            builder.CurrentNode = new BinaryOperatorHandler(builder).ParsePrimary();
        }

        public AstNode ParseIdentfier(Token token, bool isAlreadyNextToken = false)
        {
            if (!isAlreadyNextToken)
                builder.tokenIndex++;
            return new AstIdentifier(token, token.Value);
        }

        private AstNode ParseCall(Token functionNameToken)
        {
            var args = builder.ParseTokens(builder.GetConditionTokens());
            return new AstCall(functionNameToken, NodeConventer.TokenToNode(functionNameToken), args);
        }
    }
}
