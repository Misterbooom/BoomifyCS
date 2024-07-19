﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomifyCS.Lexer;
using BoomifyCS.Objects;

namespace BoomifyCS.Interpreter
{
    public enum ByteType
    {
        ADD,
        SUB,
        MUL,
        DIV,
        MOD,
        POW,
        FLOORDIV,
        EQ,
        LT,
        GT,
        LTE,
        GTE,
        AND,
        OR,
        LOAD_CONST,
        LOAD,
        STORE,
        POP,
        NEQ, 
        NOT, 
        BITAND, 
        BITOR, 
        BITXOR, 
        BITNOT,
        LSHIFT, 
        RSHIFT, 
        JUMP,
        JUMP_IF_TRUE,
        JUMP_IF_FALSE
    }
    public class ByteInstruction
    {
        public ByteType Type;
        public List<Object> Value;
        public int IndexOfInstruction;
        public ByteInstruction(ByteType type, Object value, int indexOfInstruction)
        {
            Type = type;
            Value = new List<Object> { value };
            this.IndexOfInstruction = indexOfInstruction;
        }

        public ByteInstruction(ByteType type, List<Object> value,int indexOfInstruction)
        {
            Type = type;
            Value = value;
            IndexOfInstruction = indexOfInstruction;
        }
        public ByteInstruction(ByteType type, int indexOfInstruction)
        {
            Type = type;
            IndexOfInstruction = indexOfInstruction;
        }

        public override string ToString()
        {
            string name = Type.ToString();
            string arguments = "";
            if (Value != null)
            {
                foreach (Object value in Value)
                {
                    arguments += value.ToString();
                }
            }
            
            return $"{name} {arguments}    ({IndexOfInstruction})";
        }
        public void SetValue(object value)
        {
            if (value is List<object> list)
            {
                Value = list;
            }
            else
            {
                Value = new List<object> { value };
            }
        }
    }
    public static class ByteCodeConfig
    {
        public static Dictionary<TokenType, ByteType> BinaryOperators = new Dictionary<TokenType, ByteType>
        {
            {TokenType.ADD, ByteType.ADD},
            {TokenType.SUB, ByteType.SUB},
            {TokenType.MUL, ByteType.MUL},
            {TokenType.DIV, ByteType.DIV},
            {TokenType.MOD, ByteType.MOD},
            {TokenType.POW, ByteType.POW},
            {TokenType.FLOORDIV, ByteType.FLOORDIV},
            {TokenType.LT, ByteType.LT},
            {TokenType.GT, ByteType.GT},
            {TokenType.LTE, ByteType.LTE},
            {TokenType.GTE, ByteType.GTE},
            {TokenType.AND, ByteType.AND},
            {TokenType.OR, ByteType.OR},
            {TokenType.EQ, ByteType.EQ},
            {TokenType.NEQ, ByteType.NEQ},
            {TokenType.NOT, ByteType.NOT}, 
            {TokenType.BITAND, ByteType.BITAND}, 
            {TokenType.BITOR, ByteType.BITOR},
            {TokenType.BITXOR, ByteType.BITXOR},
            {TokenType.BITNOT, ByteType.BITNOT}, 
            {TokenType.LSHIFT, ByteType.LSHIFT},
            {TokenType.RSHIFT, ByteType.RSHIFT},
        };
        public static Dictionary<ByteType, string> byteToString = new Dictionary<ByteType, string>
        {
            { ByteType.ADD, "+" },
            { ByteType.SUB, "-" },
            { ByteType.MUL, "*" },
            { ByteType.DIV, "/" },
            { ByteType.MOD, "%" },
            { ByteType.POW, "**" },
            { ByteType.FLOORDIV, "//" },
            { ByteType.EQ, "==" },
            { ByteType.LT, "<" },
            { ByteType.GT, ">" },
            { ByteType.LTE, "<=" },
            { ByteType.GTE, ">=" },
            { ByteType.AND, "&&" },
            { ByteType.OR, "||" },
            { ByteType.LOAD_CONST, "LOAD_CONST" },
            { ByteType.LOAD, "LOAD" },
            { ByteType.STORE, "STORE" },
            { ByteType.POP, "POP" },
            { ByteType.NEQ, "!=" },
            { ByteType.NOT, "!" }, 
            { ByteType.BITAND, "&" }, 
            { ByteType.BITOR, "|" },
            { ByteType.BITXOR, "^" }, 
            { ByteType.BITNOT, "~" },
            { ByteType.LSHIFT, "<<" }, 
            { ByteType.RSHIFT, ">>" },
        };

    }

}
