using BoomifyCS.Lexer;
using System;
using BoomifyCS.Exceptions;

namespace BoomifyCS.Objects
{
    public class BifyFloat : BifyObject
    {
        public double Value;

        public BifyFloat(double value) : base() => this.Value = value;

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
                return new BifyFloat(this.Value + otherFloat.Value);
            }
            if (other is BifyInteger otherInt)
            {
                return new BifyFloat(this.Value + otherInt.Value);
            }
            Traceback.Instance.ThrowException(new BifyTypeError(ErrorMessage.InvalidTypeForOperation("Add", GetName(), other.GetName())));
            return null;
        }

        public override BifyObject Sub(BifyObject other)
        {
            if (other is BifyFloat otherFloat)
            {
                return new BifyFloat(this.Value - otherFloat.Value);
            }
            if (other is BifyInteger otherInt)
            {
                return new BifyFloat(this.Value - otherInt.Value);
            }
            Traceback.Instance.ThrowException(new BifyTypeError(ErrorMessage.InvalidTypeForOperation("Sub", GetName(), other.GetName())));
            return null;
        }

        public override BifyObject Mul(BifyObject other)
        {
            if (other is BifyFloat otherFloat)
            {
                return new BifyFloat(this.Value * otherFloat.Value);
            }
            if (other is BifyInteger otherInt)
            {
                return new BifyFloat(this.Value * otherInt.Value);
            }
            Traceback.Instance.ThrowException(new BifyTypeError(ErrorMessage.InvalidTypeForOperation("Mul", GetName(), other.GetName())));
            return null;
        }

        public override BifyObject Div(BifyObject other)
        {
            if (other is BifyFloat otherFloat)
            {
                if (otherFloat.Value == 0)
                {
                    Traceback.Instance.ThrowException(new BifyZeroDivisionError(ErrorMessage.DivisionByZero()));
                }
                return new BifyFloat(this.Value / otherFloat.Value);
            }
            if (other is BifyInteger otherInt)
            {
                if (otherInt.Value == 0)
                {
                    Traceback.Instance.ThrowException(new BifyZeroDivisionError(ErrorMessage.DivisionByZero()));
                }
                return new BifyFloat(this.Value / otherInt.Value);
            }
            Traceback.Instance.ThrowException(new BifyTypeError(ErrorMessage.InvalidTypeForOperation("Div", GetName(), other.GetName())));
            return null;
        }

        public override BifyObject Mod(BifyObject other)
        {
            if (other is BifyFloat otherFloat)
            {
                return new BifyFloat(this.Value % otherFloat.Value);
            }
            if (other is BifyInteger otherInt)
            {
                return new BifyFloat(this.Value % otherInt.Value);
            }
            Traceback.Instance.ThrowException(new BifyTypeError(ErrorMessage.InvalidTypeForOperation("Mod", GetName(), other.GetName())));
            return null;
        }

        public override BifyObject Pow(BifyObject other)
        {
            if (other is BifyFloat otherFloat)
            {
                return new BifyFloat(Math.Pow(this.Value, otherFloat.Value));
            }
            if (other is BifyInteger otherInt)
            {
                return new BifyFloat(Math.Pow(this.Value, otherInt.Value));
            }
            Traceback.Instance.ThrowException(new BifyTypeError(ErrorMessage.InvalidTypeForOperation("Pow", GetName(), other.GetName())));
            return null;
        }

        public override BifyObject FloorDiv(BifyObject other)
        {
            if (other is BifyFloat otherFloat)
            {
                if (otherFloat.Value == 0)
                {
                    Traceback.Instance.ThrowException(new BifyZeroDivisionError(ErrorMessage.DivisionByZero()));
                }
                return new BifyFloat(Math.Floor(this.Value / otherFloat.Value));
            }
            if (other is BifyInteger otherInt)
            {
                if (otherInt.Value == 0)
                {
                    Traceback.Instance.ThrowException(new BifyZeroDivisionError(ErrorMessage.DivisionByZero()));
                }
                return new BifyFloat(Math.Floor(this.Value / otherInt.Value));
            }
            Traceback.Instance.ThrowException(new BifyTypeError(ErrorMessage.InvalidTypeForOperation("FloorDiv", GetName(), other.GetName())));
            return null;
        }

        public override BifyBoolean Bool()
        {
            return new BifyBoolean(this.Value != 0);
        }

        public override BifyInteger Int()
        {
            return new BifyInteger((int)this.Value);
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
            Traceback.Instance.ThrowException(new BifyTypeError(ErrorMessage.InvalidTypeForOperation("Lte", GetName(), other.GetName())));
            return null;
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
            Traceback.Instance.ThrowException(new BifyTypeError(ErrorMessage.InvalidTypeForOperation("Gte", GetName(), other.GetName())));
            return null;
        }

        public override string ToString()
        {
            return ObjectToString().Value;
        }
    }
}
