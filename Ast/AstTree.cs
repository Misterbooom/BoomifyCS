using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using BoomifyCS.Lexer;
using BoomifyCS.Parser;

namespace BoomifyCS.Ast
{
    public class AstTree
    {
        private int _codeTokenPosition = 0;
        public int lineCount = 0;
        public string runFrom = "";
        private List<Token> _codeTokens = new List<Token>();
        private List<Token> _lineTokens = new List<Token>();
        private string[] _sourceCode;
        public AstTree(string[] sourcecode = null) {
            _sourceCode = sourcecode ?? new string[] { "1" };

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
                //if (runFrom == "main")
                //{
                //    Console.WriteLine(lineCount);
                //    lineTokens.WriteTokens();

                //}
                //lineTokens.WriteTokens();
                AstNode node = BuildAstTree(lineTokens);
                nodes.Add(node);
                //Console.WriteLine(AstParser.SimpleEval(node));
            }
            if (nodes.Count == 1)
            {
                return new AstLine(nodes[0]);
            }
            while (nodes.Count > 1)
            {
                AstNode right = nodes.Pop();
                AstNode left = nodes.Pop();
                AstLine line = new AstLine(left, right);
                nodes.Add(line);
            }
            if (nodes.Count > 0)
            {
                return nodes[0];

            }
            return new AstEOL(new Token(TokenType.EOL,""));

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
            int lineTokenPosition = 0;

            while (lineTokenPosition < tokens.Count)
            {
                Token currentToken = tokens[lineTokenPosition];

                if (currentToken.Type == TokenType.WHITESPACE)
                {
                    lineTokenPosition++;
                    continue;
                }
                else if (currentToken.Type  == TokenType.NEXTLINE)
                {
                    lineTokenPosition++;
                    lineCount++;

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
                        (AstNode, int) result = NodeParser.MultiTokenStatement(currentToken, tokens, lineTokenPosition,this);
                        lineTokenPosition = result.Item2;
                        if (result.Item1 is AstElse)
                        {
                            try
                            {

                                if (operatorStack.Peek() is AstIf astIf)
                                {
                                    astIf.ElseNode = (AstElse)result.Item1;
                                    lineTokenPosition++;
                                    continue;
                                }
                                
                                else
                                {
                                    throw new BifySyntaxError($"Unexpected else, last token - {operatorStack.Peek().Token.Type}", _sourceCode[lineCount], _sourceCode[lineCount], lineCount + 1); ;
                                }
                            }
                            catch (InvalidOperationException)
                            {
                                throw new BifySyntaxError("Unexpected else ", _sourceCode[lineCount], _sourceCode[lineCount], lineCount + 1);

                            }
                        }
                        else if (result.Item1 is AstElseIf astElseIf) 
                        {
                            try
                            {
                                if (operatorStack.Peek() is AstIf astIf)
                                {
                                    if (astIf.ElseNode != null)
                                    {
                                        throw new BifySyntaxError($"Else-if used after else statement", tokens, tokens, lineCount);

                                    }
                                    astIf.SetElseIfNode(astElseIf);
                                    lineTokenPosition++;
                                    continue;
                                }
                                else
                                {
                                    throw new BifySyntaxError($"Unexpected else-if, last token - {operatorStack.Peek().Token.Type}", tokens, tokens, lineCount);
                                }

                            }
                            catch (InvalidOperationException)
                            {
                                throw new BifySyntaxError("Unexpected else if ", tokens, new List<Token> { currentToken }, lineCount);
                            }
                        }
                        else if (result.Item1 is AstCall || result.Item1 is AstIdentifier)
                        {
                            operandStack.Push(result.Item1);
                            lineTokenPosition++;
                            continue;
                        }
                        
                        else if (result.Item1 is AstUnaryOperator)
                        {
                            operandStack.Pop();
                            lineTokenPosition++;
                            continue;
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
                        AstNode op = operatorStack.Pop();
                        if (operandStack.Count < 2)
                        {
                            throw new BifyParsingError( $"Not enough operands for operator - '{op.Token.Value}'", _sourceCode[lineCount],op.Token.Value,lineCount);
                        }
                        AstNode right = operandStack.Pop();
                        AstNode left = operandStack.Pop();
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

                lineTokenPosition++;
            }
            if (operatorStack.Count == 1 && operandStack.Count == 0)
            {
                return operatorStack.Peek();
            }
            
            while (operatorStack.Count > 0)
            {
                AstNode op = operatorStack.Pop();

                if (operandStack.Count < 2)
                {
                    throw new BifyParsingError($"Not enough operands for operator - '{op.Token.Value}'", _sourceCode[lineCount], op.Token.Value, lineCount);
                }
                AstNode right = operandStack.Pop();
                AstNode left = operandStack.Pop();
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
                AstNode op = operatorStack.Pop();
                if (operandStack.Count < 2)
                {
                    throw new BifyParsingError($"Not enough operands for operator - '{op.Token.Value}'", _sourceCode[lineCount], op.Token.Value, lineCount);
                }
                AstNode right = operandStack.Pop();
                AstNode left = operandStack.Pop();
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
