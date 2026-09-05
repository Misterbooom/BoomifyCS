using System;
using System.Collections.Generic;
using BoomifyCS.Exceptions;
using BoomifyCS.Lexer;
using BoomifyCS.Parser;
using BoomifyCS.Assembly;

namespace BoomifyCS.Ast
{
    class AstBuilder
    {
        public int TokenIndex = 0;
        public readonly List<Token> Tokens;
        public AstNode? CurrentNode
        {
            get => _currentNode;
            set
            {
                _currentNode = value;
                if (_currentNode != null)
                {
                    _currentNode.LineNumber = Traceback.Instance.Line;
                }
            }
        }
        private AstNode? _currentNode;
        public readonly bool IsHandlingLine = true;
        public List<AstNode> Nodes { get; set; } = new List<AstNode>();
        private static readonly List<string> TypeTable = [];

        public AstBuilder(List<Token> tokens, bool isHandlingLine = true)
        {
            this.Tokens = tokens;
            this.IsHandlingLine = isHandlingLine;
        }

        public AstNode BuildNode()
        {
            if (Tokens == null || Tokens.Count == 0)
            {
                return null;
            }
            while (TokenIndex < Tokens.Count)
            {
                Token token = Tokens[TokenIndex];
                var handler = TokenHandlerFactory.CreateHandler(token, this);
                handler.HandleToken(token);

                if (CurrentNode != null)
                {
                    CurrentNode.LineNumber = CurrentNode.Token.Line;
                    Nodes.Add(CurrentNode);
                    CurrentNode = null;
                }
            }
            if (Nodes.Count > 1)
            {
                Nodes.WriteNodes();
                if (!IsHandlingLine)
                {
                    new BifySyntaxError("Invalid Syntax! Check your input for missing operators or delimiters.").Throw();
                }
                else
                {
                    HandleInvalidSyntax();
                }
            }
            return Nodes.Count == 1 ? Nodes[0] : null;
        }

        public Token GetPreviousToken()
        {

            return TokensFormatter.GetTokenOrNull(Tokens, TokenIndex - 1);
        }
        public Token Peek()
        {
            Token token = TokenIndex < Tokens.Count ? Tokens[TokenIndex] : null;
            Traceback.Instance.SetCurrentLine(token == null ? Traceback.Instance.Line : token.Line);
            return token;
        }

        public Token NextToken()
        {
            Token token = Tokens[TokenIndex++];
            Traceback.Instance.SetCurrentLine(token.Line);
            return token;
        }

        public bool IsAtEnd() => TokenIndex >= Tokens.Count;

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

            return compiler.VariableManager.TryGetBifyType(token.Value) != null || TypeTable.Contains(token.Value) || token.Value == "var";
        }
        public void AddType(string name)
        {
            TypeTable.Add(name);
        }
        public Token GetNextToken()
        {
            if (TokenIndex + 1 >= Tokens.Count)
            {
                return null;
            }
            Token token = Tokens[TokenIndex + 1];
            return token;
        }
        public void MoveToEnd()
        {
            TokenIndex = Tokens.Count;
        }
        public AstBlock HandleBody(string name)
        {
            if (IsAtEnd())
            {
                throw new IndexOutOfRangeException("While handling the body, the token index goes out of range!");
            }
            List<Token> bodyTokens = null;
            if (GetNextToken()?.Type == TokenType.LCUR)
            {
                bodyTokens = GetBlockTokens();
            }
            if (bodyTokens == null)
            {
                new BifySyntaxError($"{name} body not found.").Throw();
            }

            AstBlock blockNode = ParseBlock(bodyTokens);
            TokenIndex++;
            return blockNode;
        }

        public List<Token> GetConditionTokens() => TokensFormatter.GetTokensBetween(Tokens, ref TokenIndex, TokenType.LPAREN, TokenType.RPAREN);

        public List<Token> GetBlockTokens() => TokensFormatter.GetTokensBetween(Tokens, ref TokenIndex, TokenType.LCUR, TokenType.RCUR);

        public AstNode ParseTokens(List<Token> conditionTokens) => new AstBuilder(conditionTokens, false).BuildNode();

        public AstBlock ParseBlock(List<Token> blockTokens) => new AstBlock(((AstModule)new AstTree(Traceback.Instance.Source).ParseTokens(blockTokens)).ChildNodes);
        private void HandleInvalidSyntax()
        {
            if (Nodes.Count == 2)
            {
                Nodes.WriteNodes();
                if (Nodes[0].Token.Type == TokenType.POINTER || Nodes[0] is AstIdentifier)
                {
                    new BifySyntaxError($"Invalid variable declaration syntax. '{Nodes[0].Token.Value}' is not a type").Throw();
                }
                new BifySyntaxError("Opa 2 node").Throw();
            }
            new BifySyntaxError("Invalid Syntax! Check your input for missing operators or delimiters.").Throw();
        }
    }
}
