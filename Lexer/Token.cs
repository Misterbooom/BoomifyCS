using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoomifyCS.Lexer
{
    public enum TokenType
    {
        CALL,
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
        ADDE,
        SUBE,
        MULE,
        DIVE,
        FLOORDIVE,
        POWE,
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
        BLOCK,
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
        ELSEIF,
        DECREMENT,
        INCREMENT,
        FUNCTIONDECL,
        NEXTLINE,
        BITAND,
        BITOR,
        BITXOR,
        BITNOT,
        BREAK,
        CONTINUE,
        TRY,
        CATCH,
        FINALLY,
        THROW,
    }
    public class Token(TokenType type, string value, List<Token> tokens = null)
    {
        public TokenType Type = type;
        public string Value = value;
        public List<Token> Tokens = tokens;

        public override string ToString()
        {
            //if (Tokens != null)
            //{
            //    return $"Token(type = '{Type}',value = '{Value} tokens - {Tokens.TokensToString()}')";

            //}
            return $"Token(type = '{Type}',value = '{Value}')";
        }

    }



}

