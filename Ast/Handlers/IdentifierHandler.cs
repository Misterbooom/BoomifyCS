using BoomifyCS.Lexer;
using System.Collections.Generic;

namespace BoomifyCS.Ast.Handlers
{
    class IdentifierHandler(AstBuilder builder) : TokenHandler(builder)
    {
        public override void HandleToken(Token token)
        {
            if (token.Type == TokenType.CONSTRUCTOR)
            {
                Builder.Nodes.Add(new AstIdentifier(new Token(TokenType.IDENTIFIER,"void"),"void"));
                Builder.Nodes.Add(new AstIdentifier(token, token.Value));

                new FunctionHandler(Builder).HandleToken(token);
                return;
            }
            if (Builder.IsType(token))
            {
                HandleTypeDeclaration(token);
                return;
            }
            if (IsNextTokenFunctionCall() && Builder.Nodes.Count != 0)
            {
                HandleFunctionCall(token);
                return;
            }
            HandleExpression(token);
        }

        private void HandleTypeDeclaration(Token token)
        {
            Builder.Nodes.Add(new AstIdentifier(token, token.Value));
            Builder.TokenIndex++;
            TokenType stopTokenType = TokenType.NULL;
            List<Token> tokens = [];
            while (!Builder.IsAtEnd())
            {
                Token t = Builder.NextToken();
                if (t.Type == TokenType.ASSIGN || t.Type == TokenType.LPAREN)
                {
                    stopTokenType = t.Type;
                    break;
                }
                tokens.Add(t);
            }
            if (tokens.Count == 0)
            {
                return;
            }
            Builder.Nodes.Add(Builder.ParseTokens(tokens));

            if (stopTokenType == TokenType.LPAREN)
            {
                Builder.TokenIndex--;
                new FunctionHandler(Builder).HandleToken(token);
            }
            else
            {
                new VariableDeclarationHandler(Builder).HandleToken(new Token(TokenType.ASSIGN,"="));
            }


        }

        

       

        private bool IsNextTokenFunctionCall()
        {
            Token next = TokensFormatter.GetTokenOrNull(Builder.Tokens, Builder.TokenIndex + 1);
            return next != null && next.Type == TokenType.LPAREN;
        }

        private void HandleFunctionCall(Token token)
        {
            Builder.Nodes.Add(new AstIdentifier(token, token.Value));
            new FunctionHandler(Builder).HandleToken(token);
        }

        private void HandleExpression(Token token)
        {
            Builder.CurrentNode = new BinaryOperatorHandler(Builder).ParsePrimary();
        }

        public AstNode ParseIdentfier(Token token, bool isAlreadyNextToken = false)
        {
            if (!isAlreadyNextToken)
                Builder.TokenIndex++;
            return new AstIdentifier(token, token.Value);
        }

        private AstNode ParseCall(Token functionNameToken)
        {
            var args = Builder.ParseTokens(Builder.GetConditionTokens());
            return new AstCall(functionNameToken, NodeConventer.TokenToNode(functionNameToken), args);
        }
    }
}
