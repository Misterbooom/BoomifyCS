using System;
using System.Collections.Generic;
using BoomifyCS.Ast.Validators;
using BoomifyCS.Exceptions;
using BoomifyCS.Lexer;
using BoomifyCS.Parser;

namespace BoomifyCS.Ast
{
    class AstBuilder
    {
        int tokenIndex = 0;
        List<Token> tokens;
        private readonly Stack<AstNode> operatorStack = new();
        private readonly Stack<AstNode> operandStack = new();

        public AstBuilder(List<Token> tokens) => this.tokens = tokens;

        public AstNode BuildNode()
        {
            while (tokenIndex < tokens.Count)
            {
                HandleToken(tokens[tokenIndex]);
                tokenIndex++;
            }
            while (operatorStack.Count > 0)
            {
                PopOperator();
            }
            while (operandStack.Count > 1)
            {
                AstNode right = operandStack.Pop();
                AstNode left = operandStack.Pop();

                AstNode combinedNode = new AstBinaryOp(new Token(TokenType.ADD, "concat"));

                OperandValidator.Validate(left, right, combinedNode);

                combinedNode.Left = left;
                combinedNode.Right = right;

                AddOperand(combinedNode);
            }
            return operandStack.Count > 0 ? operandStack.Pop() : null;
        }


        private void HandleToken(Token token)
        {
            Traceback.Traceback.Instance.line = token.Line;
            if (token.Type == TokenType.VARDECL)
            {
                HandleVariableDeclaration();
            }
            else if (token.Type == TokenType.LPAREN)
            {
                HandleParenthesis();
            }
            else if (token.Type == TokenType.INCREMENT || token.Type == TokenType.DECREMENT)
            {
                HandleUnaryOperators();
            }
            else if (token.Type == TokenType.LBRACKET)
            {
                HandleBracket();
            }
            else if (TokenConfig.assignmentOperators.ContainsValue(token.Type))
            {
                HandleAssignmentOperator();
            }
            else if (TokenConfig.binaryOperators.ContainsValue(token.Type))
            {
                HandleOperator(token);
            }
            else if (token.Type == TokenType.DOT)
            {
                HandleDot();
            }
            else if (token.Type == TokenType.IDENTIFIER)
            {
                HandleIdentifier();
            }
            else if (token.Type == TokenType.IF)
            {
                HandleIf();
            }
            else if (token.Type == TokenType.ELSE)
            {
                HandleElse();
            }
            else
            {
                AddOperand(NodeConventer.TokenToNode(token));
            }

        }

        private void HandleElse()
        {
            Token nextToken = TokensFormatter.GetTokenOrNull(tokens, tokenIndex + 1);

            if (nextToken != null && nextToken.Type == TokenType.IF)
            {
                HandleElseIf();
            }
            else
            {
                HandleElseStatement();
            }
        }

        private void HandleElseIf()
        {
            Token elseToken = tokens[tokenIndex];
            int line = Traceback.Traceback.Instance.line;

            // Extract and parse tokens
            List<Token> conditionTokens = GetConditionTokens();
            List<Token> blockTokens = GetBlockTokens();
            AstNode conditionNode = ParseCondition(conditionTokens);
            AstNode blockNode = ParseBlock(blockTokens);

            // Create the AstElseIf node and validate it
            AstElseIf astElseIf = new(elseToken, blockNode, conditionNode);
            Traceback.Traceback.Instance.SetCurrentLine(line);
            ConditionStatementValidator.ValidateElseIfStatement(conditionTokens, operandStack, astElseIf);

            // Associate the AstElseIf node with the parent AstIf node
            AstIf astIf = (AstIf)operandStack.Peek();
            astIf.AddElseIfNode(astElseIf);
        }

        private void HandleElseStatement()
        {
            Token elseToken = tokens[tokenIndex];
            int line = Traceback.Traceback.Instance.line;

            // Extract and parse tokens
            List<Token> blockTokens = GetBlockTokens();
            AstNode blockNode = ParseBlock(blockTokens);

            // Create the AstElse node and validate it
            AstElse astElse = new(elseToken, blockNode);
            Traceback.Traceback.Instance.SetCurrentLine(line);
            ConditionStatementValidator.ValidateElseStatement(operandStack, astElse);

            // Associate the AstElse node with the parent AstIf node
            AstIf astIf = (AstIf)operandStack.Peek();
            astIf.ElseNode = astElse;
        }

