using BoomifyCS.Lexer;
using System;
using BoomifyCS.Exceptions;
using System.Globalization;

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
                return new BifyFloat(checked(this.Value + otherFloat.Value));
            }
            if (other is BifyInteger otherInt)
            {
                return new BifyFloat(checked(this.Value + otherInt.Value));
            }
            Traceback.Instance.ThrowException(new BifyTypeError(ErrorMessage.InvalidTypeForOperation("Add", GetName(), other.GetName())));
            return new BifyInteger(0);
        }

        public override BifyObject Sub(BifyObject other)
        {
            if (other is BifyFloat otherFloat)
            {
                return new BifyFloat(checked(this.Value - otherFloat.Value));
            }
            if (other is BifyInteger otherInt)
            {
                return new BifyFloat(checked(this.Value - otherInt.Value));
            }
            Traceback.Instance.ThrowException(new BifyTypeError(ErrorMessage.InvalidTypeForOperation("Sub", GetName(), other.GetName())));
            return new BifyInteger(0);
        }

        public override BifyObject Mul(BifyObject other)
        {
            if (other is BifyFloat otherFloat)
            {
                return new BifyFloat(checked(this.Value * otherFloat.Value));
            }
            if (other is BifyInteger otherInt)
            {
                return new BifyFloat(checked(this.Value * otherInt.Value));
            }
            Traceback.Instance.ThrowException(new BifyTypeError(ErrorMessage.InvalidTypeForOperation("Mul", GetName(), other.GetName())));
            return new BifyInteger(0);
        }

        public override BifyObject Div(BifyObject other)
        {
            if (other is BifyFloat otherFloat)
            {
                if (otherFloat.Value == 0)
                {
                    Traceback.Instance.ThrowException(new BifyZeroDivisionError(ErrorMessage.DivisionByZero()));
                }
                return new BifyFloat(checked(this.Value / otherFloat.Value));
            }
            if (other is BifyInteger otherInt)
            {
                if (otherInt.Value == 0)
                {
                    Traceback.Instance.ThrowException(new BifyZeroDivisionError(ErrorMessage.DivisionByZero()));
                }
                return new BifyFloat(checked(this.Value / otherInt.Value));
            }
            Traceback.Instance.ThrowException(new BifyTypeError(ErrorMessage.InvalidTypeForOperation("Div", GetName(), other.GetName())));
            return new BifyInteger(0);
        }

        public override BifyObject Mod(BifyObject other)
        {
            if (other is BifyFloat otherFloat)
            {
                return new BifyFloat(checked(this.Value % otherFloat.Value));
            }
            if (other is BifyInteger otherInt)
            {
                return new BifyFloat(checked(this.Value % otherInt.Value));
            }
            Traceback.Instance.ThrowException(new BifyTypeError(ErrorMessage.InvalidTypeForOperation("Mod", GetName(), other.GetName())));
            return new BifyInteger(0);
        }

        public override BifyObject Pow(BifyObject other)
        {
            if (other is BifyFloat otherFloat)
            {
                return new BifyFloat(checked((float)Math.Pow(this.Value, otherFloat.Value)));
            }
            if (other is BifyInteger otherInt)
            {
                return new BifyFloat(checked((float)Math.Pow(this.Value, otherInt.Value)));
            }
            Traceback.Instance.ThrowException(new BifyTypeError(ErrorMessage.InvalidTypeForOperation("Pow", GetName(), other.GetName())));
            return new BifyInteger(0);
        }

        public override BifyObject FloorDiv(BifyObject other)
        {
            if (other is BifyFloat otherFloat)
            {
                if (otherFloat.Value == 0)
                {
                    Traceback.Instance.ThrowException(new BifyZeroDivisionError(ErrorMessage.DivisionByZero()));
                }
                return new BifyFloat(checked((float)Math.Floor(this.Value / otherFloat.Value)));
            }
            if (other is BifyInteger otherInt)
            {
                if (otherInt.Value == 0)
                {
                    Traceback.Instance.ThrowException(new BifyZeroDivisionError(ErrorMessage.DivisionByZero()));
                }
                return new BifyFloat(checked((float)Math.Floor(this.Value / otherInt.Value)));
            }
            Traceback.Instance.ThrowException(new BifyTypeError(ErrorMessage.InvalidTypeForOperation("FloorDiv", GetName(), other.GetName())));
            return new BifyInteger(0);
        }

        public override BifyBoolean Bool()
        {
            return new BifyBoolean(this.Value != 0);
        }

        public override BifyInteger Int()
        {
            return BifyInteger.Convert(this);
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
            return new BifyBoolean(true);
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
            return new BifyBoolean(true);
        }

        public override string ToString()
        {
            return ObjectToString().Value;
        }

        public static BifyFloat Convert(object other)
        {
            if (other is BifyFloat bifyFloat)
            {
                return bifyFloat;
            }
            else if (other is BifyInteger integer)
            {
                return new BifyFloat(integer.Value);
            }
            else if (other is BifyString bifyString)
            {
                if (double.TryParse(bifyString.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out double result))
                {
                    return new BifyFloat(result);
                }
                else
                {
                    Traceback.Instance.ThrowException(new BifyOverflowError("String cannot be converted to Float"));
                    return new BifyFloat(0);
                }
            }
            Traceback.Instance.ThrowException(new BifyOverflowError("Unsupported conversion to Float"));
            return new BifyFloat(0);
        }
    }
}
