using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using BoomifyCS.Lexer;
using BoomifyCS.Parser;
using BoomifyCS.Exceptions;
using System.IO;
using BoomifyCS.Parser.NodeParser;

namespace BoomifyCS.Ast
{
    public class AstTree
    {
        private int _codeTokenPosition = 0;
        public int lineCount = 0;
        public readonly string[] sourceCode;
        public readonly string modulePath;
        private readonly string _moduleName;

        public AstTree(string modulePath, string[] sourcecode = null)
        {
            sourceCode = sourcecode ?? [""];
            this.modulePath = modulePath;
            _moduleName = Path.GetFileName(this.modulePath);
        }

        public AstNode ParseTokens(List<Token> tokens)
        {
            List<AstNode> nodes = [];

            while (_codeTokenPosition < tokens.Count)
            {
                
                ParseSingleLine(tokens, nodes);
                
            }

            return GenerateFinalAstNode(nodes);
        }

        private void ParseSingleLine(List<Token> tokens, List<AstNode> nodes)
        {
            var (lineTokens, newTokenPosition) = TokensParser.SplitTokensByLine(tokens, _codeTokenPosition);
            lineTokens.WriteTokens();

            _codeTokenPosition = newTokenPosition;
            AstNode node = BuildAstTree(lineTokens);
            nodes.Add(new AstLine(node));
        }

        private AstNode GenerateFinalAstNode(List<AstNode> nodes)
        {
            if (nodes.Count == 1)
            {
                return new AstModule(new Token(TokenType.IDENTIFIER, _moduleName), _moduleName, modulePath, new AstLine(nodes[0]));
            }

            while (nodes.Count > 1)
            {
                AstNode right = nodes.Pop();
                AstNode left = nodes.Pop();

                AstNode combinedNode = new AstNode(new Token(TokenType.NEXTLINE,"Parent"),left, right); 

                nodes.Add(combinedNode);
            }


            if (nodes.Count > 0)
            {
                return new AstModule(new Token(TokenType.IDENTIFIER, _moduleName), _moduleName, modulePath, nodes[0]);
            }

            return new AstModule(new Token(TokenType.IDENTIFIER, _moduleName), _moduleName, modulePath, new AstEOL(new Token(TokenType.EOL, "")));
        }

        public AstNode BuildAstTree(List<Token> tokens)
        {
            Stack<AstNode> operandStack = new();
            Stack<AstNode> operatorStack = new();
            int lineTokenPosition = 0;

            while (lineTokenPosition < tokens.Count)
            {
                ProcessSingleToken(tokens, operandStack, operatorStack, ref lineTokenPosition);
            }

            return GenerateAstNodeFromStacks(operandStack, operatorStack);
        }

        private void ProcessSingleToken(List<Token> tokens, Stack<AstNode> operandStack, Stack<AstNode> operatorStack, ref int lineTokenPosition)
        {
            Token currentToken = tokens[lineTokenPosition];

            if (currentToken.Type == TokenType.WHITESPACE || currentToken.Type == TokenType.NEXTLINE)
            {
                SkipWhitespaceOrNewline(ref lineTokenPosition, currentToken);
                return;
            }

            if (currentToken.Type == TokenType.EOL)
            {
                return;
            }

            if (TokenConfig.multiTokenStatements.ContainsValue(currentToken.Type))
            {
                HandleMultiTokenStatements(tokens, operandStack, operatorStack, ref lineTokenPosition, currentToken);
            }
            else if (TokensParser.IsOperator(currentToken.Type))
            {
                ProcessOperator(operatorStack, operandStack, currentToken);
            }
            else if (currentToken.Type == TokenType.LPAREN)
            {
                operatorStack.Push(NodeConverter.TokenToNode(currentToken, this));
            }
            else if (currentToken.Type == TokenType.RPAREN)
            {
                HandleClosingParenthesis(operatorStack, operandStack);
            }
            else
            {
                operandStack.Push(NodeConverter.TokenToNode(currentToken, this));
            }

            lineTokenPosition++;
        }

        private void SkipWhitespaceOrNewline(ref int lineTokenPosition, Token currentToken)
        {
            lineTokenPosition++;
            if (currentToken.Type == TokenType.NEXTLINE)
            {
                lineCount++;
            }
        }

