using System;
using System.Collections.Generic;
using System.Data;
using BoomifyCS.Lexer;

namespace BoomifyCS.Parser
{
    public class TokensParser
    {
        /// <summary>
        /// Splits a list of tokens into a line of tokens starting from a specified position.
        /// </summary>
        /// <param name="tokens">The list of tokens to split into lines.</param>
        /// <param name="tokenPosition">The starting position in the tokens list to begin splitting.</param>
        /// <returns>
        /// A tuple containing:
        /// - A list of tokens representing the line starting from <paramref name="tokenPosition"/>.
        /// - The updated position in the tokens list after the line has been extracted.
        /// </returns>
        public static Tuple<List<Token>, int> SplitTokensByLine(List<Token> tokens, int tokenPosition)
        {
            List<Token> lineTokens = [];
            Token currentToken = tokens[tokenPosition];
            
            while (tokenPosition < tokens.Count && currentToken.Type != TokenType.EOL && currentToken.Type != TokenType.BLOCK)
            {
                currentToken = tokens[tokenPosition];
                lineTokens.Add(currentToken);
                tokenPosition++;
            }
            if (currentToken .Type == TokenType.EOL)
            {
                tokenPosition++;
            }
            return new Tuple<List<Token>, int>(lineTokens, tokenPosition);
        }

        public static bool IsOperator(string key)
        {

            return TokenConfig.binaryOperators.ContainsKey(key);
    
        }
        public static bool IsOperator(TokenType type)
        {

            return TokenConfig.binaryOperators.ContainsValue(type);
        }
        public static (Token,int) FindTokenByTT(TokenType tokenType,List<Token> tokens,int start = 0)
        {
            for (int i = start; i < tokens.Count;i++)
            {
                Token token = tokens[i];
                if (token.Type == tokenType)
                {
                    return (token, i);
                }
            }
            return (null,1);

        }
        public static (List<Token>,int) AllTokensToEol(List<Token> tokens,int start = 0)
        {
            List<Token> result = [];
            for (int i = start; i < tokens.Count; i++)
            {
                Token token = tokens[i];
                if (token.Type == TokenType.EOL)
                {
                    return (result, i);
                }
                result.Add(token);
            }
            return (result,tokens.Count);
        }

        public static (List<Token>, int) TokensInBrackets(List<Token> tokens, int start = 0)
        {
            List<Token> result = [];
            int bracketCounter = 0;
            bool insideBrackets = false;
            for (int i = start; i < tokens.Count; i++)
            {
                Token token = tokens[i];

                if (token.Type == TokenType.LPAREN)
                {
                    bracketCounter++;
                    if (!insideBrackets)
                    {
                        insideBrackets = true;
                    }
                    else
                    {
                        result.Add(token);
                    }
                }
                else if (token.Type == TokenType.RPAREN)
                {
                    bracketCounter--;
                    if (bracketCounter == 0)
                    {
                        return (result, i);
                    }
                    else
                    {
                        result.Add(token); 
                    }
                }
                else
                {
                    if (insideBrackets)
                    {
                        result.Add(token);
                    }
                }
            }

            if (bracketCounter > 0)
            {
                throw new InvalidOperationException("Unmatched parentheses in token list.");
            }

            return (result, tokens.Count);
        }
        public static List<List<Token>> SplitTokensByTT(List<Token> tokens, TokenType tokenType, int start = 0)
        {
            List<List<Token>> result = [];
            int lastTokenIndex = start;

            for (int i = start; i < tokens.Count; i++)
            {
                Token token = tokens[i];
                if (token.Type == tokenType)
                {
                    result.Add(tokens.GetRange(lastTokenIndex, i - lastTokenIndex));
                    lastTokenIndex = i + 1;
                }
            }

            if (lastTokenIndex < tokens.Count)
            {
                result.Add(tokens.GetRange(lastTokenIndex, tokens.Count - lastTokenIndex));
            }

            return result;
        }


    }

}

