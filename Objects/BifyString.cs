using System;
using System.Text;
using BoomifyCS.Ast;
using BoomifyCS.Lexer;
using BoomifyCS.Exceptions;

namespace BoomifyCS.Objects
{
    public class BifyString : BifyObject
    {
        public string Value;

        public BifyString(Token token, string value) : base() => this.Value = value;
        public BifyString(string value) : base() => this.Value = value;

        public override BifyString ObjectToString()
        {
            return new BifyString($"{Value}");
        }

        public override BifyString Repr()
        {
            return new BifyString($"BifyString({Value})");
        }

        public override BifyObject Mul(BifyObject other)
        {
            if (other is BifyInteger otherInt)
            {
                StringBuilder builder = new();
                for (int i = 0; i < otherInt.Value; i++)
                {
                    builder.Append(Value);
                }
                return new BifyString(builder.ToString());
            }
            return new BifyBoolean(false);
        }

        public override BifyObject Add(BifyObject other)
        {
            return new BifyString(this.Value + other.ObjectToString().Value);
        }

        public override BifyObject Eq(BifyObject other)
        {
            if (other is BifyString otherStr)
            {
                return new BifyBoolean(this.Value == otherStr.Value);
            }
            return new BifyBoolean(false);
        }

        public override BifyObject Neq(BifyObject other)
        {
            if (other is BifyString otherStr)
            {
                return new BifyBoolean(this.Value != otherStr.Value);
            }
            return new BifyBoolean(true);
        }

        public override BifyBoolean Bool()
        {
            if (Value == "")
            {
                return new BifyBoolean(false);
            }
            return new BifyBoolean(true);
        }

        public override BifyInteger Int()
        {
            
            return BifyInteger.Convert(this); 
        }

        public override string ToString()
        {
            return ObjectToString().Value;
        }
        public static BifyString Convert(object other)
        {
            if (other is BifyString bifyString)
            {
                return bifyString;
            }
            else if (other is BifyInteger integer)
            {
                return new BifyString(integer.Value.ToString());
            }
            else if (other is BifyFloat bifyFloat)
            {
                return new BifyString(bifyFloat.Value.ToString());
            }
            Traceback.Instance.ThrowException(new BifyOverflowError("Unsupported conversion to String"));
            return null;
        }
    }
}
