using System;
using System.Collections.Generic;
using BoomifyCS.Ast;
using BoomifyCS.Exceptions;
using BoomifyCS.Lexer;
using BoomifyCS.Objects;
using BoomifyCS.Parser.NodeParser;
using BoomifyCS.Parser;
namespace BoomifyCS.Parser
{
    public static class StatementParser
    {
        public static (AstNode, int) ParseVarDecl(Token token, List<Token> tokens, int currentPos, AstTree astParser)
        {
            var varNameToken = FindRequiredToken(TokenType.IDENTIFIER, tokens, ref currentPos);
            var assignmentToken = FindRequiredToken(TokenType.ASSIGN, tokens, ref currentPos);

            currentPos++;  
            var (varValueTokens, tokensProcessed) = TokensParser.AllTokensToEol(tokens, currentPos);
            //varValueTokens.WriteTokens();
            currentPos = tokensProcessed + 1;
            //varValueTokens.WriteTokens();
            AstNode varNameNode = new AstVar(varNameToken, new BifyVar(varNameToken.Value));
            AstNode varValueNode = astParser.BuildAstTree(varValueTokens);
            AstAssignment assignmentNode = new(assignmentToken)
            {
                Left = varNameNode,
                Right = varValueNode
            };

            AstVarDecl astVarDecl = new(token, assignmentNode);
            return (astVarDecl, currentPos);
        }

        public static (AstNode, int) ParseIf(Token token, List<Token> tokens, int currentPos, AstTree astParser)
        {
            var conditionNode = ParseCondition(tokens, ref currentPos, astParser);
            var blockNode = ParseBlock(tokens, ref currentPos, astParser);

            return (new AstIf(token, conditionNode, (AstBlock)blockNode), currentPos);
        }

        public static (AstNode, int) ParseElse(Token token, List<Token> tokens, int currentPos, AstTree astParser)
        {
            AstNode elseNode;

            if (IsNextTokenElseIf(tokens, currentPos))
            {
                elseNode = ParseElseIf(token, tokens, currentPos, astParser).Item1;
            }
            else
            {
                elseNode = ParseElseBlock(token, tokens, ref currentPos, astParser);
            }

            return (elseNode, currentPos);
        }

        public static (AstNode, int) ParseElseIf(Token token, List<Token> tokens, int currentPos, AstTree astParser)
        {
            var conditionNode = ParseCondition(tokens, ref currentPos, astParser);
            var blockNode = ParseBlock(tokens, ref currentPos, astParser);

            return (new AstElseIf(token, (AstBlock)blockNode, conditionNode), currentPos);
        }

        public static (AstNode, int) ParseWhile(Token token, List<Token> tokens, int currentPos, AstTree astParser)
        {
            var conditionNode = ParseCondition(tokens, ref currentPos, astParser);
            var blockNode = ParseBlock(tokens, ref currentPos, astParser);

            return (new AstWhile(token, (AstBlock)blockNode, conditionNode), currentPos);
        }

        public static (AstNode, int) ParseFor(Token token, List<Token> tokens, int currentPos, AstTree astParser)
        {
            var (initNode, conditionNode, incrementNode) = ParseForLoopHeader(tokens, ref currentPos, astParser);
            var blockNode = ParseBlock(tokens, ref currentPos, astParser);

            ValidateForLoop(initNode, conditionNode, incrementNode);

            return (new AstFor(token, (AstBlock)blockNode, conditionNode, incrementNode, initNode), currentPos);
        }



        public static (AstNode, int) ParseFunctionDecl(Token token, List<Token> tokens, int currentPos, AstTree astParser)
        {
            var (identifierToken, tokenEnd) = FindTokenSafe(TokenType.IDENTIFIER, tokens, currentPos);
            currentPos = tokenEnd + 1;
            var (argumentsTokens, argumentsEnd) = FindTokensInBracketsSafe(tokens, currentPos);
            AstNode identifierNode = NodeConverter.TokenToNode(identifierToken);
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
            var argumentsTokens = ParseOptionalArguments(tokens, ref currentPos);

            if (argumentsTokens == null || argumentsTokens.Count == 0)
            {
                return HandleIdentifierWithoutArguments(token, tokens, ref currentPos);
            }

            return ParseFunctionCall(token, argumentsTokens, ref currentPos, astParser);
        }

