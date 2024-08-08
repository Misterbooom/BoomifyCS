using BoomifyCS.Lexer;
using System;
using BoomifyCS.Exceptions;

namespace BoomifyCS.Objects
{
    public class BifyFloat(Token token, double value) : BifyObject(token)
    {
        public double Value = value;

        public override string ToString()
        {
            return $"{Value}";
        }

        public override string Repr()
        {
            return $"BifyFloat({Value})";
        }

        public override BifyObject Add(BifyObject other)
        {
            if (other is BifyFloat otherFloat)
            {
                return new BifyFloat(this.Token, this.Value + otherFloat.Value);
            }
            if (other is BifyInteger otherInt)
            {
                return new BifyFloat(this.Token, this.Value + otherInt.Value);
            }
            throw new BifyTypeError($"Invalid type for Add operation: {GetType().Name} and {other.GetType().Name}");
        }

        public override BifyObject Sub(BifyObject other)
        {
            if (other is BifyFloat otherFloat)
            {
                return new BifyFloat(this.Token, this.Value - otherFloat.Value);
            }
            if (other is BifyInteger otherInt)
            {
                return new BifyFloat(this.Token, this.Value - otherInt.Value);
            }
            throw new BifyTypeError($"Invalid type for Sub operation: {GetType().Name} and {other.GetType().Name}");
        }

        public override BifyObject Mul(BifyObject other)
        {
            if (other is BifyFloat otherFloat)
            {
                return new BifyFloat(this.Token, this.Value * otherFloat.Value);
            }
            if (other is BifyInteger otherInt)
            {
                return new BifyFloat(this.Token, this.Value * otherInt.Value);
            }
            throw new BifyTypeError($"Invalid type for Mul operation: {GetType().Name} and {other.GetType().Name}");
        }

        public override BifyObject Div(BifyObject other)
        {
            if (other is BifyFloat otherFloat)
            {
                if (otherFloat.Value == 0)
                {
                    throw new BifyZeroDivisionError("Cannot divide by zero.");
                }
                return new BifyFloat(this.Token, this.Value / otherFloat.Value);
            }
            if (other is BifyInteger otherInt)
            {
                if (otherInt.Value == 0)
                {
                    throw new BifyZeroDivisionError("Cannot divide by zero.");
                }
                return new BifyFloat(this.Token, this.Value / otherInt.Value);
            }
            throw new BifyTypeError($"Invalid type for Div operation: {GetType().Name} and {other.GetType().Name}");
        }

        public override BifyObject Mod(BifyObject other)
        {
            if (other is BifyFloat otherFloat)
            {
                return new BifyFloat(this.Token, this.Value % otherFloat.Value);
            }
            if (other is BifyInteger otherInt)
            {
                return new BifyFloat(this.Token, this.Value % otherInt.Value);
            }
            throw new BifyTypeError($"Invalid type for Mod operation: {GetType().Name} and {other.GetType().Name}");
        }

        public override BifyObject Pow(BifyObject other)
        {
            if (other is BifyFloat otherFloat)
            {
                return new BifyFloat(this.Token, Math.Pow(this.Value, otherFloat.Value));
            }
            if (other is BifyInteger otherInt)
            {
                return new BifyFloat(this.Token, Math.Pow(this.Value, otherInt.Value));
            }
            throw new BifyTypeError($"Invalid type for Pow operation: {GetType().Name} and {other.GetType().Name}");
        }

        public override BifyObject FloorDiv(BifyObject other)
        {
            if (other is BifyFloat otherFloat)
            {
                if (otherFloat.Value == 0)
                {
                    throw new DivideByZeroException("Cannot divide by zero.");
                }
                return new BifyFloat(this.Token, Math.Floor(this.Value / otherFloat.Value));
            }
            if (other is BifyInteger otherInt)
            {
                if (otherInt.Value == 0)
                {
                    throw new DivideByZeroException("Cannot divide by zero.");
                }
                return new BifyFloat(this.Token, Math.Floor(this.Value / otherInt.Value));
            }
            throw new BifyTypeError($"Invalid type for FloorDiv operation: {GetType().Name} and {other.GetType().Name}");
        }
        public override BifyBoolean Bool()
        {
            if (this.Value == 0)
            {
                return new BifyBoolean(false);
            }
            else
            {
                return new BifyBoolean(true);
            }
        }
        public override BifyObject Int()
        {
            return new BifyInteger(Token, (int)this.Value);
        }
    }
}
