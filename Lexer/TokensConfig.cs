using System.Collections.Generic;

namespace BoomifyCS.Lexer
{
    public class TokenConfig
    {
        public readonly static Dictionary<string, TokenType> multiCharTokens = new()
        {
            { "if", TokenType.IF },

            { "else", TokenType.ELSE },
            { "while", TokenType.WHILE },
            { "for", TokenType.FOR },
            { "break", TokenType.BREAK },
            { "continue", TokenType.CONTINUE },
            { "//", TokenType.FLOORDIV },
            { "return", TokenType.RETURN },
            {"**", TokenType.POW},
            { "<=", TokenType.LTEQ },
            { ">=", TokenType.GTEQ },
            { "+=", TokenType.ADDE },
            { "-=", TokenType.SUBE },
            { "*=", TokenType.MULE },
            { "/=", TokenType.DIVE },
            { "//=", TokenType.FLOORDIVE },
            { "**=", TokenType.POWE },
            { "==", TokenType.EQ },
            { "--", TokenType.DECREMENT},
            { "++",TokenType.INCREMENT },
            { "!=", TokenType.NEQ },
            { "&&", TokenType.AND },
            { "||", TokenType.OR},
            { "!!",TokenType.COMMENT}


        };

        public readonly static Dictionary<char, TokenType> singleCharTokens = new()
        {
            { '=', TokenType.ASSIGN },
            { ';', TokenType.SEMICOLON },
            { ',', TokenType.COMMA },
            { '(', TokenType.LPAREN },
            { ')', TokenType.RPAREN },
            { '[', TokenType.LBRACKET },
            { ']', TokenType.RBRACKET },
            { '{', TokenType.LCUR },
            { '}', TokenType.RCUR },


            { ':', TokenType.COLON },
            { '.', TokenType.DOT },
            { '+', TokenType.ADD },
            { '-', TokenType.SUB },
            { '*', TokenType.MUL },
            { '/', TokenType.DIV },
            { '%', TokenType.MOD },
            { '<', TokenType.LT },
            { '>', TokenType.GT },
            { '!', TokenType.NOT },
        };
        public readonly static Dictionary<string, TokenType> binaryOperators = new()
        {
            { "+", TokenType.ADD },
            { "-", TokenType.SUB },
            { "*", TokenType.MUL },
            { "/", TokenType.DIV },
            { "//", TokenType.FLOORDIV },
            { "%", TokenType.MOD },
            { "**", TokenType.POW },
            { "<<", TokenType.LSHIFT },
            { ">>", TokenType.RSHIFT },
            { "&", TokenType.BIT_AND },
            { "|", TokenType.BIT_OR },
            { "^", TokenType.BIT_XOR },
            { "==", TokenType.EQ },
            { "!=", TokenType.NEQ },
            { "<", TokenType.LT },
            { ">", TokenType.GT },
            { "<=", TokenType.LTEQ },
            { ">=", TokenType.GTEQ },
            { "&&", TokenType.AND },
            { "||", TokenType.OR },
            { "!", TokenType.NOT },
            { ",", TokenType.COMMA }
        };

        public readonly static Dictionary<string, TokenType> comparisonOperators = new()
        {
            { "==", TokenType.EQ },
            { "!=", TokenType.NEQ },
        };

        public readonly static Dictionary<string, TokenType> assignmentOperators = new()
        {
            { "=", TokenType.ASSIGN },
            { "+=", TokenType.ADDE },
            { "-=", TokenType.SUBE },
            { "*=", TokenType.MULE },
            { "/=", TokenType.DIVE },
            { "//=", TokenType.FLOORDIVE },
            { "**=", TokenType.POWE },

        };
        public readonly static Dictionary<string, TokenType> multiTokenStatements = new()
        {
            { "if", TokenType.IF },
            { "else", TokenType.ELSE },
            { "while", TokenType.WHILE },
            { "for", TokenType.FOR },
            { "return", TokenType.RETURN },
            { "function",TokenType.FUNCTIONDECL },
            { "+=", TokenType.ADDE },
            { "-=", TokenType.SUBE },
            { "*=", TokenType.MULE },
            { "/=", TokenType.DIVE },
            { "//=", TokenType.FLOORDIVE },
            { "**=", TokenType.POWE },
            { "--", TokenType.DECREMENT},
            { "++",TokenType.INCREMENT},
            { "identifier", TokenType.IDENTIFIER },
            { "=", TokenType.ASSIGN},
            { "[",TokenType.LBRACKET },
        };
    }
}
