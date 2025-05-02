using System.Collections.Generic;
using System.Data.Common;
using System;
using System.Text;
using System.Text.RegularExpressions;
using BoomifyCS.Exceptions;
using System.Linq;

namespace BoomifyCS.Lexer
{
    public partial class MyLexer
    {
        private int _position;
        private readonly string _code;
        private int _lineCount = 1;
        private readonly string[] _lines;
        int column = 1; // Initialize column counter

        List<Token> tokens = [];

        public MyLexer(string code)
        {
            this._code = code;
            Traceback.Instance.InitializeSource(_code.Split("\n"));
            _lines = _code.Split("\n");
            _position = 0;

        }


        public List<Token> Tokenize()
        {
            Stack<int> parenthesesStack = new();
            Stack<int> squareBracketsStack = new();
            Stack<int> curlyBracesStack = new();

            while (_position < _code.Length)
            {
                char currentChar = _code[_position];
                int tempPos = _position;
                KeyValuePair<string, TokenType> multichar = GenerateMultiChar();
                _position = tempPos;
                if (currentChar == '\n')
                {
                    _lineCount++;
                    _position++;
                    ResetColumn(); // Reset column to 1 for the new Line
                    continue;
                }
                if (char.IsWhiteSpace(currentChar))
                {
                    UpdateColumn(currentChar);
                    _position++;
                    continue;
                }

                // Handle parentheses
                if (currentChar == '(')
                {
                    parenthesesStack.Push(_lineCount);
                }
                else if (currentChar == ')')
                {
                    if (parenthesesStack.Count == 0)
                    {
                        Traceback.Instance.ThrowException(new BifySyntaxError(ErrorMessage.UnmatchedClosingParenthesis(), "", ")"), column);
                    }
                    parenthesesStack.Pop();
                }


                else if (currentChar == ';')
                {
                    if (parenthesesStack.Count == 0 && squareBracketsStack.Count == 0 && curlyBracesStack.Count == 0)
                    {
                        AddToken(new Token(TokenType.EOL, ";"));
                    }
                    else
                    {
                        AddToken(new Token(TokenType.SEMICOLON, ";"));
                    }
                    UpdateColumn(currentChar);
                    _position++;
                    continue;
                }
                if (currentChar == '"' || currentChar == '\'')
                {
                    string str = GenerateString();
                    AddToken(new Token(currentChar == '"' ? TokenType.STRING : TokenType.CHAR, str));
                    UpdateColumn(str.Length);
                }
                else if (currentChar == '-' && char.IsDigit(_code[_position + 1]))
                {
                    string digit = GenerateDigit();
                    AddToken(new Token(TokenType.NUMBER, digit));
                    UpdateColumn(digit.Length);
                }
                else if (char.IsDigit(currentChar))
                {
                    string digit = GenerateDigit();
                    AddToken(new Token(TokenType.NUMBER, digit));
                    UpdateColumn(digit.Length);
                }
                else if (IsIdentifier(currentChar))
                {
                    string identifier = GenerateIdentifier();
                    if (TokenConfig.multiCharTokens.TryGetValue(identifier, out TokenType tokenType))
                    {
                        AddToken(new Token(tokenType, identifier));
                    }
                    else
                    {
                        AddToken(new Token(TokenType.IDENTIFIER, identifier));
                    }
                    UpdateColumn(identifier.Length);


                }
                else if (multichar.Key != " ")
                {
                    if (multichar.Value == TokenType.COMMENT)
                    {
                        SkipComment();
                        continue;
                    }
                    AddToken(new Token(multichar.Value, multichar.Key));
                    _position += multichar.Key.Length - 1;  
                    UpdateColumn(multichar.Key.Length);
                }
               
                else if (TokenConfig.singleCharTokens.TryGetValue(currentChar, out TokenType tokenType))
                {
                    AddToken(new Token(tokenType, currentChar.ToString()));
                    UpdateColumn(currentChar);
                }

                else
                {
                    if (!char.IsWhiteSpace(currentChar))
                        Traceback.Instance.ThrowException(new BifySyntaxError(ErrorMessage.UnexpectedToken(currentChar.ToString()), _lines[_lineCount - 1], _code[_position].ToString(), _lineCount));
                }

                _position++;
                Traceback.Instance.SetCurrentLine(_lineCount);
            }
            return tokens;
        }
        private void ResetColumn() => column = 1;
        private void UpdateColumn(char currentChar) => column++;
        private void UpdateColumn(int length = 1) => column += length;

        private void AddToken(Token token)
        {
            token.Line = _lineCount;
            token.Column = column;
            tokens.Add(token);
        }