        private void HandleIf()
        {
            // Extract and parse tokens
            List<Token> conditionTokens = GetConditionTokens();
            List<Token> blockTokens = GetBlockTokens();
            AstNode conditionNode = ParseCondition(conditionTokens);
            AstNode blockNode = ParseBlock(blockTokens);

            // Validate and create the AstIf node
            ConditionStatementValidator.ValidateIfStatement(blockTokens, conditionTokens);
            AstIf ifNode = new(conditionTokens[0], conditionNode, blockNode);
            AddOperand(ifNode);
        }

    

        private void HandleIdentifier()
        {
            Token token = TokensFormatter.GetTokenOrNull(tokens, tokenIndex + 1);
            if (token != null && token.Type == TokenType.LPAREN)
            {
                HandleCall();
            }
            AddOperand(NodeConventer.TokenToNode(tokens[tokenIndex]));
        }
        private void HandleCall()
        {
            AstIdentifier identifier = (AstIdentifier)NodeConventer.TokenToNode(tokens[tokenIndex]);
            List<Token> parenthesisTokens = TokensFormatter.GetTokensBetween(tokens, ref tokenIndex, TokenType.LPAREN, TokenType.RPAREN);
            AstNode argumentsNode = new AstBuilder(parenthesisTokens).BuildNode();
            AstCall astCall = new(identifier.Token, identifier, argumentsNode);
            AddOperand(astCall);
        }
        private void HandleDot()
        {
            Token nextToken = TokensFormatter.GetTokenOrNull(tokens, tokenIndex + 1);
            if (nextToken.Type == TokenType.DOT)
            {
                Token rangeToken = new Token(TokenType.RANGE, tokens[tokenIndex + 1].Value + tokens[tokenIndex]);
                AddOperator(new AstRangeOperator(rangeToken));
                tokenIndex++;
            }

        }
        private void HandleBracket()
        {
            List<Token> tokensInBrackets = TokensFormatter.GetTokensBetween(tokens, ref tokenIndex, TokenType.LBRACKET, TokenType.RBRACKET);
            if (operatorStack.TryPop(out AstNode operatorNode))
            {

                HandleArray(tokensInBrackets);

            }
            else if (operandStack.TryPop(out AstNode previousNode))
            {
                Console.WriteLine(operatorStack.ToString());
                AstNode indexNode = new AstBuilder(tokensInBrackets).BuildNode();
                IndexOperatorValidator.Validate(previousNode, indexNode);
                AstIndexOperator astIndexOperator = new(indexNode, previousNode);
                AddOperand(astIndexOperator);

            }

        }
        private void HandleArray(List<Token> tokensInBrackets)
        {
            AstNode valueNode = new AstBuilder(tokensInBrackets).BuildNode();
            AstArray arrayNode = new(tokens[tokenIndex - tokensInBrackets.Count], valueNode);
            AddOperand(arrayNode);
        }
        private void HandleAssignmentOperator()
        {
            Token assignToken = tokens[tokenIndex];
            Token variableToken = TokensFormatter.GetTokenOrNull(tokens, tokenIndex - 1);
            tokenIndex += 1;
            List<Token> valueTokens = tokens[tokenIndex..];
            VariableDeclarationValidator.Validate(variableToken, new Token(TokenType.ASSIGN, ""), valueTokens);
            tokenIndex = tokens.Count;
            operandStack.Pop();
            AstBuilder builder = new(valueTokens);
            AstNode valueNode = builder.BuildNode();
            AstAssignmentOperator astAssignmentOperator = new(assignToken, (AstIdentifier)NodeConventer.TokenToNode(variableToken), valueNode);
            AddOperand(astAssignmentOperator);


        }
        private void HandleUnaryOperators()
        {
            Token previousToken = TokensFormatter.GetTokenOrNull(tokens, tokenIndex - 1);
            UnaryOperatorValidator.Validate(tokens[tokenIndex].Value, previousToken);
            AstIdentifier astIdentifier = (AstIdentifier)NodeConventer.TokenToNode(previousToken);
            AstUnaryOperator unaryOperator = new(tokens[tokenIndex], astIdentifier, 1);
            operandStack.Pop();
            AddOperand(unaryOperator);

        }
        private void HandleParenthesis()
        {
            List<Token> parenthesisTokens = TokensFormatter.GetTokensBetween(tokens, ref tokenIndex, TokenType.LPAREN, TokenType.RPAREN);
            parenthesisTokens.WriteTokens();
            AstBuilder builder = new(parenthesisTokens);
            operandStack.Push(builder.BuildNode());
        }



