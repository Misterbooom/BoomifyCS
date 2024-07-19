using System;
using BoomifyCS.Lexer;

namespace BoomifyCS.Objects
{
    public class BifyInteger : BifyObject
    {
        public int Value;

        public BifyInteger(Token token, int value) : base(token)
        {
            this.Value = value;
        }

        public override string ToString()
        {
            return $"{Value}";
        }

        public string Repr()
        {
            return $"BifyInteger({Value})";
        }

        public override BifyObject Add(BifyObject other)
        {
            if (other is BifyInteger otherInt)
            {
                return new BifyInteger(this.Token, this.Value + otherInt.Value);
            }
            if (other is BifyFloat otherFloat)
            {
                return new BifyFloat(this.Token, this.Value + otherFloat.Value);
            }
            throw new InvalidOperationException("Invalid type for Add operation");
        }

        public override BifyObject Sub(BifyObject other)
        {
            if (other is BifyInteger otherInt)
            {
                return new BifyInteger(this.Token, this.Value - otherInt.Value);
            }
            if (other is BifyFloat otherFloat)
            {
                return new BifyFloat(this.Token, this.Value - otherFloat.Value);
            }
            throw new InvalidOperationException("Invalid type for Sub operation");
        }

        public override BifyObject Mul(BifyObject other)
        {
            if (other is BifyInteger otherInt)
            {
                return new BifyInteger(this.Token, this.Value * otherInt.Value);
            }
            if (other is BifyFloat otherFloat)
            {
                return new BifyFloat(this.Token, this.Value * otherFloat.Value);
            }
            throw new InvalidOperationException("Invalid type for Mul operation");
        }

        public override BifyObject Div(BifyObject other)
        {
            if (other is BifyInteger otherInt)
            {
                if (otherInt.Value == 0)
                {
                    throw new DivideByZeroException("Cannot divide by zero.");
                }
                return new BifyFloat(this.Token, (double)this.Value / otherInt.Value);
            }
            if (other is BifyFloat otherFloat)
            {
                if (otherFloat.Value == 0)
                {
                    throw new DivideByZeroException("Cannot divide by zero.");
                }
                return new BifyFloat(this.Token, this.Value / otherFloat.Value);
            }
            throw new InvalidOperationException("Invalid type for Div operation");
        }

        public override BifyObject Mod(BifyObject other)
        {
            if (other is BifyInteger otherInt)
            {
                return new BifyInteger(this.Token, this.Value % otherInt.Value);
            }
            if (other is BifyFloat otherFloat)
            {
                return new BifyFloat(this.Token, this.Value % otherFloat.Value);
            }
            throw new InvalidOperationException("Invalid type for Mod operation");
        }

        public override BifyObject Pow(BifyObject other)
        {
            if (other is BifyInteger otherInt)
            {
                double result = Math.Pow(this.Value, otherInt.Value);
                if (result == (int)result)
                {
                    return new BifyInteger(this.Token, (int)result);
                }
                return new BifyFloat(this.Token, result);
            }
            if (other is BifyFloat otherFloat)
            {
                return new BifyFloat(this.Token, Math.Pow(this.Value, otherFloat.Value));
            }
            throw new InvalidOperationException("Invalid type for Pow operation");
        }

        public override BifyObject FloorDiv(BifyObject other)
        {
            if (other is BifyInteger otherInt)
            {
                if (otherInt.Value == 0)
                {
                    throw new DivideByZeroException("Cannot divide by zero.");
                }
                return new BifyInteger(this.Token, (int)Math.Floor((double)this.Value / otherInt.Value));
            }
            if (other is BifyFloat otherFloat)
            {
                if (otherFloat.Value == 0)
                {
                    throw new DivideByZeroException("Cannot divide by zero.");
                }
                return new BifyFloat(this.Token, Math.Floor(this.Value / otherFloat.Value));
            }
            throw new InvalidOperationException("Invalid type for FloorDiv operation");
        }
        // Bitwise AND operator (&)
        public override BifyObject BitAnd(BifyObject other)
        {
            if (other is BifyInteger otherInt)
            {
                return new BifyInteger(this.Token, this.Value & otherInt.Value);
            }
            throw new InvalidOperationException("Invalid type for BitAnd operation");
        }

