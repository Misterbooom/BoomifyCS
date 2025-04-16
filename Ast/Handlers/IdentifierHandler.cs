using BoomifyCS.Lexer;
using BoomifyCS.Exceptions;
using System.Collections.Generic;
using BoomifyCS.Parser;

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
            ProcessPointerTokens();
            Token possibleFunctionName = TokensFormatter.GetTokenOrNull(builder.tokens, builder.tokenIndex);
            if (possibleFunctionName != null && possibleFunctionName.Type == TokenType.IDENTIFIER)
            {
                builder.Nodes.Add(new AstIdentifier(possibleFunctionName, possibleFunctionName.Value));
                builder.tokenIndex++;
                if (!builder.IsAtEnd() && builder.Peek().Type == TokenType.LPAREN)
                {
                    new FunctionHandler(builder).HandleToken(token);
                    return;
                }
                else
                {
                    new VariableDeclarationHandler(builder).HandleToken(token);
                    return;
                }
            }
            new VariableDeclarationHandler(builder).HandleToken(token);
        }

        private void ProcessPointerTokens()
        {
            while (!builder.IsAtEnd() && builder.Peek().Type == TokenType.MUL)
            {
                Token pointerToken = builder.NextToken();
                pointerToken.Type = TokenType.POINTER;
                builder.Nodes.Add(new AstUnaryOperator(pointerToken, null, true));
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
