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
        NULL,
        TRUE,
        FALSE,
        FUNCTION,
        RETURN,
        GTEQ,
        LTEQ,
        EQEQ,
        LSHIFT,
        RSHIFT,
        BIT_AND,
        BIT_OR,
        BIT_XOR,
        AND,
        OR,
        NOT,
        BIT_NOT,
        BIT_NEG,
        INCR,
        DECR,
        
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



}

