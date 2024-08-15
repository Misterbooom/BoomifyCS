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
            throw new BifyTypeError(ErrorMessage.InvalidTypeForOperation("Add", GetType().Name, other.GetType().Name));
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
            throw new BifyTypeError(ErrorMessage.InvalidTypeForOperation("Sub", GetType().Name, other.GetType().Name));
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
            throw new BifyTypeError(ErrorMessage.InvalidTypeForOperation("Mul", GetType().Name, other.GetType().Name));
        }

        public override BifyObject Div(BifyObject other)
        {
            if (other is BifyFloat otherFloat)
            {
                if (otherFloat.Value == 0)
                {
                    throw new BifyZeroDivisionError(ErrorMessage.DivisionByZero());
                }
                return new BifyFloat(this.Token, this.Value / otherFloat.Value);
            }
            if (other is BifyInteger otherInt)
            {
                if (otherInt.Value == 0)
                {
                    throw new BifyZeroDivisionError(ErrorMessage.DivisionByZero());
                }
                return new BifyFloat(this.Token, this.Value / otherInt.Value);
            }
            throw new BifyTypeError(ErrorMessage.InvalidTypeForOperation("Div", GetType().Name, other.GetType().Name));
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
            throw new BifyTypeError(ErrorMessage.InvalidTypeForOperation("Mod", GetType().Name, other.GetType().Name));
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
            throw new BifyTypeError(ErrorMessage.InvalidTypeForOperation("Pow", GetType().Name, other.GetType().Name));
        }

        public override BifyObject FloorDiv(BifyObject other)
        {
            if (other is BifyFloat otherFloat)
            {
                if (otherFloat.Value == 0)
                {
                    throw new BifyZeroDivisionError(ErrorMessage.DivisionByZero());
                }
                return new BifyFloat(this.Token, Math.Floor(this.Value / otherFloat.Value));
            }
            if (other is BifyInteger otherInt)
            {
                if (otherInt.Value == 0)
                {
                    throw new BifyZeroDivisionError(ErrorMessage.DivisionByZero());
                }
                return new BifyFloat(this.Token, Math.Floor(this.Value / otherInt.Value));
            }
            throw new BifyTypeError(ErrorMessage.InvalidTypeForOperation("FloorDiv", GetType().Name, other.GetType().Name));
        }

        public override BifyBoolean Bool()
        {
            return new BifyBoolean(this.Value != 0);
        }

        public override BifyObject Int()
        {
            return new BifyInteger(this.Token, (int)this.Value);
        }

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
            throw new BifyTypeError(ErrorMessage.InvalidTypeForOperation("Lte", GetType().Name, other.GetType().Name));
        }

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
            throw new BifyTypeError(ErrorMessage.InvalidTypeForOperation("Gte", GetType().Name, other.GetType().Name));
        }
    }
}