        public static (AstNode, int) ParseAssignmentOperator(Token token, List<Token> tokens, int currentPos, AstTree astParser)
        {
            var identifierToken = GetPreviousTokenOrThrow(tokens, currentPos);
            AstNode identifierNode = NodeConverter.TokenToNode(identifierToken);

            var (valueTokens, endOfValue) = TokensParser.AllTokensToEol(tokens, currentPos + 1);
            if (valueTokens.Count == 0)
            {
                throw new BifySyntaxError("Expected value after assignment operator");
            }

            currentPos = endOfValue + 1;
            AstNode valueNode = astParser.BuildAstTree(valueTokens);

            return (new AstAssignmentOperator(token, (AstIdentifier)identifierNode, valueNode), currentPos);
        }
        public static (AstNode, int) ParseBracket(Token _, List<Token> tokens, int currentPos, AstTree astParser) {
            var (argumentsTokens, endOfValue) = TokensParser.AllTokensToEol(tokens, currentPos + 1);
            currentPos = endOfValue + 1;
            argumentsTokens = argumentsTokens[..^1];
            if (argumentsTokens.ContainsTokenType(TokenType.COMMA))
            {
                return ParseArray(argumentsTokens,currentPos,astParser);
            }

            return ParseIndexOperator(argumentsTokens, currentPos, astParser);
        }
        private static (AstNode, int) ParseIndexOperator(List<Token> argumentsTokens, int currentPos, AstTree astParser)
        {
            AstNode indexValue = astParser.BuildAstTree(argumentsTokens);
            if (indexValue is not AstConstant && indexValue is not AstBinaryOp && indexValue is not AstIdentifier)
            {
                throw new BifyTypeError(ErrorMessage.IndexOfArrayTypeError());
            }
            return (new AstIndexOperator(indexValue,null),currentPos);
        }
        private static (AstNode, int) ParseArray(List<Token> argumentsTokens,int currentPos,AstTree astParser)
        {
            AstNode argumentsNode = astParser.BuildAstTree(argumentsTokens);
            AstArray astArray = new(argumentsTokens.TokensToString().StringToToken(), argumentsNode);
            return (astArray, currentPos);
        }

        private static Token FindRequiredToken(TokenType tokenType, List<Token> tokens, ref int currentPos)
        {
            var (token, index) = TokensParser.FindTokenByTT(tokenType, tokens, currentPos);
            if (token == null)
            {
                throw new BifySyntaxError($"Expected token of type {tokenType} not found");
            }
            currentPos = index;
            return token;
        }

        private static AstNode ParseCondition(List<Token> tokens, ref int currentPos, AstTree astParser)
        {
            var (conditionTokens, conditionEnd) = FindTokensInBracketsSafe(tokens, currentPos);
            currentPos = conditionEnd + 1;
            return astParser.BuildAstTree(conditionTokens);
        }

        private static AstNode ParseBlock(List<Token> tokens, ref int currentPos, AstTree astParser)
        {
            var (blockToken, blockEnd) = FindTokenSafe(TokenType.BLOCK, tokens, currentPos);
            currentPos = blockEnd + 1;
            return astParser.BuildAstTree([blockToken]);
        }

        private static (AstNode, AstNode, AstNode) ParseForLoopHeader(List<Token> tokens, ref int currentPos, AstTree astParser)
        {
            var (tokensInBrackets, bracketEnd) = FindTokensInBracketsSafe(tokens, currentPos);
            currentPos = bracketEnd + 1;
            List<List<Token>> bracketTokensSplit = TokensParser.SplitTokensByTT(tokensInBrackets, TokenType.SEMICOLON);

            ValidateForLoopHeader(bracketTokensSplit);

            AstNode initNode = astParser.BuildAstTree(bracketTokensSplit[0]);
            AstNode conditionNode = astParser.BuildAstTree(bracketTokensSplit[1]);
            AstNode incrementNode = astParser.BuildAstTree(bracketTokensSplit[2]);

            return (initNode, conditionNode, incrementNode);
        }

