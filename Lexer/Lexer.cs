using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using BoomifyCS.Exceptions;

namespace BoomifyCS.Lexer
{
    public partial class MyLexer
    {
        private int _position;
        private readonly string _code;
        private readonly string _currentLine;
        private int _lineCount = 1;
        private readonly string[] _lines;
        public MyLexer(string code)
        {
            this._code = code;

            _lines = _code.Split("\n");
            _position = 0;
        }


        public List<Token> Tokenize()
        {
            List<Token> tokens = [];
            Stack<int> parenthesesStack = new();
            Stack<int> squareBracketsStack = new();
            Stack<int> curlyBracesStack = new();

            while (_position < _code.Length)
            {
                char currentChar = _code[_position];
                KeyValuePair<string, TokenType> multichar = GenerateMultiChar();
                if (currentChar == ' ')
                {
                    _position++;
                    continue;
                }
                else if (currentChar == '\n')
                {
                    tokens.Add(new Token(TokenType.NEXTLINE, "\n"));
                    _lineCount++;
                    _position++;
                    continue;
                }

                else if (currentChar == ';')
                {
                    if (parenthesesStack.Count == 0 && squareBracketsStack.Count == 0 && curlyBracesStack.Count == 0)
                    {
                        tokens.Add(new Token(TokenType.EOL, ";"));
                    }
                    else
                    {
                        tokens.Add(new Token(TokenType.SEMICOLON, ";"));
                    }
                    _position++;
                    continue;
                }

                // Handle parentheses
                else if (currentChar == '(')
                {
                    parenthesesStack.Push(_lineCount);
                }
                else if (currentChar == ')')
                {
                    if (parenthesesStack.Count == 0)
                    {
                        throw new BifySyntaxError(ErrorMessage.UnmatchedClosingParenthesis(), _lines[_lineCount], ")", _lineCount);
                    }
                    parenthesesStack.Pop();
                }

                // Handle square brackets
                else if (currentChar == '[')
                {
                    squareBracketsStack.Push(_lineCount);
                }
                else if (currentChar == ']')
                {
                    if (squareBracketsStack.Count == 0)
                    {
                        throw new BifySyntaxError(ErrorMessage.UnmatchedClosingBracket(), _lines[_lineCount], "]", _lineCount);
                    }
                    squareBracketsStack.Pop();
                }


                else if (currentChar == '}')
                {
                    if (curlyBracesStack.Count == 0)
                    {
                        throw new BifySyntaxError(ErrorMessage.UnmatchedClosingBrace(), _lines[_lineCount], "}", _lineCount);
                    }
                    curlyBracesStack.Pop();
                }

                else if (currentChar == '"' || currentChar == '\'')
                {
                    string str = GenerateString();
                    tokens.Add(new Token(TokenType.STRING, str));
                }
                else if (currentChar == '{')
                {
                    tokens.Add(GenerateObject());


                }
                else if (currentChar == '-' && char.IsDigit(_code[_position + 1]))
                {
                    string digit = GenerateDigit();
                    tokens.Add(new Token(TokenType.NUMBER, digit));
                }
                else if (char.IsDigit(currentChar))
                {
                    string digit = GenerateDigit();
                    tokens.Add(new Token(TokenType.NUMBER, digit));
                }
                
                else if (multichar.Key != " ")
                {
                    if (multichar.Value == TokenType.COMMENT) {
                        SkipComment();
                        continue;
                    }
                    tokens.Add(new Token(multichar.Value, multichar.Key));
                }
                else if (TokenConfig.singleCharTokens.TryGetValue(currentChar, out TokenType tokenType))
                {
                    tokens.Add(new Token(tokenType, currentChar.ToString()));
                }
                else if (IsIdentifier(currentChar))
                {
                    string identifier = GenerateIdentifier();
                    tokens.Add(new Token(TokenType.IDENTIFIER, identifier));
                }
                else
                {
                    throw new BifySyntaxError(ErrorMessage.UnexpectedToken(_code[_position].ToString()), _lines[_lineCount - 1], _code[_position].ToString(), _lineCount);
                }
                _position++;
            }

            // Check for unmatched opening brackets
            if (parenthesesStack.Count > 0)
            {
                int line = parenthesesStack.Pop();
                throw new BifySyntaxError(ErrorMessage.UnmatchedOpeningParenthesis(), _lines[line - 1], "(", line);
            }

            if (squareBracketsStack.Count > 0)
            {
                int line = squareBracketsStack.Pop();
                throw new BifySyntaxError(ErrorMessage.UnmatchedOpeningBracket(), _lines[line - 1], "[", line);
            }

            if (curlyBracesStack.Count > 0)
            {
                int line = curlyBracesStack.Pop();
                throw new BifySyntaxError(ErrorMessage.UnmatchedOpeningBrace(), _lines[line - 1], "{", line);
            }

            return tokens;
        }

        public static bool IsIdentifier(char identifier)
        {
            return identifier == '_' || char.IsLetter(identifier);
        }
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
                    _position++;
                    continue;
                }

                if (currentChar == '.' && dotCount > 0)
                {
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
            foreach (KeyValuePair<string, TokenType> kvp in TokenConfig.multiCharTokens)
            {
                if (_position + kvp.Key.Length <= _code.Length)
                {
                    string match = _code.Substring(_position, kvp.Key.Length);
                    if (match == kvp.Key)
                    {
                        _position += match.Length - 1;
                        return kvp;
                    }
                }
            }
            return new KeyValuePair<string, TokenType>(" ", TokenType.WHITESPACE);
        }

        public string GenerateString()
        {
            string str = "";
            char stringChar = ' ';
            int counter = 0;
            while (_position < _code.Length)
            {
                char currentChar = _code[_position];
                if (currentChar == '"' || currentChar.ToString() == "'")
                {
                    if (counter == 0)
                    {
                        stringChar = currentChar;
                        counter++;
                    }
                    else if (currentChar == stringChar)
                    {
                        counter--;
                        break;
                    }
                }
                else if (currentChar == '\n')
                {
                    break;
                }
                else
                {
                    str += currentChar;
                }
                _position++;
            }
            if (counter > 0)
            {
                throw new BifySyntaxError(
                    ErrorMessage.MissingCloseQuotationMark(),
                    _currentLine,
                    _currentLine,
                    _lineCount - 1
                );

            }
            return str;

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
                        throw new BifySyntaxError(ErrorMessage.UnmatchedClosingBrace(), _lines[_lineCount], "}", _lineCount);

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
                throw new BifySyntaxError(ErrorMessage.UnmatchedOpeningBrace(), _lines[_lineCount], "{", _lineCount - 1);

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
