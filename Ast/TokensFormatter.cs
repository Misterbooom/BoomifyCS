using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomifyCS.Exceptions;
using BoomifyCS.Lexer;

namespace BoomifyCS.Ast
{
    class TokensFormatter
    {
        public static List<Token> NextLine(List<Token> tokens, ref int tokenIndex)
        {
            List<Token> lineTokens = new();
            int curlyCount = 0;
            int elseCount = 0;
            int ifCount = 0;
            while (tokens.Count > tokenIndex)
            {
                Token token = tokens[tokenIndex];
                Token nextToken = GetTokenOrNull(tokens, tokenIndex + 1);
                Token previousToken = GetTokenOrNull(tokens, tokenIndex - 1);
                if (token.Type == TokenType.LCUR)
                {
                    curlyCount++;
                }
                else if (token.Type == TokenType.RCUR)
                {
                    
                    curlyCount--;
                    if (curlyCount <= 0 && tokenIndex + 1 < tokens.Count)
                    {
                        if (nextToken.Type != TokenType.IF && nextToken.Type != TokenType.ELSE)
                        {
                            lineTokens.Add(token);
                            tokenIndex++;
                            break;
                        }
                        
                    }
                }
                if (token.Type == TokenType.EOL && curlyCount == 0)
                {
                    tokenIndex++;
                    break;
                }
                else if (token.Type == TokenType.ELSE && nextToken.Type != TokenType.IF)
                {
                    elseCount++;
                }
                else if (token.Type == TokenType.IF && previousToken != null && previousToken.Type != TokenType.ELSE)
                {
                    ifCount++;
                }
                if (elseCount > 1 || ifCount > 1)
                {
                    break;
                }
                lineTokens.Add(token);
                tokenIndex++;

               
            }

            return lineTokens;
        }
       
        public static Token GetTokenOrNull(List<Token> tokens, int index) {
            if (index + 1  > tokens.Count || index < 0)
            {
                return null;
            }
            return tokens[index];
        }
        public static List<List<Token>> SplitTokensByType(List<Token> tokens, TokenType type)
        {
            List<List<Token>> tokenGroups = new();
            List<Token> currentGroup = new();

            foreach (Token token in tokens)
            {
                if (token.Type == type)
                {
                    if (currentGroup.Count > 0)
                    {
                        tokenGroups.Add(currentGroup);
                        currentGroup = new(); 
                    }
                }
                else
                {
                    currentGroup.Add(token);
                }
            }

            if (currentGroup.Count > 0)
            {
                tokenGroups.Add(currentGroup);
            }

            return tokenGroups;
        }

        public static List<Token> GetTokensBetween(List<Token> tokens, ref int index, TokenType open, TokenType close)
        {
            int count = 0;
            List<Token> newTokens = new();

            while (index < tokens.Count)
            {
                Token token = tokens[index];

                if (token.Type == open)
                {
                    count++;
                }
                else if (token.Type == close)
                {
                    count--;
                    if (count == 0)
                    {
                        break;
                    }
                }

                if (count != 0)
                {
                    newTokens.Add(token);
                }

                index++;
            }

            if (count > 0)
            {
                Token lastOpenToken = tokens[index - 1];
                BifySyntaxError error = new(
                    ErrorMessage.UnmatchedToken(open.ToString(), close.ToString()),
                    "",
                    GetKeyByValue(TokenConfig.singleCharTokens, open).ToString()
 

                );

                Traceback.Traceback.Instance.ThrowException(error,lastOpenToken.Column - 1);
            }

            return newTokens[1..];
        }
        private static char GetKeyByValue(Dictionary<char, TokenType> dictionary, TokenType value)
        {
            var keyValuePair = dictionary.FirstOrDefault(kv => kv.Value == value);

            if (!EqualityComparer<KeyValuePair<char, TokenType>>.Default.Equals(keyValuePair, default))
            {
                return keyValuePair.Key;
            }
            return ' ';
        }




    }

}
