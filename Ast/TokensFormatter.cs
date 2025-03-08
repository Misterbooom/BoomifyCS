using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomifyCS.Exceptions;
using BoomifyCS.Lexer;
using BoomifyCS.Parser;

namespace BoomifyCS.Ast
{
    class TokensFormatter
    {
        public static List<Token> NextLine(List<Token> tokens, ref int tokenIndex)
        {
            List<Token> lineTokens = new();
            int curlyCount = 0;
            bool isInsideConditionChain = false; 

            while (tokenIndex < tokens.Count)
            {
                Token token = tokens[tokenIndex];
                Token nextToken = GetTokenOrNull(tokens, tokenIndex + 1);
                Token prevToken = GetTokenOrNull(tokens, tokenIndex - 1);

                if (lineTokens.Count > 0 && IsNewCondition(token, prevToken) && !isInsideConditionChain)
                {
                    break;
                }

                if (token.Type == TokenType.LCUR) curlyCount++;
                else if (token.Type == TokenType.RCUR)
                {
                    curlyCount = Math.Max(0, curlyCount - 1);
                    if (curlyCount == 0 && !IsConditionChainToken(nextToken))
                    {
                        lineTokens.Add(token);
                        tokenIndex++;
                        break;
                    }
                }

                lineTokens.Add(token);
                tokenIndex++;

                isInsideConditionChain = IsConditionChainToken(token) ||
                                        (nextToken != null && IsConditionChainToken(nextToken));

                if (token.Type == TokenType.EOL && curlyCount == 0 && !isInsideConditionChain)
                {
                    break;
                }
            }

            return lineTokens;
        }

        private static bool IsConditionChainToken(Token token)
        {
            return token != null &&
                  (token.Type == TokenType.IF ||
                   token.Type == TokenType.ELSE);
        }

        private static bool IsNewCondition(Token token, Token prevToken)
        {
            return (token.Type == TokenType.IF && (prevToken == null || prevToken.Type != TokenType.ELSE)) ||
                   (token.Type == TokenType.ELSE && prevToken == null);
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

                Traceback.Instance.ThrowException(error,lastOpenToken.Column - 1);
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
