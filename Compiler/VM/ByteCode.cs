using System;
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
        DEFINE,
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
        JUMP_IF_FALSE,
        CALL,
        MODULE,
        ADDE,
        SUBE,
        MULE,
        DIVE,
        FLOORDIVE,
        POWE,
        NEW_ARRAY,
    }
    public class ByteInstruction
    {
        public ByteType Type;
        public List<object> Value;
        public int IndexOfInstruction;
        public ByteInstruction(ByteType type, object value, int indexOfInstruction)
        {
            Type = type;
            Value = [value];
            this.IndexOfInstruction = indexOfInstruction;
        }

        public ByteInstruction(ByteType type, List<object> value,int indexOfInstruction)
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
                    arguments += " " + value.ToString();
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
                Value = [value];
            }
        }
    }
    public static class ByteCodeConfig
    {
        public readonly static Dictionary<TokenType, ByteType> BinaryOperators = new()
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
            {TokenType.LTEQ, ByteType.LTE},
            {TokenType.GTEQ, ByteType.GTE},
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
        public readonly static Dictionary<TokenType, ByteType> AssignmentOperators = new()
        {
            {TokenType.ADDE, ByteType.ADDE},
            {TokenType.SUBE, ByteType.SUBE},
            {TokenType.DIVE, ByteType.DIVE},
            {TokenType.DIV, ByteType.DIVE},
            {TokenType.POWE, ByteType.POWE},
            {TokenType.FLOORDIVE, ByteType.FLOORDIVE},
        };
        public readonly static Dictionary<ByteType, string> byteToString = new()
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
            { ByteType.DEFINE, "DEFINE" },
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
