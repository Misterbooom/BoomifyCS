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
        public void ParseTokens(List<Token> tokens)
        {
            _codeTokens = tokens;

            while (_codeTokenPosition < tokens.Count)
            {
                var (lineTokens, newTokenPosition) = TokensParser.SplitTokensByLine(tokens, _codeTokenPosition);

                _codeTokenPosition = newTokenPosition;
                _lineTokens = lineTokens;
                AstNode node = BuildAstTree(lineTokens);
                Console.WriteLine(node.ToString());
            
                //Console.WriteLine(AstParser.SimpleEval(node));
            }
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
                if (TokensParser.IsOperator(currentToken.Type))
                {
                    operatorStack.Add(AstParser.TokenToNode(currentToken));
                }
                else
                {
                    operandStack.Add(AstParser.TokenToNode(currentToken));
                }
                Console.WriteLine(currentToken);
                _lineTokenPosition++;
            }
            return AstParser.ConnectNodes(operatorStack, operandStack);
            
        }



    }
}
