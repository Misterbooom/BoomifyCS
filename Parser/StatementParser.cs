using System;
using System.Collections.Generic;
using BoomifyCS.Ast;
using BoomifyCS.Lexer;
using BoomifyCS.Objects;
using BoomifyCS.Exceptions;

namespace BoomifyCS.Parser
{
    public static class StatementParser
    {
        public static (AstNode, int) ParseVarDecl(Token token, List<Token> tokens, int currentPos, AstTree astParser)
        {
            try
            {
                var (varNameToken, varNameIndex) = FindTokenSafe(TokenType.IDENTIFIER, tokens, currentPos);
                var (assignmentToken, assignmentIndex) = FindTokenSafe(TokenType.ASSIGN, tokens, currentPos);

                currentPos = assignmentIndex + 1;
                AstAssignment assignmentNode = new(assignmentToken);

                var (varValueTokens, tokensProcessed) = TokensParser.AllTokensToEol(tokens, currentPos);
                currentPos += tokensProcessed;
                AstNode varNameNode = new AstVar(varNameToken, new BifyVar(varNameToken.Value));
                AstNode varValueNode = astParser.BuildAstTree(varValueTokens);
                assignmentNode.Left = varNameNode;
                assignmentNode.Right = varValueNode;

                AstVarDecl astVarDecl = new(token, assignmentNode);
                return (astVarDecl, currentPos);
            }
            catch (NullReferenceException)
            {
                throw new BifySyntaxError("Failed to parse variable declaration", tokens, [token], 0);
            }
        }

        public static (AstNode, int) ParseIf(Token token, List<Token> tokens, int currentPos, AstTree astParser)
        {
            try
            {
                var (conditionTokens, conditionEnd) = FindTokensInBracketsSafe(tokens, currentPos);
                currentPos = conditionEnd + 1;
                AstNode conditionNode = astParser.BuildAstTree(conditionTokens);

                var (blockToken, blockEnd) = FindTokenSafe(TokenType.OBJECT, tokens, currentPos);
                currentPos = blockEnd;
                List<Token> blockTokens = [blockToken];
                AstNode blockNode = astParser.BuildAstTree(blockTokens);

                AstIf astIf = new(token, conditionNode, (AstBlock)blockNode);
                return (astIf, currentPos);
            }
            catch (NullReferenceException)
            {
                throw new BifySyntaxError("Failed to parse if statement", tokens, [token], 0);
            }
        }

        public static (AstNode, int) ParseElse(Token token, List<Token> tokens, int currentPos, AstTree astParser)
        {
            try
            {
                if (tokens[currentPos + 2].Type == TokenType.IF)
                {
                    var (conditionTokens, conditionEnd) = TokensParser.TokensInBrackets(tokens, currentPos);
                    currentPos = conditionEnd + 1;
                    AstNode conditionNode = astParser.BuildAstTree(conditionTokens);

                    var (blockToken, blockEnd) = FindTokenSafe(TokenType.OBJECT, tokens, currentPos);
                    currentPos = blockEnd;
                    List<Token> blockTokens = [blockToken];
                    AstNode blockNode = astParser.BuildAstTree(blockTokens);

                    AstElseIf astElseIf = new(token, (AstBlock)blockNode, conditionNode);
                    return (astElseIf, currentPos);
                }
                else
                {
                    var (blockToken, blockEnd) = FindTokenSafe(TokenType.OBJECT, tokens, currentPos);
                    currentPos = blockEnd;
                    List<Token> blockTokens = [blockToken];
                    AstNode blockNode = astParser.BuildAstTree(blockTokens);
                    AstElse astElse = new(token, (AstBlock)blockNode);
                    return (astElse, currentPos);
                }
            }
            catch (NullReferenceException)
            {
                throw new BifySyntaxError("Failed to parse else statement", tokens, [token], currentPos);
            }
        }

