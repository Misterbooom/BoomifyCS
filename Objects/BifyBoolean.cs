using BoomifyCS.Lexer;
using System;

namespace BoomifyCS.Objects
{
    public class BifyBoolean : BifyObject
    {
        public bool Value;

        public BifyBoolean(Token token, bool value) : base(token)
        {
            this.Value = value;
        }
        public BifyBoolean(bool value) : base(new Token(TokenType.IDENTIFIER, value.ToString()))
        {
            this.Value = value;
        }

        public override string ToString()
        {
            return $"{Value}";
        }

        public override string Repr()
        {
            return $"BifyBoolean({Value})";
        }

        // Logical AND operator (&&)
        public override BifyObject And(BifyObject other)
        {
            if (other is BifyBoolean otherBool)
            {
                return new BifyBoolean(this.Token, this.Value && otherBool.Value);
            }
            return new BifyBoolean(false);
        }

        // Logical OR operator (||)
        public override BifyObject Or(BifyObject other)
        {
            if (other is BifyBoolean otherBool)
            {
                return new BifyBoolean(this.Token, this.Value || otherBool.Value);
            }
            return new BifyBoolean(false);
        }

        // Logical NOT operator (!)
        public override BifyObject Not()
        {
            return new BifyBoolean(this.Token, !this.Value);
        }

        // Equal to (==)
        public override BifyObject Eq(BifyObject other)
        {
            if (other is BifyBoolean otherBool)
            {
                return new BifyBoolean(this.Token, this.Value == otherBool.Value);
            }
            return new BifyBoolean(false);

        }

        // Not equal to (!=)
        public override BifyObject Neq(BifyObject other)
        {
            if (other is BifyBoolean otherBool)
            {
                return new BifyBoolean(this.Token, this.Value != otherBool.Value);
            }
            return new BifyBoolean(false);
        }
        public override BifyBoolean Bool()
        {
            return this;
        }
        public override BifyObject Int()
        {
            if (Value == false)
            {
                return new BifyInteger(0);

            }
            else
            {
                return new BifyInteger(1);
            }
        }
       
    }
}
