using System;
using System.Collections.Generic;
using BoomifyCS.Ast.Validators;
using BoomifyCS.Exceptions;
using BoomifyCS.Lexer;
using BoomifyCS.Parser;
using System.Linq;

namespace BoomifyCS.Ast
{
    class AstBuilder
    {
        public int tokenIndex = 0;
        public List<Token> tokens;
        // Хранит текущий узел, созданный обработчиком
        public AstNode? CurrentNode { get; set; }
        // Собираем все разобранные узлы в списке
        public List<AstNode> Nodes { get; set; } = new List<AstNode>();

        public AstBuilder(List<Token> tokens)
        {
            this.tokens = tokens;
        }

        public AstNode BuildNode()
        {
            while (tokenIndex < tokens.Count)
            {
                Token token = tokens[tokenIndex];
                var handler = TokenHandlerFactory.CreateHandler(token, this);
                handler.HandleToken(token);

                if (CurrentNode != null)
                {
                    Nodes.Add(CurrentNode);
                    CurrentNode = null;
                }
            }
            if (Nodes.Count > 1)
            {
                Nodes.WriteNodes();
                new BifySyntaxError("Multiple nodes generated—this likely indicates ambiguous or incomplete syntax. Please check your input for missing operators or delimiters.").Throw();
            }
            return Nodes.Count == 1 ? Nodes[0] : null;
        }

        public Token GetPreviousToken()
        {
            if (tokenIndex == 0)
                return null;
            return tokens[tokenIndex - 1];
        }
        public Token Peek()
        {
            return tokens[tokenIndex];
        }

        public Token NextToken()
        {
            return tokens[tokenIndex++];
        }

        public bool IsAtEnd() => tokenIndex >= tokens.Count;

        public Token Consume(TokenType type, string message = "Unexpected token")
        {
            if (Peek().Type == type)
            {
                return NextToken();
            }
            else
            {
                new BifySyntaxError(message).Throw();
                return null;

            }
        }
       

        public List<Token> GetConditionTokens() => TokensFormatter.GetTokensBetween(tokens, ref tokenIndex, TokenType.LPAREN, TokenType.RPAREN);

        public List<Token> GetBlockTokens() => TokensFormatter.GetTokensBetween(tokens, ref tokenIndex, TokenType.LCUR, TokenType.RCUR);

        public AstNode ParseTokens(List<Token> conditionTokens) => new AstBuilder(conditionTokens).BuildNode();

        public AstNode ParseBlock(List<Token> blockTokens) => new AstBlock(((AstModule)new AstTree(Traceback.Instance.source).ParseTokens(blockTokens)).ChildNodes);
    }
}