        // Bitwise OR operator (|)
        public override BifyObject BitOr(BifyObject other)
        {
            if (other is BifyInteger otherInt)
            {
                return new BifyInteger(this.Token, this.Value | otherInt.Value);
            }
            throw new InvalidOperationException("Invalid type for BitOr operation");
        }

        // Bitwise XOR operator (^)
        public override BifyObject BitXor(BifyObject other)
        {
            if (other is BifyInteger otherInt)
            {
                return new BifyInteger(this.Token, this.Value ^ otherInt.Value);
            }
            throw new InvalidOperationException("Invalid type for BitXor operation");
        }

        // Bitwise NOT operator (~)
        public override BifyObject BitNot()
        {
            return new BifyInteger(this.Token, ~this.Value);
        }

        // Left shift operator (<<)
        public override BifyObject LeftShift(BifyObject other)
        {
            if (other is BifyInteger shiftAmount)
            {
                return new BifyInteger(this.Token, this.Value << shiftAmount.Value);
            }
            throw new InvalidOperationException("Invalid type for LeftShift operation");
        }

        // Right shift operator (>>)
        public override BifyObject RightShift(BifyObject other)
        {
            if (other is BifyInteger shiftAmount)
            {
                return new BifyInteger(this.Token, this.Value >> shiftAmount.Value);
            }
            throw new InvalidOperationException("Invalid type for RightShift operation");
        }
        // Equality operator (==)
        public override BifyObject Eq(BifyObject other)
        {
            if (other is BifyInteger otherInt)
            {
                return new BifyBoolean(this.Token, this.Value == otherInt.Value);
            }
            if (other is BifyFloat otherFloat)
            {
                return new BifyBoolean(this.Token, this.Value == otherFloat.Value);
            }
            return new BifyBoolean(this.Token, false);
        }

        // Inequality operator (!=)
        public override BifyObject Neq(BifyObject other)
        {
            if (other is BifyInteger otherInt)
            {
                return new BifyBoolean(this.Token, this.Value != otherInt.Value);
            }
            if (other is BifyFloat otherFloat)
            {
                return new BifyBoolean(this.Token, this.Value != otherFloat.Value);
            }
            return new BifyBoolean(this.Token, true);
        }

        // Less than operator (<)
        public override BifyObject Lt(BifyObject other)
        {
            if (other is BifyInteger otherInt)
            {
                return new BifyBoolean(this.Token, this.Value < otherInt.Value);
            }
            if (other is BifyFloat otherFloat)
            {
                return new BifyBoolean(this.Token, this.Value < otherFloat.Value);
            }
            throw new InvalidOperationException("Invalid type for Lt operation");
        }

        // Greater than operator (>)
        public override BifyObject Gt(BifyObject other)
        {
            if (other is BifyInteger otherInt)
            {
                return new BifyBoolean(this.Token, this.Value > otherInt.Value);
            }
            if (other is BifyFloat otherFloat)
            {
                return new BifyBoolean(this.Token, this.Value > otherFloat.Value);
            }
            throw new InvalidOperationException("Invalid type for Gt operation");
        }

        // Less than or equal to operator (<=)
        public override BifyObject Lte(BifyObject other)
        {
            if (other is BifyInteger otherInt)
            {
                return new BifyBoolean(this.Token, this.Value <= otherInt.Value);
            }
            if (other is BifyFloat otherFloat)
            {
                return new BifyBoolean(this.Token, this.Value <= otherFloat.Value);
            }
            throw new InvalidOperationException("Invalid type for Lte operation");
        }

        // Greater than or equal to operator (>=)
        public override BifyObject Gte(BifyObject other)
        {
            if (other is BifyInteger otherInt)
            {
                return new BifyBoolean(this.Token, this.Value >= otherInt.Value);
            }
            if (other is BifyFloat otherFloat)
            {
                return new BifyBoolean(this.Token, this.Value >= otherFloat.Value);
            }
            throw new InvalidOperationException("Invalid type for Gte operation");
        }
    }
}
