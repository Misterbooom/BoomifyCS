using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoomifyCS.Lexer
{
    public enum TokenType
    {
        IDENTIFIER,
        NUMBER,
        STRING,
        SEMICOLON,
        COMMA,
        EOL,
        VARDECL,
        ASSIGN,
        IF,
        ELSE,
        WHILE,
        FOR,
        ADD,
        SUB,
        MUL,
        DIV,
        FLOORDIV,
        POW,
        MOD,
        LT,
        GT,
        LTE,
        GTE,
        IADD,
        ISUB,
        IMUL,
        IDIV,
        IFLOORDIV,
        IPOW,
        LPAREN,
        RPAREN,
        LBRACKET,
        RBRACKET,
        COLON,
        DOT,
        WHITESPACE,
        LSQUARE,
        RSQUARE,
        LCUR,
        RCUR,
        EQ,
        NEQ,
        ARRAY,
        OBJECT,
    }
    public class Token
    {
        public TokenType Type;
        public string Value;
        public List<Token> Tokens;
        public Token(TokenType type, string value, List<Token> tokens = null)
        {
            this.Type = type;
            this.Value = value;
            this.Tokens = tokens;
        }
        public override string ToString()
        {
            return $"Token(type = '{Type}',value = '{Value}')";
        }

    }
    public class TokenConfig
    {

        public static Dictionary<string, TokenType> multiCharTokens = new Dictionary<string, TokenType>
        {
            { "if", TokenType.IF },
            { "else", TokenType.ELSE },
            { "while", TokenType.WHILE },
            { "for", TokenType.FOR },
            { "var", TokenType.VARDECL },
            { "//", TokenType.FLOORDIV },
            { "**", TokenType.POW },
            { "<=", TokenType.LTE },
            { ">=", TokenType.GTE },
            { "+=", TokenType.IADD },
            { "-=", TokenType.ISUB },
            { "*=", TokenType.IMUL },
            { "/=", TokenType.IDIV },
            { "//=", TokenType.IFLOORDIV },
            { "**=", TokenType.IPOW },
            { "==",TokenType.EQ}
        };
        public static Dictionary<char, TokenType> singleCharTokens = new Dictionary<char, TokenType>
        {
            { '=', TokenType.ASSIGN },
            { ';', TokenType.SEMICOLON },
            { ',', TokenType.COMMA },
            { '(', TokenType.LPAREN },
            { ')', TokenType.RPAREN },
            { '[', TokenType.LBRACKET },
            { ']', TokenType.RBRACKET },
            { ':', TokenType.COLON },
            { '.', TokenType.DOT },
            {'+', TokenType.ADD},
            {'-', TokenType.SUB},
            {'*', TokenType.MUL},
            {'/', TokenType.DIV},
            {'%', TokenType.MOD},
            {'<', TokenType.LT},
            {'>',TokenType.GT},
            {' ',TokenType.WHITESPACE },
            {'!',TokenType.NEQ },
        };
    }


}

