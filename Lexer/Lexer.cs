using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using BoomifyCS.Exceptions;

namespace BoomifyCS.Lexer
{
    public class MyLexer
    {
        private int _position;
        private readonly string _code;
        private readonly string _currentLine;
        private int _lineCount = 0;
        private readonly string[] _lines;
        public MyLexer(string code)
        {
            this._code = code;

            List<string> tempLines = [];

            string[] linesArray = Regex.Split(code, "\r?\n");

            foreach (string line in linesArray)
            {
                tempLines.Add(line); 
                
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
            List<Token> tokens = [];
            List<int> bracketsStack = [];
            while (_position < _code.Length)
            {

                char currentChar = _code[_position];


                
                if (currentChar == '\n')
                {
                    tokens.Add(new Token(TokenType.NEXTLINE, "\n"));
                    _lineCount++;
                    _position++;
                    continue;
                }
                
                

                if (currentChar == ';')
                {
                    if (bracketsStack.Count == 0)
                    {
                        tokens.Add(new Token(TokenType.EOL, ";"));
                    }
                    else
                    {
                        tokens.Add(new Token(TokenType.SEMICOLON,";"));
                    }
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


                KeyValuePair<string, TokenType> multichar = GenerateMultiChar();
                  
                if (currentChar == '"' || currentChar.ToString() == "'")
                {
                    string str = GenerateString();
                    Token token = new(TokenType.STRING, str);
                    tokens.Add(token);
                }
                else if (currentChar == '[')
                {
                    _position++;
                    string array = GenerateArray();
                    Token token = new(TokenType.ARRAY, array);
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
                    Token token = new(TokenType.NUMBER, digit);
                    tokens.Add(token);
                }

                else if (multichar.Key != " ")
                {

                    Token token = new(multichar.Value, multichar.Key);
                    tokens.Add(token);
                }
                else if (TokenConfig.singleCharTokens.TryGetValue(currentChar, out TokenType tokenType))
                {
                    Token token = new(tokenType, currentChar.ToString());
                    tokens.Add(token);
                }
                else if (IsIdentifier(currentChar))
                {
                    string identifier = GenerateIdentifier();
                    Token token = new(TokenType.IDENTIFIER, identifier);
                    tokens.Add(token);
                }

                _position++;
            }
            if (bracketsStack.Count > 0)
            {
                int line = bracketsStack[bracketsStack.Count - 1];
                throw new BifySyntaxError("Unmatched '(': missing ')'.",_lines[line], "(",line);
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
                        throw new InvalidOperationException("Unmatched '}' found.");
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
                throw new InvalidOperationException("Unmatched '{' found.");
            }
            string blockString = _code.Substring(start, _position - start);
            MyLexer lexer = new(block.ToString());
            List<Token> tokens = lexer.Tokenize();
            int length = _code.Length - _position;
            _position += 1;
            return new Token(TokenType.OBJECT, blockString, tokens);


            
            
        }

    }
}
