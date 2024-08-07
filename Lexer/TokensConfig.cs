﻿using System.Collections.Generic;

namespace BoomifyCS.Lexer
{
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
            {"null", TokenType.NULL},
            {"true", TokenType.TRUE},
            {"false", TokenType.FALSE},
            { "return", TokenType.RETURN },
            { "function",TokenType.FUNCTIONDECL },
            {"**", TokenType.POW},
            { "<=", TokenType.LTE },
            { ">=", TokenType.GTE },
            { "+=", TokenType.ADDE },
            { "-=", TokenType.SUBE },
            { "*=", TokenType.MULE },
            { "/=", TokenType.IDIV },
            { "//=", TokenType.FLOORDIVE },
            { "**=", TokenType.POWE },
            { "==", TokenType.EQ },
            { "--", TokenType.INCREMENT },
            { "++",TokenType.DECREMENT},
            { "!=", TokenType.NEQ },
            { "&&", TokenType.AND },


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
            { '+', TokenType.ADD },
            { '-', TokenType.SUB },
            { '*', TokenType.MUL },
            { '/', TokenType.DIV },
            { '%', TokenType.MOD },
            { '<', TokenType.LT },
            { '>', TokenType.GT },
            { '!', TokenType.NEQ },
        };
        public static Dictionary<string, TokenType> binaryOperators = new Dictionary<string, TokenType>
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
            { "--", TokenType.DECREMENT },
            { "++", TokenType.INCREMENT },
            { ",", TokenType.COMMA }
        };

        public static Dictionary<string, TokenType> comparisonOperators = new Dictionary<string, TokenType>
        {
            { "==", TokenType.EQ },
            { "!=", TokenType.NEQ },
        };

        public static Dictionary<string, TokenType> assignmentOperators = new Dictionary<string, TokenType>
        {
            { "=", TokenType.ASSIGN },
            { "+=", TokenType.ADDE },
            { "-=", TokenType.SUBE },
            { "*=", TokenType.MULE },
            { "/=", TokenType.IDIV },
            { "//=", TokenType.FLOORDIVE },
            { "**=", TokenType.POWE },
            { "--", TokenType.INCREMENT },
            { "++",TokenType.DECREMENT},
        };
        public static Dictionary<string, TokenType> multiTokenStatements = new Dictionary<string, TokenType>
        {
            { "if", TokenType.IF },
            { "else", TokenType.ELSE },
            { "while", TokenType.WHILE },
            { "for", TokenType.FOR },
            { "var", TokenType.VARDECL },
            { "return", TokenType.RETURN },
            { "function",TokenType.FUNCTIONDECL },
            { "+=", TokenType.ADDE },
            { "-=", TokenType.SUBE },
            { "*=", TokenType.MULE },
            { "/=", TokenType.IDIV },
            { "//=", TokenType.FLOORDIVE },
            { "**=", TokenType.POWE },
            { "--", TokenType.INCREMENT },
            { "++",TokenType.DECREMENT},
            { "identifier", TokenType.IDENTIFIER },
        };
    }
}
