using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomifyCS.Exceptions;
using BoomifyCS.Lexer;

namespace BoomifyCS.Parser.StatementParser
{
    public class TokenFinder
    {
        public static (Token, int) FindTokenSafe(TokenType tokenType, List<Token> tokens, int currentPos)
        {
            var (token, index) = TokensParser.FindTokenByTT(tokenType, tokens, currentPos);
            if (token == null)
            {
                throw new BifySyntaxError($"Expected token of type {tokenType} not found");
            }
            return (token, index);
        }

        public static (List<Token>, int) FindTokensInBracketsSafe(List<Token> tokens, int currentPos, bool throwError = true)
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
        public static Token FindRequiredToken(TokenType tokenType, List<Token> tokens, ref int currentPos)
        {
            var (token, index) = TokensParser.FindTokenByTT(tokenType, tokens, currentPos);
            if (token == null)
            {
                throw new BifySyntaxError($"Expected token of type {tokenType} not found");
            }
            currentPos = index;
            return token;
        }
        public static bool IsNextToken(List<Token> tokens, int currentPos,TokenType tokenType)
        {
            return tokens.Count >= currentPos + 1 && tokens[currentPos + 1].Type == tokenType;
        }
        public static Token GetPreviousTokenOrThrow(List<Token> tokens, int currentPos)
        {
            if (currentPos - 1 < 0 || tokens[currentPos - 1].Type != TokenType.IDENTIFIER)
            {
                throw new BifySyntaxError("Expected identifier token before assignment operator");
            }

            return tokens[currentPos - 1];
        }
        public static List<Token> ParseOptionalArguments(List<Token> tokens, ref int currentPos)
        {
            var (argumentsTokens, argumentsEnd) = TokenFinder.FindTokensInBracketsSafe(tokens, currentPos, false);
            if (argumentsTokens.Count == 0)
            {
                return null;
            }

            currentPos = argumentsEnd + 1;
            return argumentsTokens;
        }
    }
}

