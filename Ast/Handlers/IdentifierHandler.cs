using BoomifyCS.Lexer;
using System.Collections.Generic;
using System.Linq;
using BoomifyCS.Exceptions;
using BoomifyCS.Parser;

namespace BoomifyCS.Ast.Handlers
{
    internal class IdentifierHandler(AstBuilder builder) : TokenHandler(builder)
    {
        public override void HandleToken(Token token)
        {
            if (token.Type == TokenType.CONSTRUCTOR)
            {
                Builder.Nodes.Add(new AstIdentifier(new Token(TokenType.IDENTIFIER, "void"), "void"));
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

            int cachedIndex = Builder.TokenIndex;
            Builder.NextToken();

            TokenType stopTokenType = TokenType.NULL;
            List<Token> intermediateTokens = [];

            while (Builder.PeekTokenOrNull() is { } t)
            {
                if (t.Type is TokenType.ASSIGN or TokenType.LPAREN or TokenType.LBRACKET)
                {
                    stopTokenType = t.Type;
                    break;
                }

                intermediateTokens.Add(Builder.NextToken());
            }

            if (stopTokenType != TokenType.LBRACKET && intermediateTokens.Count > 0)
            {
                Builder.Nodes.Add(new AstIdentifier(token, token.Value)); 
                Builder.Nodes.Add(Builder.ParseTokens(intermediateTokens));
            }

            switch (stopTokenType)
            {
                case TokenType.LBRACKET:
                    HandleArrayOrIndexer(token, cachedIndex);
                    break;

                case TokenType.LPAREN:
                    
                    Builder.TokenIndex--;
                    new FunctionHandler(Builder).HandleToken(token);
                    return;

                default:
                    SkipPast(TokenType.ASSIGN, TokenType.SEMICOLON);
                    new VariableDeclarationHandler(Builder).HandleToken(new Token(TokenType.ASSIGN, "="));
                    BifyDebug.Log($"Var node {Builder.CurrentNode}");
                    break;
            }
        }

        private void HandleArrayOrIndexer(Token token, int startIndex)
        {
            BifyDebug.Log("Stop token is lbracket");

            Builder.TokenIndex = startIndex;
            var typeNode = new BinaryOperatorHandler(Builder).ParsePrimary();
            BifyDebug.Log($"Var type node {typeNode}");
            BifyDebug.Log($"Var name node : {Builder.Nodes.LastOrDefault()}");
            Builder.Nodes.Add(typeNode);
            
            var nameNode = new BinaryOperatorHandler(Builder).ParsePrimary();

            if (nameNode is AstCall call)
            {
                Builder.TokenIndex = startIndex;
                
                SkipUntil(TokenType.LPAREN);

                Builder.Nodes.Add(call.CallableName);
                new FunctionHandler(Builder).HandleToken(token);
                return;
            }

            Builder.Nodes.Add(nameNode);



            SkipPast(TokenType.ASSIGN, TokenType.SEMICOLON);
            new VariableDeclarationHandler(Builder).HandleToken(new Token(TokenType.ASSIGN, "="));
        }
        private void SkipUntil(params TokenType[] stopTypes)
        {
            while (Builder.PeekTokenOrNull() is { } t && !stopTypes.Contains(t.Type))
            {
                Builder.NextToken();
            }
        }
        private void SkipPast(params TokenType[] stopTypes)

        {
 
            SkipUntil(stopTypes);
            Builder.TokenIndex++;
        }
   

        private bool IsNextTokenFunctionCall()
        {
            Token next = TokensFormatter.GetTokenOrNull(Builder.Tokens, Builder.TokenIndex + 1);
            return next?.Type == TokenType.LPAREN;
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

        public AstNode ParseIdentifier(Token token, bool isAlreadyNextToken = false)
        {
            if (!isAlreadyNextToken)
            {
                Builder.NextToken();
            }

            return new AstIdentifier(token, token.Value);
        }

        private AstNode ParseCall(Token functionNameToken)
        {
            var args = Builder.ParseTokens(Builder.GetConditionTokens());
            return new AstCall(functionNameToken, NodeConventer.TokenToNode(functionNameToken), args);
        }
    }
}