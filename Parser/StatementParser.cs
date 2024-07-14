﻿using System;
using System.Collections.Generic;
using BoomifyCS.Ast;
using BoomifyCS.Lexer;

namespace BoomifyCS.Parser
{
    public static class StatementParser
    {
        public static (AstNode, int) ParseVarDecl(Token token, List<Token> tokens, int currentPos)
        {
            try
            {
                var (varNameToken, varNameIndex) = FindTokenSafe(TokenType.IDENTIFIER, tokens, currentPos);
                var (assignmentToken, assignmentIndex) = FindTokenSafe(TokenType.ASSIGN, tokens, currentPos);

                currentPos = assignmentIndex + 1;
                AstAssignment assignmentNode = new AstAssignment(assignmentToken);

                var (varValueTokens, tokensProcessed) = TokensParser.AllTokensToEol(tokens, currentPos);
                currentPos += tokensProcessed;

                AstNode varNameNode = NodeParser.TokenToNode(varNameToken);
                AstNode varValueNode = NodeParser.TokenToAst(varValueTokens);

                assignmentNode.Left = varNameNode;
                assignmentNode.Right = varValueNode;

                AstVarDecl astVarDecl = new AstVarDecl(token, assignmentNode);
                return (astVarDecl, currentPos);
            }
            catch (NullReferenceException)
            {
                throw new BifySyntaxError("Failed to parse variable declaration", tokens, new List<Token> { token }, 0);
            }
        }

        public static (AstNode, int) ParseIf(Token token, List<Token> tokens, int currentPos)
        {
            try
            {
                var (conditionTokens, conditionEnd) = FindTokensInBracketsSafe(tokens, currentPos);
                currentPos = conditionEnd + 1;
                AstNode conditionNode = NodeParser.TokenToAst(conditionTokens);

                var (blockToken, blockEnd) = FindTokenSafe(TokenType.OBJECT, tokens, currentPos);
                currentPos = blockEnd;
                List<Token> blockTokens = new List<Token> { blockToken };
                AstNode blockNode = NodeParser.TokenToAst(blockTokens);

                AstIf astIf = new AstIf(token, conditionNode, (AstBlock)blockNode);
                return  (astIf, currentPos);
            }
            catch (NullReferenceException)
            {
                throw new BifySyntaxError("Failed to parse if statement", tokens, new List<Token> { token }, 0);
            }
        }

        public static (AstNode, int) ParseElse(Token token, List<Token> tokens, int currentPos)
        {
            try
            {
                var (blockToken, blockEnd) = FindTokenSafe(TokenType.OBJECT, tokens, currentPos);
                currentPos = blockEnd;
                List<Token> blockTokens = new List<Token> { blockToken };
                AstNode blockNode = NodeParser.TokenToAst(blockTokens);

                AstElse astElse = new AstElse(token, (AstBlock)blockNode);
                return (astElse, currentPos);
            }
            catch (NullReferenceException)
            {
                throw new BifySyntaxError("Failed to parse else statement", tokens, new List<Token> { token }, 0    );
            }
        }

        public static (AstNode, int) ParseElseIf(Token token, List<Token> tokens, int currentPos)
        {
            try
            {
                var (conditionTokens, conditionEnd) = FindTokensInBracketsSafe(tokens, currentPos);
                currentPos = conditionEnd + 1;
                AstNode conditionNode = NodeParser.TokenToAst(conditionTokens);

                var (blockToken, blockEnd) = FindTokenSafe(TokenType.OBJECT, tokens, currentPos);
                currentPos = blockEnd;
                List<Token> blockTokens = new List<Token> { blockToken };
                AstNode blockNode = NodeParser.TokenToAst(blockTokens);

                AstElseIf astElseIf = new AstElseIf(token, (AstBlock)blockNode, conditionNode);
                return (astElseIf, currentPos);
            }
            catch (NullReferenceException)
            {
                throw new BifySyntaxError("Failed to parse else-if statement", tokens, new List<Token> { token }, 0);
            }
        }
        public static (AstNode, int) ParseWhile(Token token, List<Token>tokens, int currentPos)
        {
            try
            {
                var (conditionTokens, conditionEnd) = FindTokensInBracketsSafe(tokens, currentPos);
                currentPos = conditionEnd + 1;
                AstNode conditionNode = NodeParser.TokenToAst(conditionTokens);

                var (blockToken, blockEnd) = FindTokenSafe(TokenType.OBJECT, tokens, currentPos);
                currentPos = blockEnd;
                List<Token> blockTokens = new List<Token> { blockToken };
                AstNode blockNode = NodeParser.TokenToAst(blockTokens);

                AstWhile astWhile = new AstWhile(token, (AstBlock)blockNode, conditionNode);
                return (astWhile,currentPos);
            }
            catch (NullReferenceException)
            {
                throw new BifySyntaxError("Failed to parse while statement", tokens, new List<Token> { token }, 0);
            }
        }
        public static (AstNode,int) ParseFor(Token token, List<Token> tokens, int currentPos)
        {
            var (tokensInBrackets,bracketEnd) = FindTokensInBracketsSafe(tokens,currentPos);
            currentPos = bracketEnd + 1;
            List<List<Token>> bracketTokensSplited = TokensParser.SplitTokensByTT(tokensInBrackets, TokenType.SEMICOLON);
            bracketTokensSplited[0].Add(new Token(TokenType.EOL, ";"));
            AstNode initNode = NodeParser.BuiltTokensToAst(bracketTokensSplited[0]);
            AstNode conditionNode = NodeParser.BuiltTokensToAst(bracketTokensSplited[1]);
            AstNode incrementNode = NodeParser.BuiltTokensToAst(bracketTokensSplited[2]);

            var (blockToken,blockTokenEnd) = FindTokenSafe(TokenType.OBJECT, tokens, currentPos);
            currentPos += blockTokenEnd;
            AstNode blockNode = NodeParser.TokenToAst(blockToken);
            return (new AstFor(token,(AstBlock)blockNode,conditionNode,incrementNode,initNode),currentPos);
        }
        public static (AstNode,int) ParseUnaryOp(Token token, List<Token> tokens,int currentPos)
        {
            var (identifierToken, tokenEnd) = FindTokenSafe(TokenType.IDENTIFIER, tokens, currentPos - 1);
            currentPos += tokenEnd;
            if (token.Type == TokenType.INCREMENT)
            {
                AstUnaryOperator unaryOperator = new AstUnaryOperator(token, NodeParser.TokenToNode(identifierToken),1);
                return (unaryOperator, currentPos);


            }
            else
            {
                AstUnaryOperator unaryOperator = new AstUnaryOperator(token, NodeParser.TokenToNode(identifierToken), -1);
                return (unaryOperator, currentPos);


            }
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
                    throw new NullReferenceException();
                }
                return (conditionTokens, conditionEnd);
            }
            catch (NullReferenceException)
            {
                throw new BifySyntaxError("Condition tokens in brackets not found", tokens, tokens, 0);
            }
        }
        
    }
}
