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
        public string runFrom = "";
        private List<Token> _codeTokens = new List<Token>();
        private List<Token> _lineTokens = new List<Token>();
        public AstTree(int lineCount = 1) {
            this.lineCount = lineCount; 

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
                lineCount += 1;
                if (runFrom == "main")
                {
                    Console.WriteLine(lineCount);
                    lineTokens.WriteTokens();

                }
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
            Stack<AstNode> operandStack = new Stack<AstNode>();
            Stack<AstNode> operatorStack = new Stack<AstNode>();
            _lineTokenPosition = 0;

            while (_lineTokenPosition < tokens.Count)
            {
                Token currentToken = tokens[_lineTokenPosition];

                if (currentToken.Type == TokenType.WHITESPACE)
                {
                    _lineTokenPosition++;
                    continue;
                }
                else if (currentToken.Type == TokenType.EOL)
                {
                    break;
                }
                else if (TokenConfig.multiTokenStatements.ContainsValue(currentToken.Type))
                {
                    try
                    {
                        Tuple<AstNode, int> result = NodeParser.MultiTokenStatement(currentToken, tokens, _lineTokenPosition);
                        _lineTokenPosition = result.Item2;
                        if (result.Item1 is AstElse)
                        {
                            try
                            {
                                if (operatorStack.Peek() is AstIf astIf)
                                {
                                    astIf.ElseNode = (AstElse)result.Item1;
                                    _lineTokenPosition++;
                                    continue;
                                }
                                else
                                {
                                    throw new BifySyntaxError("Unexpected else ", tokens, tokens, lineCount);
                                }
                            }
                            catch (InvalidOperationException e)
                            {
                                throw new BifySyntaxError("Unexpected else ", tokens, new List<Token> { currentToken }, lineCount);

                            }
                        }
                        else if (result.Item1 is AstElseIf)
                        {
                            Console.WriteLine(result.Item1);
                        }
                        
                        operatorStack.Push(result.Item1);

                    }
                    catch (BifyException e)
                    {
                        e.CurrentLine = lineCount;
                        throw e;
                    }

                }
                else if (TokensParser.IsOperator(currentToken.Type))
                {
                    ProcessOperator(operatorStack, operandStack, currentToken);
                }
                else if (currentToken.Type == TokenType.LPAREN)
                {
                    operatorStack.Push(NodeParser.TokenToNode(currentToken));
                }
                else if (currentToken.Type == TokenType.RPAREN)
                {
                    while (operatorStack.Count > 0 && operatorStack.Peek().Token.Type != TokenType.LPAREN)
                    {
                        AstNode right = operandStack.Pop();
                        AstNode left = operandStack.Pop();
                        AstNode op = operatorStack.Pop();
                        op.Left = left;
                        op.Right = right;
                        operandStack.Push(op);
                    }

                    if (operatorStack.Count > 0 && operatorStack.Peek().Token.Type == TokenType.LPAREN)
                    {
                        operatorStack.Pop();
                    }
                }
                else
                {
                    operandStack.Push(NodeParser.TokenToNode(currentToken));
                }

                _lineTokenPosition++;
            }
            if (operatorStack.Count == 1 && operandStack.Count == 0)
            {
                return operatorStack.Peek();
            }
            while (operatorStack.Count > 0)
            {
                
                AstNode right = operandStack.Pop();
                AstNode left = operandStack.Pop();
                AstNode op = operatorStack.Pop();
                op.Left = left;
                op.Right = right;
                operandStack.Push(op);
            }

            if (operandStack.Count > 1)
            {
                return NodeParser.ConnectNodes(operatorStack, operandStack);
            }
            else if (operandStack.Count == 1)
            {
                return operandStack.Pop();
            }
            return null;
        }


        private void ProcessOperator(Stack<AstNode> operatorStack, Stack<AstNode> operandStack, Token currentToken)
        {
            while (operatorStack.Count > 0 &&
                   operatorStack.Peek().Token.Type != TokenType.LPAREN &&

                   AstConfig.Precedence.TryGetValue(operatorStack.Peek().Token.Type, out int operatorPrecedence) &&
                   AstConfig.Precedence.TryGetValue(currentToken.Type, out int currentOperatorPrecedence) &&
                   operatorPrecedence >= currentOperatorPrecedence
                  )
            {
                operatorStack.WriteNodes();
                AstNode right = operandStack.Pop();
                AstNode left = operandStack.Pop();
                AstNode op = operatorStack.Pop();
                op.Left = left;
                op.Right = right;
                operandStack.Push(op);
            }
            if (currentToken.Type != TokenType.EOL)
            {
                operatorStack.Push(NodeParser.TokenToNode(currentToken));

            }
        }


      



    }
}
