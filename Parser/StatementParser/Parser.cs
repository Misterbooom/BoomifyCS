using System;
using System.Collections.Generic;
using BoomifyCS.Ast;
using BoomifyCS.Exceptions;
using BoomifyCS.Lexer;
using BoomifyCS.Objects;
using BoomifyCS.Parser.NodeParser;
using BoomifyCS.Parser;
namespace BoomifyCS.Parser.StatementParser
{
    public class Parser
    {
        public delegate (AstNode, int) StatementParserDelegate(Token token, List<Token> tokens, int currentPos, AstTree astParser);

        public static StatementParserDelegate ParseWhile = LoopStatementParser.ParseWhile;
        public static StatementParserDelegate ParseIf = ConditionalStatementParser.ParseIf;
        public static StatementParserDelegate ParseElse = ConditionalStatementParser.ParseElse;
        public static StatementParserDelegate ParseFor = LoopStatementParser.ParseFor;
        public static StatementParserDelegate ParseIdentifier = IdentifierStatementParser.ParseIdentifier;

        

        public static (AstNode, int) ParseVarDecl(Token token, List<Token> tokens, int currentPos, AstTree astParser)
        {
            var varNameToken = TokenFinder.FindRequiredToken(TokenType.IDENTIFIER, tokens, ref currentPos);
            var assignmentToken = TokenFinder.FindRequiredToken(TokenType.ASSIGN, tokens, ref currentPos);

            currentPos++;
            var (varValueTokens, tokensProcessed) = TokensParser.AllTokensToEol(tokens, currentPos);
            currentPos = tokensProcessed + 1;
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

      
        public static (AstNode, int) ParseFunctionDecl(Token token, List<Token> tokens, int currentPos, AstTree astParser)
        {
            var (identifierToken, tokenEnd) = TokenFinder.FindTokenSafe(TokenType.IDENTIFIER, tokens, currentPos);
            currentPos = tokenEnd + 1;
            var (argumentsTokens, argumentsEnd) = TokenFinder.FindTokensInBracketsSafe(tokens, currentPos,ErrorMessage.InvalidFunctionDeclaration());
            AstNode identifierNode = NodeConverter.TokenToNode(identifierToken);
            currentPos = argumentsEnd + 1;
            AstNode argumentsNode = astParser.BuildAstTree(argumentsTokens);
            var (blockToken, blockEnd) = TokenFinder.FindTokenSafe(TokenType.BLOCK, tokens, currentPos);
            currentPos = blockEnd;
            AstNode blockNode = astParser.BuildAstTree([blockToken]);
            AstFunctionDecl astFunctionDecl = new(token, (AstIdentifier)identifierNode, argumentsNode, (AstBlock)blockNode);
            return (astFunctionDecl, currentPos);
        }



        public static (AstNode, int) ParseAssignmentOperator(Token token, List<Token> tokens, int currentPos, AstTree astParser)
        {
            var identifierToken = TokenFinder.GetPreviousTokenOrThrow(tokens, currentPos);
            AstNode identifierNode = NodeConverter.TokenToNode(identifierToken);

            var (valueTokens, endOfValue) = TokensParser.AllTokensToEol(tokens, currentPos + 1);
            if (valueTokens.Count == 0)
            {
                throw new BifySyntaxError(ErrorMessage.ExpectedValueAfterAssignment());
            }

            currentPos = endOfValue + 1;
            AstNode valueNode = astParser.BuildAstTree(valueTokens);

            return (new AstAssignmentOperator(token, (AstIdentifier)identifierNode, valueNode), currentPos);
        }
        public static (AstNode, int) ParseBracket(Token _, List<Token> tokens, int currentPos, AstTree astParser)
        {
            var (argumentsTokens, endOfValue) = TokensParser.AllTokensToEol(tokens, currentPos + 1);
            currentPos = endOfValue + 1;
            argumentsTokens = argumentsTokens[..^1];
            if (argumentsTokens.ContainsTokenType(TokenType.COMMA))
            {
                return ParseArray(argumentsTokens, currentPos, astParser);
            }

            return ParseIndexOperator(argumentsTokens, currentPos, astParser);
        }
        private static (AstNode, int) ParseIndexOperator(List<Token> argumentsTokens, int currentPos, AstTree astParser)
        {
            AstNode indexValue = astParser.BuildAstTree(argumentsTokens);
            argumentsTokens.WriteTokens();
            if (indexValue is not AstConstant && indexValue is not AstBinaryOp && indexValue is not AstIdentifier)
            {
                throw new BifyTypeError(ErrorMessage.IndexOfArrayTypeError());
            }
            return (new AstIndexOperator(indexValue, null), currentPos);
        }
        private static (AstNode, int) ParseArray(List<Token> argumentsTokens, int currentPos, AstTree astParser)
        {
            AstNode argumentsNode = astParser.BuildAstTree(argumentsTokens);
            AstArray astArray = new(argumentsTokens.TokensToString().StringToToken(), argumentsNode);
            return (astArray, currentPos);
        }




    }
}