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

        public override string ToString()
        {
            return $"{Value}";
        }

        public string Repr()
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
            throw new InvalidOperationException("Logical AND operation not implemented for this type.");
        }

        // Logical OR operator (||)
        public override BifyObject Or(BifyObject other)
        {
            if (other is BifyBoolean otherBool)
            {
                return new BifyBoolean(this.Token, this.Value || otherBool.Value);
            }
            throw new InvalidOperationException("Logical OR operation not implemented for this type.");
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
            throw new InvalidOperationException("Equality operation not implemented for this type.");
        }

        // Not equal to (!=)
        public override BifyObject Neq(BifyObject other)
        {
            if (other is BifyBoolean otherBool)
            {
                return new BifyBoolean(this.Token, this.Value != otherBool.Value);
            }
            throw new InvalidOperationException("Inequality operation not implemented for this type.");
        }
    }
}
