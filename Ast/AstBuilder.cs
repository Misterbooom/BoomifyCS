using System;
using System.Collections.Generic;
using BoomifyCS.Ast.Validators;
using BoomifyCS.Exceptions;
using BoomifyCS.Lexer;
using BoomifyCS.Parser;
using System.Linq;
using BoomifyCS.Assembly;

namespace BoomifyCS.Ast
{
    class AstBuilder
    {
        public int tokenIndex = 0;
        public List<Token> tokens;
        public AstNode? CurrentNode { get; set; }
        public List<AstNode> Nodes { get; set; } = new List<AstNode>();

        public AstBuilder(List<Token> tokens)
        {
            this.tokens = tokens;
        }

        public AstNode BuildNode()
        {
            if (tokens == null || tokens.Count == 0)
            {
                return null;
            }
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
        public AstNode[] BuildMultipleNodes()
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

            return Nodes.ToArray();
        }
        public Token GetPreviousToken()
        {
            if (tokenIndex == 0)
                return null;
            return tokens[tokenIndex - 1];
        }
        public Token Peek()
        {
            return tokenIndex < tokens.Count ? tokens[tokenIndex] : null;
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

        public bool IsType(Token token)
        {
            AssemblyCompiler compiler = AssemblyCompiler.Instance;
            if (compiler.VariableManager.TryGetBifyType(token.Value) != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public Token GetNextToken()
        {
            if (tokenIndex >= tokens.Count)
            {
                return null;
            }
            Token token = tokens[tokenIndex + 1];
            return token;
        }
        public void MoveToEnd()
        {
            tokenIndex = tokens.Count;
        }
        public List<Token> GetConditionTokens() => TokensFormatter.GetTokensBetween(tokens, ref tokenIndex, TokenType.LPAREN, TokenType.RPAREN);

        public List<Token> GetBlockTokens() => TokensFormatter.GetTokensBetween(tokens, ref tokenIndex, TokenType.LCUR, TokenType.RCUR);

        public AstNode ParseTokens(List<Token> conditionTokens) => new AstBuilder(conditionTokens).BuildNode();

        public AstNode ParseBlock(List<Token> blockTokens) => new AstBlock(((AstModule)new AstTree(Traceback.Instance.source).ParseTokens(blockTokens)).ChildNodes);
    }
}
