using System;
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
        
        
    }
    public class ByteInstruction
    {
        public ByteType Type;
        public List<BifyObject> Value;

        public ByteInstruction(ByteType type, BifyObject value)
        {
            Type = type;
            Value = new List<BifyObject> { value };
        }

        public ByteInstruction(ByteType type, List<BifyObject> value)
        {
            Type = type;
            Value = value;
        }
        public ByteInstruction(ByteType type)
        {
            Type = type;
        }

        public override string ToString()
        {
            string name = Type.ToString();
            string arguments = "";
            if (Value != null)
            {
                foreach (BifyObject value in Value)
                {
                    arguments += value.ToString();
                }
            }
            
            return $"{name} {arguments}";
        }
    }
    public static class ByteCodeConfig
    {
        public static  Dictionary<TokenType, ByteType> TokenToByte = new Dictionary<TokenType, ByteType>
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
        };
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
        };
    }

}
