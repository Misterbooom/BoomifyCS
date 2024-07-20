using System;
using System.Text;
using BoomifyCS.Ast;
using BoomifyCS.Lexer;

namespace BoomifyCS.Objects
{
    public class BifyString : BifyObject
    {
        public string Value;

        public BifyString(Token token, string value) : base(token)
        {
            this.Value = value;
        }

        public override string ToString()
        {
            return $"{Value}";
        }

        public string Repr()
        {
            return $"BifyString({Value})";
        }
        public override BifyObject Mul(BifyObject other)
        {
            if (other is BifyInteger otherInt)
            {
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < otherInt.Value; i++)
                {
                    builder.Append(Value);
                }
                return new BifyString(Token, builder.ToString());
            }
            return new BifyBoolean(this.Token, false);
        }

        public override BifyObject Add(BifyObject other)
        {
            if (other is BifyString otherStr)
            {
                return new BifyString(this.Token, this.Value + otherStr.Value);
            }
            return new BifyBoolean(this.Token, false);

        }

        public override BifyObject Eq(BifyObject other)
        {
            if (other is BifyString otherStr)
            {
                return new BifyBoolean(this.Token, this.Value == otherStr.Value);
            }
            return new BifyBoolean(this.Token, false);

        }

        public override BifyObject Neq(BifyObject other)
        {
            if (other is BifyString otherStr)
            {
                return new BifyBoolean(this.Token, this.Value != otherStr.Value);
            }
            return new BifyBoolean(this.Token, true);

        }
    }
}

