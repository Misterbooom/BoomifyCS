using System;
using System.Collections.Generic;
using BoomifyCS.Ast;
using BoomifyCS.Exceptions;
using BoomifyCS.Lexer;
using BoomifyCS.Objects;
using BoomifyCS.Parser.NodeParser;

namespace BoomifyCS.Parser
{
    public static class StatementParser
    {
        public static (AstNode, int) ParseVarDecl(Token token, List<Token> tokens, int currentPos, AstTree astParser)
        {
            var (varNameToken, _) = FindTokenSafe(TokenType.IDENTIFIER, tokens, currentPos);
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

        public static (AstNode, int) ParseIf(Token token, List<Token> tokens, int currentPos, AstTree astParser)
        {
            var (conditionTokens, conditionEnd) = FindTokensInBracketsSafe(tokens, currentPos);
            currentPos = conditionEnd + 1;
            AstNode conditionNode = astParser.BuildAstTree(conditionTokens);

            var (blockToken, blockEnd) = FindTokenSafe(TokenType.BLOCK, tokens, currentPos);
            currentPos = blockEnd + 1;
            List<Token> blockTokens = [blockToken];
            AstNode blockNode = astParser.BuildAstTree(blockTokens);

            AstIf astIf = new(token, conditionNode, (AstBlock)blockNode);
            return (astIf, currentPos);
        }

        public static (AstNode, int) ParseElse(Token token, List<Token> tokens, int currentPos, AstTree astParser)
        {
            if (tokens.Count >= currentPos + 1 && tokens[currentPos + 1].Type == TokenType.IF)
            {
                int lineNumber = astParser.lineCount;

                var (conditionTokens, conditionEnd) = FindTokensInBracketsSafe(tokens, currentPos);
                currentPos = conditionEnd + 1;
                AstNode conditionNode = astParser.BuildAstTree(conditionTokens);

                var (blockToken, blockEnd) = FindTokenSafe(TokenType.BLOCK, tokens, currentPos);
                currentPos = blockEnd + 1;
                List<Token> blockTokens = [blockToken];
                AstNode blockNode = astParser.BuildAstTree(blockTokens);

                AstElseIf astElseIf = new(token, (AstBlock)blockNode, conditionNode)
                {
                    LineNumber = lineNumber - 1
                };

                return (astElseIf, currentPos);
            }
            else
            {
                int lineNumber = astParser.lineCount;
                var (blockToken, blockEnd) = FindTokenSafe(TokenType.BLOCK, tokens, currentPos);
                currentPos = blockEnd + 1;
                List<Token> blockTokens = [blockToken];
                AstNode blockNode = astParser.BuildAstTree(blockTokens);
                AstElse astElse = new(token, (AstBlock)blockNode)
                ;
                return (astElse, currentPos);
            }
        }

        public static (AstNode, int) ParseElseIf(Token token, List<Token> tokens, int currentPos, AstTree astParser)
        {
            var (conditionTokens, conditionEnd) = FindTokensInBracketsSafe(tokens, currentPos);
            currentPos = conditionEnd + 1;
            AstNode conditionNode = astParser.BuildAstTree(conditionTokens);

            var (blockToken, blockEnd) = FindTokenSafe(TokenType.BLOCK, tokens, currentPos);
            currentPos = blockEnd + 1;
            List<Token> blockTokens = [blockToken];
            AstNode blockNode = astParser.BuildAstTree(blockTokens);

            AstElseIf astElseIf = new(token, (AstBlock)blockNode, conditionNode) ;
            return (astElseIf, currentPos);
        }

        public static (AstNode, int) ParseWhile(Token token, List<Token> tokens, int currentPos, AstTree astParser)
        {
            var (conditionTokens, conditionEnd) = FindTokensInBracketsSafe(tokens, currentPos);
            currentPos = conditionEnd + 1;
            AstNode conditionNode = astParser.BuildAstTree(conditionTokens);

            var (blockToken, blockEnd) = FindTokenSafe(TokenType.BLOCK, tokens, currentPos);
            currentPos = blockEnd;
            List<Token> blockTokens = [blockToken];
            AstNode blockNode = astParser.BuildAstTree(blockTokens);

            AstWhile astWhile = new(token, (AstBlock)blockNode, conditionNode);
            return (astWhile, currentPos);
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
            var (blockToken, blockTokenEnd) = FindTokenSafe(TokenType.BLOCK, tokens, currentPos);
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
            var (blockToken, blockEnd) = FindTokenSafe(TokenType.BLOCK, tokens, currentPos);
            currentPos = blockEnd;
            AstNode blockNode = astParser.BuildAstTree([blockToken]);
            AstFunctionDecl astFunctionDecl = new(token, (AstIdentifier)identifierNode, argumentsNode, (AstBlock)blockNode);
            return (astFunctionDecl, currentPos);
        }

        public static (AstNode, int) ParseIdentifier(Token token, List<Token> tokens, int currentPos, AstTree astParser)
        {

            var (argumentsTokens, argumentsEnd) = FindTokensInBracketsSafe(tokens, currentPos,false);
            if (argumentsTokens == null || argumentsTokens.Count == 0)
            {
                return (NodeConverter.TokenToNode(token), currentPos + 1);
            }
            currentPos = argumentsEnd + 1;
            AstNode identifierNode = NodeConverter.TokenToNode(token);
            AstNode argumentsNode = astParser.BuildAstTree(argumentsTokens);
            Token callToken = new(TokenType.CALL, token.Value + "(" + argumentsTokens.ToCustomString() + ")");
            return (new AstCall(callToken, identifierNode, argumentsNode), currentPos);
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
            var (valueTokens, endOfValue) = TokensParser.AllTokensToEol(tokens, currentPos + 1);
            if (valueTokens.Count == 0)
            {
                throw new BifySyntaxError("Expected value after assignment operator", tokens, tokens, currentPos);
            }
            currentPos = endOfValue + 1;
            AstNode valueNode = astParser.BuildAstTree(valueTokens);
            return (new AstAssignmentOperator(token, (AstIdentifier)identifierNode, valueNode), currentPos);
        }

        private static (Token, int) FindTokenSafe(TokenType tokenType, List<Token> tokens, int currentPos)
        {
            var (token, index) = TokensParser.FindTokenByTT(tokenType, tokens, currentPos);
            if (token == null)
            {
                throw new BifySyntaxError($"Expected token of type {tokenType} not found");
            }
            return (token, index);
        }

        private static (List<Token>, int) FindTokensInBracketsSafe(List<Token> tokens, int currentPos,bool throwError = true)
        {
            var (conditionTokens, conditionEnd) = TokensParser.TokensInBrackets(tokens, currentPos);
            if (conditionTokens == null || conditionTokens.Count == 0)
            {
                if (throwError)
                    throw new BifyParsingError("Tokens in brackets not found", tokens, tokens, 0);
                else
                {
                    return (new List<Token>(), currentPos);
                }
            }
            return (conditionTokens, conditionEnd);
        }
    }
}