        public static (AstNode, int) ParseElseIf(Token token, List<Token> tokens, int currentPos, AstTree astParser)
        {
            try
            {
                var (conditionTokens, conditionEnd) = FindTokensInBracketsSafe(tokens, currentPos);
                currentPos = conditionEnd + 1;
                AstNode conditionNode = astParser.BuildAstTree(conditionTokens);

                var (blockToken, blockEnd) = FindTokenSafe(TokenType.OBJECT, tokens, currentPos);
                currentPos = blockEnd;
                List<Token> blockTokens = [blockToken];
                AstNode blockNode = astParser.BuildAstTree(blockTokens);

                AstElseIf astElseIf = new(token, (AstBlock)blockNode, conditionNode);
                return (astElseIf, currentPos);
            }
            catch (NullReferenceException)
            {
                throw new BifySyntaxError("Failed to parse else-if statement", tokens, [token], 0);
            }
        }
        public static (AstNode, int) ParseWhile(Token token, List<Token> tokens, int currentPos, AstTree astParser)
        {
            try
            {
                var (conditionTokens, conditionEnd) = FindTokensInBracketsSafe(tokens, currentPos);
                currentPos = conditionEnd + 1;
                AstNode conditionNode = astParser.BuildAstTree(conditionTokens);

                var (blockToken, blockEnd) = FindTokenSafe(TokenType.OBJECT, tokens, currentPos);
                currentPos = blockEnd;
                List<Token> blockTokens = [blockToken];
                AstNode blockNode = astParser.BuildAstTree(blockTokens);

                AstWhile astWhile = new(token, (AstBlock)blockNode, conditionNode);
                return (astWhile, currentPos);
            }
            catch (NullReferenceException)
            {
                throw new BifySyntaxError("Failed to parse while statement", tokens, [token], 0);
            }
        }
        public static (AstNode, int) ParseFor(Token token, List<Token> tokens, int currentPos, AstTree astParser)
        {
            var (tokensInBrackets, bracketEnd) = FindTokensInBracketsSafe(tokens, currentPos);
            currentPos = bracketEnd + 1;
            List<List<Token>> bracketTokensSplited = TokensParser.SplitTokensByTT(tokensInBrackets, TokenType.SEMICOLON);
            bracketTokensSplited[0].Add(new Token(TokenType.EOL, ";"));
            AstNode initNode = astParser.BuildAstTree(bracketTokensSplited[0]);
            AstNode conditionNode = astParser.BuildAstTree(bracketTokensSplited[1]);
            AstNode incrementNode = astParser.BuildAstTree(bracketTokensSplited[2]);

            var (blockToken, blockTokenEnd) = FindTokenSafe(TokenType.OBJECT, tokens, currentPos);
            currentPos += blockTokenEnd;
            AstNode blockNode = astParser.BuildAstTree([blockToken]);
            return (new AstFor(token, (AstBlock)blockNode, conditionNode, incrementNode, initNode), currentPos);
        }
        public static (AstNode, int) ParseUnaryOp(Token token, List<Token> tokens, int currentPos, AstTree astParser)
        {
            var (identifierToken, tokenEnd) = FindTokenSafe(TokenType.IDENTIFIER, tokens, currentPos - 1);
            currentPos += tokenEnd + 1;
            if (token.Type == TokenType.INCREMENT)
            {
                AstUnaryOperator unaryOperator = new(token, astParser.BuildAstTree([identifierToken]), 1);
                return (unaryOperator, currentPos);


            }
            else
            {
                AstUnaryOperator unaryOperator = new(token, astParser.BuildAstTree([identifierToken]), -1);
                return (unaryOperator, currentPos);


            }
        }
        public static (AstNode, int) ParseFunctionDecl(Token token, List<Token> tokens, int currentPos, AstTree astParser)
        {
            var (identifierToken, tokenEnd) = FindTokenSafe(TokenType.IDENTIFIER, tokens, currentPos);
            currentPos = tokenEnd + 1;
            var (argumentsTokens, argumentsEnd) = FindTokensInBracketsSafe(tokens, currentPos);
            AstNode identifierNode = astParser.BuildAstTree([identifierToken]);
            currentPos = argumentsEnd + 1;
            AstNode argumentsNode = astParser.BuildAstTree(argumentsTokens);
            var (blockToken, blockEnd) = FindTokenSafe(TokenType.OBJECT, tokens, currentPos);
            currentPos = blockEnd;
            AstNode blockNode = astParser.BuildAstTree([blockToken]);
            AstFunctionDecl astFunctionDecl = new(token, (AstIdentifier)identifierNode, argumentsNode, (AstBlock)blockNode);
            return (astFunctionDecl, currentPos);


        }
        public static (AstNode, int) ParseIdentifier(Token token, List<Token> tokens, int currentPos, AstTree astParser)
        {
            try
            {
                var (argumentsTokens, argumentsEnd) = FindTokensInBracketsSafe(tokens, currentPos);
                currentPos = argumentsEnd + 1;
                AstNode identifierNode = astParser.BuildAstTree([token]);
                AstNode argumentsNode = astParser.BuildAstTree(argumentsTokens);
                Token callToken = new(TokenType.CALL, token.Value + "(" + argumentsTokens.ToCustomString() + ")");
                return (new AstCall(callToken, identifierNode, argumentsNode), currentPos);
            }

            catch (BifyParsingError)
            {
                return (NodeConverter.TokenToNode(token), currentPos);
            }
        }
        public static (AstNode, int) ParseAssignmentOperator(Token token, List<Token> tokens, int currentPos, AstTree astParser)
        {
            if (currentPos - 1 < 0)
            {
                throw new BifySyntaxError($"Expected identifier token", tokens, tokens, currentPos - 1);

            }
            Token identifierToken = tokens[currentPos - 1];
            if (identifierToken.Type != TokenType.IDENTIFIER)
            {
                throw new BifySyntaxError($"Expected identifier token but found {identifierToken.Type}", tokens, tokens, currentPos - 1);
            }
            AstNode identifierNode = NodeConverter.TokenToNode(identifierToken);
            var (valueTokens,endOfValue) = TokensParser.AllTokensToEol(tokens,currentPos + 1);
            if (valueTokens.Count == 0)
            {
                throw new BifySyntaxError("Expected value after assignment operator", tokens, tokens, currentPos);
            }
            currentPos = endOfValue + 1;
            AstNode valueNode = astParser.BuildAstTree(valueTokens);
            return (new AstAssignmentOperator(token,(AstIdentifier)identifierNode,valueNode),currentPos);

        }
        private static (Token, int) FindTokenSafe(TokenType tokenType, List<Token> tokens, int currentPos)
        {
            try
            {
                var (token, index) = TokensParser.FindTokenByTT(tokenType, tokens, currentPos);
                if (token == null)
                {
                    throw new NullReferenceException();
                }
                return (token, index);
            }
            catch (NullReferenceException)
            {
                throw new BifySyntaxError($"Expected token of type {tokenType} not found", tokens, tokens, 0);
            }
        }

        private static (List<Token>, int) FindTokensInBracketsSafe(List<Token> tokens, int currentPos)
        {
            try
            {
                var (conditionTokens, conditionEnd) = TokensParser.TokensInBrackets(tokens, currentPos);
                if (conditionTokens == null || conditionTokens.Count == 0)
                {
                    throw new NullReferenceException("Can not find token in brackets");
                }
                return (conditionTokens, conditionEnd);
            }
            catch (NullReferenceException)
            {
                throw new BifyParsingError("Tokens in brackets not found", tokens, tokens, 0);
            }
        }
        
        
    }
}
