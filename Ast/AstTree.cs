using System;
using System.Collections.Generic;
using System.IO;
using BoomifyCS.Exceptions;
using BoomifyCS.Lexer;
using BoomifyCS.Parser;
using BoomifyCS.Parser.NodeParser;

namespace BoomifyCS.Ast
{
    public class AstTree
    {
        private int _codeTokenPosition = 0;
        public int lineCount = 1;
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
            _codeTokenPosition = newTokenPosition;
            AstNode node = BuildAstTree(lineTokens);
            nodes.Add(new AstLine(node));
        }

        private AstModule GenerateFinalAstNode(List<AstNode> nodes)
        {
            if (nodes.Count == 1)
            {
                return new AstModule(new Token(TokenType.IDENTIFIER, _moduleName), _moduleName, modulePath, nodes[0]);
            }

            while (nodes.Count > 1)
            {
                AstNode right = nodes.Pop();
                AstNode left = nodes.Pop();

                AstNode combinedNode = new(new Token(TokenType.NEXTLINE, "Parent"), left, right);

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
            if (currentToken.Type == TokenType.WHITESPACE || currentToken.Type == TokenType.NEXTLINE || currentToken.Type == TokenType.EOL)
            {
                if (currentToken.Type == TokenType.NEXTLINE || currentToken.Type == TokenType.EOL)
                {
                    lineCount += 1;
                }
                lineTokenPosition++;

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
            else if (currentToken.Type == TokenType.NOT)
            {
                try
                {
                    operandStack.Push(
                    new AstBinaryOp(currentToken, NodeConverter.TokenToNode(
                        tokens[lineTokenPosition + 1], this
                    ))
                    );
                }
                catch (ArgumentOutOfRangeException e)
                {
                    throw new BifySyntaxError(ErrorMessage.NotEnoughOperands(currentToken.Value), sourceCode[lineCount - 1],currentToken.Value);
                }
                
                lineTokenPosition++;
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



        private void HandleMultiTokenStatements(List<Token> tokens, Stack<AstNode> operandStack, Stack<AstNode> operatorStack, ref int lineTokenPosition, Token currentToken)
        {
            (AstNode, int) result;

            try
            {
                result = MultiTokenHandler.ProcessMultiTokenStatement(currentToken, tokens, lineTokenPosition, this);
            }
            catch (BifyError e)
            {
                e.CurrentLine = lineCount;
                e.LineTokensString = sourceCode[e.CurrentLine - 1];
                throw;
            }

            lineTokenPosition = result.Item2;
            if (result.Item1.LineNumber == 0)
            {
                result.Item1.LineNumber = lineCount;
            }
            else
            {
            }
            if (HandleSpecialMultiTokenCases(result.Item1, operatorStack))
            {

                return;
            }
            else if (result.Item1 is AstCall || result.Item1 is AstIdentifier || result.Item1 is AstArray)
            {
                operandStack.Push(result.Item1);
                return;
            }
            else if (result.Item1 is AstUnaryOperator)
            {
                operandStack.Push(result.Item1);
                return;
            }
            else if (result.Item1 is AstAssignmentOperator)
            {
                operandStack.Pop();
                operatorStack.Push(result.Item1);

                return;
            }
            else if (result.Item1 is AstIf astIf)
            {
                result.Item1.LineNumber -= astIf.BlockNode.LineNumber - 1;
            }
            else if (result.Item1 is AstIndexOperator astIndexOperator)
            {
                astIndexOperator.OperandNode = operandStack.Pop();
                operandStack.Push(result.Item1);
                return;
            }


            operatorStack.Push(result.Item1);

        }

        private bool HandleSpecialMultiTokenCases(AstNode node, Stack<AstNode> operatorStack)
        {
            if (node is AstElse astElse)
            {
                astElse.LineNumber -= astElse.BlockNode.LineNumber - 1;
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
                    throw new BifySyntaxError(ErrorMessage.ElseWithoutMatchingIf(), sourceCode[node.LineNumber], sourceCode[node.LineNumber], node.LineNumber);
                }
            }
            catch (InvalidOperationException)
            {
                throw new BifySyntaxError(ErrorMessage.ElseWithoutMatchingIf(), sourceCode[node.LineNumber % (sourceCode.Length - 1) - 1], sourceCode[node.LineNumber % (sourceCode.Length - 1) - 1], node.LineNumber);
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
                        throw new BifySyntaxError(ErrorMessage.ElseIfCannotFollowElseDirectly(), sourceCode[astElseIf.LineNumber % sourceCode.Length], sourceCode[astElseIf.LineNumber % sourceCode.Length], astElseIf.LineNumber);
                    }
                    astIf.SetElseIfNode(astElseIf);
                }
                else
                {
                    throw new BifySyntaxError(ErrorMessage.ElseIfWithoutMatchingIf(), sourceCode[astElseIf.LineNumber - 1], sourceCode[astElseIf.LineNumber - 1], astElseIf.LineNumber);
                }
            }
            catch (InvalidOperationException)
            {
                throw new BifySyntaxError(ErrorMessage.ElseIfWithoutMatchingIf(), sourceCode[astElseIf.LineNumber - 1], sourceCode[astElseIf.LineNumber - 1], astElseIf.LineNumber);
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

                throw new BifySyntaxError(ErrorMessage.NotEnoughOperands(op.Token.Value), sourceCode[op.LineNumber - 1], op.Token.Value);
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
                op.LineNumber = lineCount;
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
                op.LineNumber = lineCount;
                operandStack.Push(op);
            }

            if (currentToken.Type != TokenType.EOL)
            {
                operatorStack.Push(NodeConverter.TokenToNode(currentToken, this));
            }
        }
    }
}
