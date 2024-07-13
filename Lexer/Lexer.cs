using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;
namespace BoomifyCS.Lexer
{
    public class MyLexer
    {
        private int _position;
        private string _code;
        private string _currentLine;
        private int _lineCount = 0;
        private string[] _lines;
        public MyLexer(string code)
        {
            this._code = code;

            List<string> tempLines = new List<string>();

            string[] linesArray = Regex.Split(code, "\r?\n");

            foreach (string line in linesArray)
            {
                if (!string.IsNullOrWhiteSpace(line)) 
                {
                    tempLines.Add(line); 
                }
            }

            _lines = tempLines.ToArray();

            if (_lines.Length > 1)
            {
                _currentLine = _lines[1]; 
            }
            else
            {
                _currentLine = _lines[0]; 
            }
            _position = 0; 
        }
    

        public List<Token> Tokenize()
        {
            List<Token> tokens = new List<Token>();
            bool lastTokenWasSemicolon = false;
            List<int> bracketsStack = new List<int>();
            while (_position < _code.Length)
            {
                char currentChar = _code[_position];


                if (currentChar == ';')
                {
                    tokens.Add(new Token(TokenType.EOL, ";"));
                    lastTokenWasSemicolon = true;
                    _position++;
                    continue;
                }
                if (currentChar == '(')
                {
                    bracketsStack.Add(_lineCount);
                }
                else if (currentChar == ')') 
                {
                    if (bracketsStack.Count == 0) 
                    { 
                        throw new BifySyntaxError("Unmatched ')': missing '('.",_lines[_lineCount], ")", _lineCount);

                    }
                    bracketsStack.Pop();
                }

                if (char.IsWhiteSpace(currentChar))
                {
                    if (currentChar == '\n')
                    {
                        if (lastTokenWasSemicolon)
                        {
                            tokens.Add(new Token(TokenType.EOL, "\n"));
                        }
                        else
                        {
                            tokens.Add(new Token(TokenType.WHITESPACE, "\n"));
                        }
                        lastTokenWasSemicolon = false; 
                        _lineCount++;
                        _currentLine = string.Empty;
                        continue;
                    }
                    else if (currentChar == '\r')
                    {
                        if (_position + 1 < _code.Length && _code[_position + 1] == '\n')
                        {
                            if (lastTokenWasSemicolon)
                            {
                                tokens.Add(new Token(TokenType.EOL, "\r\n"));
                            }
                            else
                            {
                                tokens.Add(new Token(TokenType.WHITESPACE, "\r\n"));
                            }
                            _position++;
                            _currentLine = _lines[_lineCount];
                        }
                        else
                        {
                            if (lastTokenWasSemicolon)
                            {
                                tokens.Add(new Token(TokenType.EOL, "\r"));
                            }
                            else
                            {
                                tokens.Add(new Token(TokenType.WHITESPACE, "\r"));
                            }
                        }
                        lastTokenWasSemicolon = false; 
                        _lineCount++;
                        _currentLine = string.Empty; 
                    }
                    else
                    {
                        tokens.Add(new Token(TokenType.WHITESPACE, currentChar.ToString()));
                        lastTokenWasSemicolon = false;
                    }
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
                else if (TokenConfig.singleCharTokens.TryGetValue(currentChar, out TokenType tokenType))
                {
                    Token token = new Token(tokenType, currentChar.ToString());
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
            if (bracketsStack.Count > 0)
            {
                throw new BifySyntaxError("Unmatched '(': missing ')'.",_lines[bracketsStack[0]], "(",bracketsStack[0]);
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
                        counter--;
                        break;
                    }
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
                    "Syntax Error: You forgot to close a quotation mark.",
                    _currentLine, 
                    _currentLine, 
                    _lineCount 
                );

            }
            return str;

        }
        public string GenerateArray()
        {
            string array = "";
            int counter = 0;
            while (_position < _code.Length)
            {
                char currentChar = _code[_position];
                if (currentChar == '[')
                {
                    counter++;
                }
                else if (currentChar == '['){
                    counter--;
                    if (counter == 0)
                    {
                        break;
                    }
                }
                else
                {
                    array += currentChar;
                }
                _position++;
            }
            if (counter > 0)
            {
                int _lineCount = 42; // Example line count
                // Use the appropriate constructor based on the type of parameters
                throw new BifySyntaxError(
                    "Syntax Error: You forgot to close '[' with ']'.",
                    _currentLine, // This should be a string representing tokens
                    _currentLine, // This should be a string representing invalid tokens
                    _lineCount // Line number
                );

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
            MyLexer lexer = new MyLexer(block_string.Trim());
            List<Token> tokens = lexer.Tokenize();
            return new Token(TokenType.OBJECT,block_string,tokens);
        }

    }
}
