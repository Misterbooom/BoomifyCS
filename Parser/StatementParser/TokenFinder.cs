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
                throw new BifySyntaxError(ErrorMessage.ExpectedTokenNotFound(tokenType.ToString()));
            }
            return (token, index);
        }

        public static (List<Token>, int) FindTokensInBracketsSafe(List<Token> tokens, int currentPos, string message,bool throwError = true)
        {
            var (conditionTokens, conditionEnd) = TokensParser.TokensInBrackets(tokens, currentPos);
            if (conditionTokens == null)
            {
                if (throwError)
                    throw new BifyParsingError(message);
                return (null, currentPos);
            }
            return (conditionTokens, conditionEnd);
        }
        public static Token FindRequiredToken(TokenType tokenType, List<Token> tokens, ref int currentPos)
        {
            var (token, index) = TokensParser.FindTokenByTT(tokenType, tokens, currentPos);
            if (token == null)
            {
                throw new BifySyntaxError(ErrorMessage.ExpectedTokenNotFound(tokenType.ToString()));
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
        public static List<Token> ParseOptionalArguments(List<Token> tokens, ref int currentPos,string message)
        {
            var (argumentsTokens, argumentsEnd) = TokenFinder.FindTokensInBracketsSafe(tokens, currentPos, message, false);
            if (argumentsTokens == null)
            {
                return null;
            }

            currentPos = argumentsEnd + 1;
            return argumentsTokens;
        }
    }
}