        private static void ValidateForLoopHeader(List<List<Token>> bracketTokensSplit)
        {
            if (bracketTokensSplit.Count != 3)
            {
                throw new BifyInitializationError(ErrorMessage.InvalidInitializationStatement());
            }

            if (bracketTokensSplit[0].Count == 0)
            {
                throw new BifyInitializationError(ErrorMessage.ForLoopMustHaveInitialization());
            }

            if (bracketTokensSplit[1].Count == 0)
            {
                throw new BifyInitializationError(ErrorMessage.ForLoopMustHaveCondition());
            }

            if (bracketTokensSplit[2].Count == 0)
            {
                throw new BifyInitializationError(ErrorMessage.ForLoopMustHaveIncrement());
            }
        }

        private static void ValidateForLoop(AstNode initNode, AstNode conditionNode, AstNode incrementNode)
        {
            if (incrementNode is not AstAssignmentOperator && incrementNode is not AstUnaryOperator && incrementNode is not AstCall)
            {
                throw new BifyInitializationError(ErrorMessage.InvalidIncrementExpression());
            }

            if (initNode is not AstVarDecl)
            {
                throw new BifyInitializationError(ErrorMessage.InvalidVariableDeclaration());
            }

            if (conditionNode is not AstBinaryOp)
            {
                throw new BifyInitializationError(ErrorMessage.InvalidConditionExpression());
            }
        }

        private static Token GetPreviousTokenOrThrow(List<Token> tokens, int currentPos)
        {
            if (currentPos - 1 < 0 || tokens[currentPos - 1].Type != TokenType.IDENTIFIER)
            {
                throw new BifySyntaxError("Expected identifier token before assignment operator");
            }

            return tokens[currentPos - 1];
        }

        private static bool IsNextTokenElseIf(List<Token> tokens, int currentPos)
        {
            return tokens.Count >= currentPos + 1 && tokens[currentPos + 1].Type == TokenType.IF;
        }

        private static AstElse ParseElseBlock(Token token, List<Token> tokens, ref int currentPos, AstTree astParser)
        {
            var blockNode = ParseBlock(tokens, ref currentPos, astParser);
            return new AstElse(token, (AstBlock)blockNode);
        }

        private static List<Token> ParseOptionalArguments(List<Token> tokens, ref int currentPos)
        {
            var (argumentsTokens, argumentsEnd) = FindTokensInBracketsSafe(tokens, currentPos, false);
            if (argumentsTokens.Count == 0)
            {
                return null;
            }

            currentPos = argumentsEnd + 1;
            return argumentsTokens;
        }

        private static (AstNode, int) HandleIdentifierWithoutArguments(Token token, List<Token> tokens, ref int currentPos)
        {

            if (tokens.Count < currentPos + 1)
            {
                return (NodeConverter.TokenToNode(token), currentPos);
            }

            var nextToken = tokens[currentPos + 1];
            if (nextToken.Type == TokenType.INCREMENT || nextToken.Type == TokenType.DECREMENT)
            {
                AstUnaryOperator unaryOperator = new(token, NodeConverter.TokenToNode(token), 1);
                return (unaryOperator, currentPos + 1);
            }

            return (NodeConverter.TokenToNode(token), currentPos);
        }

        private static (AstNode, int) ParseFunctionCall(Token token, List<Token> argumentsTokens, ref int currentPos, AstTree astParser)
        {
            AstNode identifierNode = NodeConverter.TokenToNode(token);
            AstNode argumentsNode = astParser.BuildAstTree(argumentsTokens);
            Token callToken = new(TokenType.CALL, token.Value + "(" + argumentsTokens.ToString() + ")");
            return (new AstCall(callToken, identifierNode, argumentsNode), currentPos);
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

        private static (List<Token>, int) FindTokensInBracketsSafe(List<Token> tokens, int currentPos, bool throwError = true)
        {
            var (conditionTokens, conditionEnd) = TokensParser.TokensInBrackets(tokens, currentPos);
            if (conditionTokens == null || conditionTokens.Count == 0)
            {
                if (throwError)
                    throw new BifyParsingError("Tokens in brackets not found");
                return (new List<Token>(), currentPos);
            }
            return (conditionTokens, conditionEnd);
        }
    }
}
