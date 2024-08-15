using BoomifyCS.Lexer;
using System;
using BoomifyCS.Exceptions;

namespace BoomifyCS.Objects
{
    public class BifyFloat(Token token, double value) : BifyObject(token)
    {
        public double Value = value;

        public override BifyString ObjectToString()
        {
            return new BifyString($"{Value}");
        }

        public override BifyString Repr()
        {
            return new BifyString($"BifyFloat({Value})");
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
            throw new BifyTypeError(ErrorMessage.InvalidTypeForOperation("Add", GetName(), other.GetName()));
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
            throw new BifyTypeError(ErrorMessage.InvalidTypeForOperation("Sub", GetName(), other.GetName()));
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
            throw new BifyTypeError(ErrorMessage.InvalidTypeForOperation("Mul", GetName(), other.GetName()));
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
            throw new BifyTypeError(ErrorMessage.InvalidTypeForOperation("Div", GetName(), other.GetName()));
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
            throw new BifyTypeError(ErrorMessage.InvalidTypeForOperation("Mod", GetName(), other.GetName()));
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
            throw new BifyTypeError(ErrorMessage.InvalidTypeForOperation("Pow", GetName(), other.GetName()));
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
            throw new BifyTypeError(ErrorMessage.InvalidTypeForOperation("FloorDiv", GetName(), other.GetName()));
        }

        public override BifyBoolean Bool()
        {
            return new BifyBoolean(this.Value != 0);
        }

        public override BifyInteger Int()
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
            throw new BifyTypeError(ErrorMessage.InvalidTypeForOperation("Lte", GetName(), other.GetName()));
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
            throw new BifyTypeError(ErrorMessage.InvalidTypeForOperation("Gte", GetName(), other.GetName()));
        }
        public override string ToString()
        {
            return ObjectToString().Value;
        }
    }
}
