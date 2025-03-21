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
        public static List<List<Token>> SplitLines(List<Token> tokens)
        {
            List<List<Token>> lines = new();
            List<Token> currentLine = new();
            int curlyCount = 0;
            bool isInsideConditionChain = false;

            for (int tokenIndex = 0; tokenIndex < tokens.Count; tokenIndex++)
            {
                Token token = tokens[tokenIndex];
                if (token.Type == TokenType.LCUR)
                {
                    List<Token> tokensInCur = GetTokensBetween(tokens,ref tokenIndex,TokenType.LCUR,TokenType.RCUR);
                    Token nextToken = GetTokenOrNull(tokens, tokenIndex + 1);
                    currentLine.Add(new Token(TokenType.LCUR,"{"));
                    currentLine.AddRange(tokensInCur);
                    currentLine.Add(new Token(TokenType.RCUR, "}"));

                    if (nextToken != null && nextToken.Type != TokenType.ELSE)
                    {
                        lines.Add(new List<Token>(currentLine));
                        currentLine = new();
                    }
                }
                else if (token.Type == TokenType.EOL)
                {
                    if (currentLine.Count > 0)
                    {
                        lines.Add(new List<Token>(currentLine));
                        currentLine = new();
                    }
                }
              
                else
                {
                    currentLine.Add(token);
                }
                

            }

            if (currentLine.Count > 0)
            {
                lines.Add(new List<Token>(currentLine));
            }

            return lines;
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