        private void HandleVariableDeclaration()
        {
            Token variableToken = TokensFormatter.GetTokenOrNull(tokens, tokenIndex + 1);
            Token assignmentToken = TokensFormatter.GetTokenOrNull(tokens, tokenIndex + 2);
            tokenIndex += 3;
            List<Token> valueTokens = tokens[tokenIndex..];
            tokenIndex = tokens.Count;
            VariableDeclarationValidator.Validate(variableToken, assignmentToken, valueTokens);
            AstBuilder builder = new(valueTokens);
            AstNode valueNode = builder.BuildNode();
            AstIdentifier identifierNode = (AstIdentifier)NodeConventer.TokenToNode(variableToken);
            identifierNode.LineNumber = variableToken.Line;
            AstAssignment assignmentNode = new(assignmentToken, identifierNode, valueNode);
            AstVarDecl varDeclNode = new(tokens[tokenIndex - 3], assignmentNode);
            AddOperand(varDeclNode);
        }

        private void AddOperand(AstNode node)
        {
            if (node != null)
            {
                node.LineNumber = Traceback.Traceback.Instance.line;
                operandStack.Push(node);
            }

        }

        private void AddOperator(AstNode node)
        {
            node.LineNumber = Traceback.Traceback.Instance.line;
            operatorStack.Push(node);
        }

        private void HandleOperator(Token token)
        {
            while (operatorStack.Count > 0 && ShouldPopOperator(token))
            {
                PopOperator();
            }

            AstNode operatorNode = new AstBinaryOp(token);
            AddOperator(operatorNode);
        }

        private bool ShouldPopOperator(Token token)
        {
            if (operatorStack.Count == 0)
                return false;

            AstNode topOperator = operatorStack.Peek();
            int currentPrecedence = AstConfig.Precedence[token.Type];
            int topPrecedence = AstConfig.Precedence[topOperator.Token.Type];

            return currentPrecedence <= topPrecedence;
        }

        private void PopOperator()
        {
            AstBinaryOp operatorNode = (AstBinaryOp)operatorStack.Pop();
            if (operatorNode.Token.Type == TokenType.NOT)
            {
                AstNode operand = operandStack.Pop();
                OperandValidator.Validate(operand, operand, operatorNode);
                operatorNode.Left = operand;
                AddOperand(operatorNode);
                return;
            }
            else if (operandStack.Count < 2)
            {
                BifySyntaxError error = new BifySyntaxError(ErrorMessage.NotEnoughOperands(operatorNode.Token.Value), "", operatorNode.Token.Value);
                Traceback.Traceback.Instance.ThrowException(error, operatorNode.Token.Column);
            }
            AstNode right = operandStack.Pop();
            AstNode left = operandStack.Pop();
            OperandValidator.Validate(left, right, operatorNode);
            operatorNode.Left = left;
            operatorNode.Right = right;
            AddOperand(operatorNode);
        }
        private List<Token> GetConditionTokens()
        {
            return TokensFormatter.GetTokensBetween(tokens, ref tokenIndex, TokenType.LPAREN, TokenType.RPAREN);
        }

        private List<Token> GetBlockTokens()
        {
            return TokensFormatter.GetTokensBetween(tokens, ref tokenIndex, TokenType.LCUR, TokenType.RCUR);
        }

        private AstNode ParseCondition(List<Token> conditionTokens)
        {
            return new AstBuilder(conditionTokens).BuildNode();
        }

        private AstNode ParseBlock(List<Token> blockTokens)
        {
            return new AstTree(Traceback.Traceback.Instance.source).ParseTokens(blockTokens);
        }
    }


}