        public static bool IsIdentifier(char identifier) => identifier == '_' || char.IsLetter(identifier) || char.IsDigit(identifier);
        public void SkipComment()
        {
            while (_position < _code.Length && _code[_position] != '\n')
            {
                _position++;
            }
        }
        public string GenerateDigit()
        {
            string digit = "";
            int dotCount = 0;

            while (_position < _code.Length)
            {
                char currentChar = _code[_position];

                if (char.IsWhiteSpace(currentChar))
                {
                    break;
                }

                if (currentChar == '.' && dotCount > 0)
                {
                    _position--;
                    break;
                }

                if (currentChar != '.' && !char.IsDigit(currentChar) && currentChar != '-')
                {
                    break;
                }

                if (currentChar == '.')
                {
                    dotCount++;
                }

                digit += currentChar;
                _position++;
            }

            _position--;

            return digit;


        }
        public KeyValuePair<string, TokenType> GenerateMultiChar()
        {
            KeyValuePair<string, TokenType>? longestMatch = null;
            int startPosition = _position;

            foreach (KeyValuePair<string, TokenType> kvp in TokenConfig.multiCharTokens)
            {
                if (_position + kvp.Key.Length <= _code.Length)
                {
                    string match = _code.Substring(_position, kvp.Key.Length);
                    if (match == kvp.Key)
                    {
                        if (longestMatch == null || kvp.Key.Length > longestMatch.Value.Key.Length)
                        {
                            longestMatch = kvp;
                        }
                    }
                }
            }

            if (longestMatch != null)
            {
                string match = longestMatch.Value.Key;
                _position += longestMatch.Value.Key.Length - 1;
                return longestMatch.Value;
            }

            return new KeyValuePair<string, TokenType>(" ", TokenType.WHITESPACE);
        }


        public string GenerateString()
        {
            var result = new System.Text.StringBuilder();

            char quoteChar = _code[_position];
            _position++;

            while (_position < _code.Length)
            {
                char currentChar = _code[_position];

                if (currentChar == quoteChar)
                {
                    return result.ToString();
                }

                if (currentChar == '\\')
                {
                    _position++;
                    if (_position >= _code.Length)
                        break;

                    char escapeChar = _code[_position];
                    switch (escapeChar)
                    {
                        case 'n': result.Append('\n'); break;
                        case 't': result.Append('\t'); break;
                        case 'r': result.Append('\r'); break;
                        case '\\': result.Append('\\'); break;
                        case '"': result.Append('"'); break;
                        case '\'': result.Append('\''); break;
                        case '0': result.Append('\0'); break;
                        default:
                            result.Append(escapeChar);
                            break;
                    }
                }
                else if (currentChar == '\n')
                {
                    break;
                }
                else
                {
                    result.Append(currentChar);
                }
                _position++;
            }

            Traceback.Instance.ThrowException(new BifySyntaxError(
                ErrorMessage.MissingCloseQuotationMark(),
                _lines[_lineCount - 1],
                quoteChar.ToString(),
                _lineCount
            ));
            return result.ToString();
        }


        public string GenerateIdentifier()
        {
            string identifier = "";
            while (_position < _code.Length)
            {
                char currentChar = (_code[_position]);
                if (IsIdentifier(currentChar))
                {
                    identifier += currentChar;
                    _position++;
                }
                else
                {
                    break;
                }
            }
            _position--;
            return identifier;
        }
        public Token GenerateObject()
        {
            int counter = 0;
            StringBuilder block = new();
            int start = _position + 1;
            while (_position < _code.Length)
            {
                char currentChar = _code[_position];

                if (currentChar == '{')
                {
                    counter++;
                }
                else if (currentChar == '}')
                {
                    counter--;
                    if (counter < 0)
                    {
                        Traceback.Instance.ThrowException(new BifySyntaxError(ErrorMessage.UnmatchedClosingBrace(), _lines[_lineCount - 1], "}", _lineCount));

                    }
                    else if (counter == 0)
                    {
                        block[0] = ' ';
                        break;

                    }


                }
                else if (currentChar == '\n')
                {
                    _lineCount++;
                }

                block.Append(currentChar);
                _position++;
            }

            if (counter != 0)
            {
                Traceback.Instance.ThrowException(new BifySyntaxError(ErrorMessage.UnmatchedOpeningBrace(), _lines[_lineCount - 1], "{", _lineCount - 1));

            }
            string blockString = _code[start.._position];
            MyLexer lexer = new(block.ToString());
            List<Token> tokens = lexer.Tokenize();
            _position += 1;
            return new Token(TokenType.BLOCK, blockString, tokens);




        }

        [GeneratedRegex("\r?\n")]
        private static partial Regex MyRegex();
    }
}
