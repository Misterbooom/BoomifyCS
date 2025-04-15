using BoomifyCS.Lexer;
using BoomifyCS.Exceptions;
using System.Collections.Generic;

namespace BoomifyCS.Ast.Handlers
{
    class IdentifierHandler(AstBuilder builder) : TokenHandler(builder)
    {
        public override void HandleToken(Token token)
        {
            if (builder.Nodes.Count == 1 && builder.tokenIndex + 1 >= builder.tokens.Count)
            {
                builder.Nodes.Add(new AstIdentifier(token, token.Value));
                new VariableDeclarationHandler(builder).HandleToken(token);
            }
            else
            {
                builder.CurrentNode = ParseIdentfierOrCall(token);
            }
        }

        public AstNode ParseIdentfierOrCall(Token token, bool isAlreadyNextToken = false)
        {
            if (builder.IsAtEnd() || builder.tokenIndex + 1 >= builder.tokens.Count)
            {
                if (!isAlreadyNextToken)
                    builder.NextToken();
                return new AstIdentifier(token, token.Value);
            }

            Token nextToken = TokensFormatter.GetTokenOrNull(builder.tokens, isAlreadyNextToken ? builder.tokenIndex : builder.tokenIndex + 1);
            if (nextToken?.Type == TokenType.LPAREN)
            {
                return ParseCall(token);
            }
            else
            {
                if (!isAlreadyNextToken)
                    builder.NextToken();
                return new AstIdentifier(token, token.Value);
            }
        }

        private AstNode ParseCall(Token functionNameToken)
        {
            var args = builder.ParseTokens(builder.GetConditionTokens());
            return new AstCall(functionNameToken, NodeConventer.TokenToNode(functionNameToken), args);
        }
    }
}
