using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomifyCS.Lexer;
using BoomifyCS.Parser;

namespace BoomifyCS.Ast
{
    public class AstTree
    {
        private int _codeTokenPosition = 0;
        private int _lineTokenPosition = 0;
        public int lineCount = 1;
        private List<Token> _codeTokens = new List<Token>();
        private List<Token> _lineTokens = new List<Token>();
        public AstTree() { 
                            
        }
        /// <summary>
        /// Parses a list of tokens, processing them line by line.
        /// </summary>
        /// <param name="tokens">The list of tokens to be parsed.</param>
        public AstNode ParseTokens(List<Token> tokens)
        {
            _codeTokens = tokens;
            List<AstNode> nodes = new List<AstNode>();
            while (_codeTokenPosition < tokens.Count)
            {
                var (lineTokens, newTokenPosition) = TokensParser.SplitTokensByLine(tokens, _codeTokenPosition);
                _codeTokenPosition = newTokenPosition;
                _lineTokens = lineTokens;
                AstNode node = BuildAstTree(lineTokens);
                nodes.Add(node);
                //Console.WriteLine(AstParser.SimpleEval(node));
            }
            while (nodes.Count > 1)
            {
                AstNode right = nodes.Pop();
                AstNode left = nodes.Pop();
                AstLine line = new AstLine(left, right);
                nodes.Add(line);
            }
            return nodes[0];
        }
            /// <summary>
            /// Builds an Abstract Syntax Tree (AST) from a list of tokens.
            /// </summary>
            /// <param name="tokens">The list of tokens to build the AST from.</param>
            /// <returns>The root node of the constructed AST.</returns>
            public AstNode BuildAstTree(List<Token> tokens)
        {
            
            List<AstNode> operandStack = new List<AstNode>();
            List<AstNode> operatorStack = new List<AstNode>();
            _lineTokenPosition = 0;

            while (_lineTokenPosition < tokens.Count)
            {
                Token currentToken = tokens[_lineTokenPosition];
                if (currentToken.Type == TokenType.WHITESPACE)
                {
                    _lineTokenPosition++;
                    continue;
                }
                if (TokenConfig.multiTokenStatements.ContainsValue(currentToken.Type))
                {
                    Tuple<AstNode, int> result = AstParser.MultiTokenStatement(currentToken, tokens, _lineTokenPosition);
                    _lineTokenPosition = result.Item2;
                    operandStack.Add(result.Item1);

                }
                else if (TokensParser.IsOperator(currentToken.Type))
                {
                    operatorStack.Add(AstParser.TokenToNode(currentToken));
                }
                else
                {
                    operandStack.Add(AstParser.TokenToNode(currentToken));
                }
                _lineTokenPosition++;
            }
            if (operandStack.Count > 1)
            {
                return AstParser.ConnectNodes(operatorStack, operandStack);

            }
            else
            {
                return operandStack[0];
            }

        }



    }
}
