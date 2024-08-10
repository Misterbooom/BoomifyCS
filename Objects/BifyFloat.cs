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
            return new BifyBoolean(this.Value != 0);
        }

        public override BifyObject Int()
        {
            return new BifyInteger(this.Token, (int)this.Value);
        }

        // Add the LTE (less than or equal to) operator
        public override BifyObject Lte(BifyObject other)
        {
            if (other is BifyFloat otherFloat)
            {
                return new BifyBoolean(this.Value <= otherFloat.Value);
            }
            if (other is BifyInteger otherInt)
            {
                return new BifyBoolean(this.Value <= otherInt.Value);
            }
            throw new BifyTypeError($"Invalid type for Lte operation: {GetType().Name} and {other.GetType().Name}");
        }

        // Add the GTE (greater than or equal to) operator
        public override BifyObject Gte(BifyObject other)
        {
            if (other is BifyFloat otherFloat)
            {
                return new BifyBoolean(this.Value >= otherFloat.Value);
            }
            if (other is BifyInteger otherInt)
            {
                return new BifyBoolean(this.Value >= otherInt.Value);
            }
            throw new BifyTypeError($"Invalid type for Gte operation: {GetType().Name} and {other.GetType().Name}");
        }
    }
}
