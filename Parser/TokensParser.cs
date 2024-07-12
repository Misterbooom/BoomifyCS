using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            List<Token> lineTokens = new List<Token>();
            Token currentToken = tokens[tokenPosition];
            while (tokenPosition < tokens.Count && currentToken.Type != TokenType.EOL)
            {
                currentToken = tokens[tokenPosition];
                lineTokens.Add(currentToken);
                tokenPosition++;
            }
            return new Tuple<List<Token>, int>(lineTokens, tokenPosition);
        }
        public static bool IsOperator(string key)
        {

            return TokenConfig.arithmeticOperators.ContainsKey(key);
    
        }
        public static bool IsOperator(TokenType type)
        {

            return TokenConfig.arithmeticOperators.ContainsValue(type);
        }
        public static Tuple<Token,int> FindTokenByTT(TokenType tokenType,List<Token> tokens,int start = 0)
        {
            for (int i = start; i < tokens.Count;i++)
            {
                Token token = tokens[i];
                if (token.Type == tokenType)
                {
                    return new Tuple<Token, int>(token, i);
                }
            }
            return null;

        }
        public static Tuple<List<Token>,int> AllTokensToEol(List<Token> tokens,int start = 0)
        {
            List<Token> result = new List<Token>();
            for (int i = start; i < tokens.Count; i++)
            {
                Token token = tokens[i];
                if (token.Type == TokenType.EOL)
                {
                    return new Tuple<List<Token>, int>(result, i);
                }
                result.Add(token);
            }
            throw new NotImplementedException();
        }
    }

}

