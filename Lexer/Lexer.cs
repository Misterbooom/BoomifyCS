using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoomifyCS.Lexer
{
    public class MyLexer
    {
        private int _position;
        private string _code;
        public MyLexer(string code)
        {
            this._code = code;
            _position = 0;
        }
        public List<Token> Tokenize()
        {
            List<Token> tokens = new List<Token>();
            while (_position < _code.Length)
            {
                char currentChar = _code[_position];
                
                if (currentChar == ';')
                {
                    tokens.Add(new Token(TokenType.EOL, ";"));
                    _position++;
                    continue;

                }
                if (char.IsWhiteSpace(currentChar))
                {
                    Token token = new Token(TokenType.WHITESPACE, " ");
                    tokens.Add(token);
                    _position++;
                    continue;

                }

                KeyValuePair<string, TokenType> multichar = GenerateMultiChar();
                  
                if (currentChar == '"' || currentChar.ToString() == "'")
                {
                    string str = GenerateString();
                    Token token = new Token(TokenType.STRING, str);
                    tokens.Add(token);
                }
                else if (currentChar == '[')
                {
                    _position++;
                    string array = GenerateArray();
                    Token token = new Token(TokenType.ARRAY, array);
                    tokens.Add(token);
                }
                else if (currentChar == '{')
                {
                    Token token = GenerateObject();

                    tokens.Add(token);

                }
                else if (TokenConfig.singleCharTokens.TryGetValue(currentChar, out TokenType tokenType))
                {
                    Token token = new Token(tokenType, currentChar.ToString());
                    tokens.Add(token);
                }
                else if (char.IsDigit(currentChar))
                {
                    string digit = GenerateDigit();
                    Token token = new Token(TokenType.NUMBER, digit);
                    tokens.Add(token);
                }

                else if (multichar.Key != " ")
                {

                    Token token = new Token(multichar.Value, multichar.Key);
                    tokens.Add(token);
                }
                else if (IsIdentifier(currentChar))
                {
                    string identifier = GenerateIdentifier();
                    Token token = new Token(TokenType.IDENTIFIER, identifier);
                    tokens.Add(token);
                }

                _position++;
            }

            return tokens;
        }
        public bool IsIdentifier(char identifier)
        {
            return identifier == '_' || char.IsLetter(identifier);
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

                if (currentChar != '.' && !char.IsDigit(currentChar))
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
            string multiChar = "";
            string match = "";

            foreach (KeyValuePair<string, TokenType> kvp in TokenConfig.multiCharTokens)
            {
                if (_position + kvp.Key.Length <= _code.Length)
                {
                    match = _code.Substring(_position, kvp.Key.Length);
                    if (match == kvp.Key)
                    {
                        multiChar = match;
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
                        break;
                    }
                }
                else
                {
                    str += currentChar;
                }
                _position++;
            }
            return str;

        }
        public string GenerateArray()
        {
            string array = "";
            while (_position < _code.Length)
            {
                char currentChar = _code[_position];
                if (currentChar == ']')
                {
                    break;
                }
                array += currentChar;
                _position++;
            }
            return array;
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
            StringBuilder block = new StringBuilder();
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
                        throw new InvalidOperationException("Unmatched '}' found.");
                    }
                    else if (counter == 0)
                    {
                        block[0] = ' ';
                        break;
                    }
                }

                block.Append(currentChar);
                _position++;
            }

            if (counter != 0)
            {
                throw new InvalidOperationException("Unmatched '{' found.");
            }
            string block_string = block.ToString();
            return new Token(TokenType.OBJECT,block_string);
        }

    }
}