        private void HandleMultiTokenStatements(List<Token> tokens, Stack<AstNode> operandStack, Stack<AstNode> operatorStack, ref int lineTokenPosition, Token currentToken)
        {
            try
            {
                var result = MultiTokenHandler.ProcessMultiTokenStatement(currentToken, tokens, lineTokenPosition, this);
                lineTokenPosition = result.Item2;

                if (HandleSpecialMultiTokenCases(result.Item1, operatorStack))
                {
                    lineTokenPosition++;
                    return;
                }

                else if (result.Item1 is AstCall || result.Item1 is AstIdentifier)
                {
                    operandStack.Push(result.Item1);
                    return;
                }
                else if (result.Item1 is AstUnaryOperator)
                {
                    operandStack.Pop();
                    operandStack.Push(result.Item1);
                    return;
                }
                else if (result.Item1 is AstAssignmentOperator)
                {
                    operandStack.Pop();
                    operatorStack.Push(result.Item1);

                    return;
                }

                operatorStack.Push(result.Item1);
            }
            catch (BifyError e)
            {
                e.CurrentLine = lineCount;
                throw e;
            }
        }

        private bool HandleSpecialMultiTokenCases(AstNode node, Stack<AstNode> operatorStack)
        {
            if (node is AstElse)
            {
                HandleElseStatement(operatorStack, node);
                return true;
            }

            if (node is AstElseIf astElseIf)
            {
                HandleElseIfStatement(operatorStack, astElseIf);
                return true;
            }

            return false;
        }

        private void HandleElseStatement(Stack<AstNode> operatorStack, AstNode node)
        {
            try
            {
                if (operatorStack.Peek() is AstIf astIf)
                {
                    astIf.ElseNode = (AstElse)node;
                }
                else
                {
                    throw new BifySyntaxError($"Unexpected else, last token - {operatorStack.Peek().Token.Type}", sourceCode[lineCount], sourceCode[lineCount], lineCount + 1);
                }
            }
            catch (InvalidOperationException)
            {
                throw new BifySyntaxError("Unexpected else", sourceCode[lineCount], sourceCode[lineCount], lineCount + 1);
            }
        }

        private void HandleElseIfStatement(Stack<AstNode> operatorStack, AstElseIf astElseIf)
        {
            try
            {
                if (operatorStack.Peek() is AstIf astIf)
                {
                    if (astIf.ElseNode != null)
                    {
                        throw new BifySyntaxError("Else-if used after else statement", sourceCode[lineCount - 1], sourceCode[lineCount - 1], lineCount);
                    }
                    astIf.SetElseIfNode(astElseIf);
                }
                else
                {
                    throw new BifySyntaxError($"Unexpected else-if, last token - {operatorStack.Peek().Token.Type}", sourceCode[lineCount - 1],sourceCode[lineCount - 1], lineCount);
                }
            }
            catch (InvalidOperationException)
            {
                throw new BifySyntaxError("Unexpected else if", sourceCode[lineCount - 1], sourceCode[lineCount - 1], lineCount);
            }
        }

        private void HandleClosingParenthesis(Stack<AstNode> operatorStack, Stack<AstNode> operandStack)
        {
            while (operatorStack.Count > 0 && operatorStack.Peek().Token.Type != TokenType.LPAREN)
            {
                AstNode op = operatorStack.Pop();
                EnsureEnoughOperands(operandStack, op);
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

        private void EnsureEnoughOperands(Stack<AstNode> operandStack, AstNode op)
        {
            if (operandStack.Count < 2)
            {
                throw new BifySyntaxError($"Not enough operands for operator - '{op.Token.Value}'");
            }
        }

        private AstNode GenerateAstNodeFromStacks(Stack<AstNode> operandStack, Stack<AstNode> operatorStack)
        {
            if (operatorStack.Count == 1 && operandStack.Count == 0)
            {
                return operatorStack.Peek();
            }
            while (operatorStack.Count > 0)
            {
                AstNode op = operatorStack.Pop();
                EnsureEnoughOperands(operandStack, op);
                AstNode right = operandStack.Pop();
                AstNode left = operandStack.Pop();
                op.Left = left;
                op.Right = right;
                operandStack.Push(op);
            }

            return operandStack.Count == 1 ? operandStack.Pop() : null;
        }

        private void ProcessOperator(Stack<AstNode> operatorStack, Stack<AstNode> operandStack, Token currentToken)
        {
            while (operatorStack.Count > 0 &&
                   operatorStack.Peek().Token.Type != TokenType.LPAREN &&
                   AstConfig.Precedence.TryGetValue(operatorStack.Peek().Token.Type, out int operatorPrecedence) &&
                   AstConfig.Precedence.TryGetValue(currentToken.Type, out int currentOperatorPrecedence) &&
                   operatorPrecedence >= currentOperatorPrecedence)
            {
                AstNode op = operatorStack.Pop();
                EnsureEnoughOperands(operandStack, op);
                AstNode right = operandStack.Pop();
                AstNode left = operandStack.Pop();
                op.Left = left;
                op.Right = right;
                operandStack.Push(op);
            }

            if (currentToken.Type != TokenType.EOL)
            {
                operatorStack.Push(NodeConverter.TokenToNode(currentToken, this));
            }
        }
    }
}
