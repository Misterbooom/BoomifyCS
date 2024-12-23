using System;
using BoomifyCS.Lexer;
using BoomifyCS.Exceptions;

namespace BoomifyCS.Objects
{
    public class BifyInteger : BifyObject
    {
        public int Value;

        public BifyInteger(int value) : base()
        {
            checked
            {
                if (value < int.MinValue || value > int.MaxValue)
                {
                    throw new OverflowException("Value is out of range.");
                }
                this.Value = value;
            }
        }

        public override BifyString ObjectToString()
        {
            return new BifyString($"{Value}");
        }

        public override string ToString()
        {
            return ObjectToString().Value;
        }

        public override BifyString Repr()
        {
            return new BifyString($"BifyInteger({Value})");
        }

        public override BifyObject Add(BifyObject other)
        {
            if (other is BifyInteger otherInt)
            {
                return new BifyInteger(checked(this.Value + otherInt.Value));
            }
            if (other is BifyFloat otherFloat)
            {
                return new BifyFloat(this.Value + otherFloat.Value);
            }
            Traceback.Instance.ThrowException(new BifyTypeError(ErrorMessage.InvalidTypeForOperation("Add", GetName(), other.GetName())));
            return null;
        }

        public override BifyObject Sub(BifyObject other)
        {
            if (other is BifyInteger otherInt)
            {
                return new BifyInteger(checked(this.Value - otherInt.Value));
            }
            if (other is BifyFloat otherFloat)
            {
                return new BifyFloat(this.Value - otherFloat.Value);
            }
            Traceback.Instance.ThrowException(new BifyTypeError(ErrorMessage.InvalidTypeForOperation("Sub", GetName(), other.GetName())));
            return null;
        }

        public override BifyObject Mul(BifyObject other)
        {
            if (other is BifyInteger otherInt)
            {
                try
                {
                    return new BifyInteger(checked(this.Value * otherInt.Value));
                }
                catch (OverflowException)
                {
                    Traceback.Instance.ThrowException(new BifyOverflowError("Integer value too large to multiply"));
                    return null;
                }
            }
            if (other is BifyFloat otherFloat)
            {
                return new BifyFloat(this.Value * otherFloat.Value);
            }
            Traceback.Instance.ThrowException(new BifyTypeError(ErrorMessage.InvalidTypeForOperation("Mul", GetName(), other.GetName())));
            return null;
        }

        public override BifyObject Div(BifyObject other)
        {
            if (other is BifyInteger otherInt)
            {
                if (otherInt.Value == 0)
                {
                    Traceback.Instance.ThrowException(new BifyZeroDivisionError(ErrorMessage.DivisionByZero()));
                }
                return new BifyFloat((double)this.Value / otherInt.Value);
            }
            if (other is BifyFloat otherFloat)
            {
                if (otherFloat.Value == 0)
                {
                    Traceback.Instance.ThrowException(new BifyZeroDivisionError(ErrorMessage.DivisionByZero()));
                }
                return new BifyFloat(this.Value / otherFloat.Value);
            }
            Traceback.Instance.ThrowException(new BifyTypeError(ErrorMessage.InvalidTypeForOperation("Div", GetName(), other.GetName())));
            return null;
        }

        public override BifyObject Mod(BifyObject other)
        {
            if (other is BifyInteger otherInt)
            {
                return new BifyInteger(checked(this.Value % otherInt.Value));
            }
            if (other is BifyFloat otherFloat)
            {
                return new BifyFloat(this.Value % otherFloat.Value);
            }
            Traceback.Instance.ThrowException(new BifyTypeError(ErrorMessage.InvalidTypeForOperation("Mod", GetName(), other.GetName())));
            return null;
        }

        public override BifyObject Pow(BifyObject other)
        {
            if (other is BifyInteger otherInt)
            {
                double result = Math.Pow(this.Value, otherInt.Value);
                if (result == (int)result)
                {
                    return new BifyInteger(checked((int)result));
                }
                return new BifyFloat(result);
            }
            if (other is BifyFloat otherFloat)
            {
                return new BifyFloat(Math.Pow(this.Value, otherFloat.Value));
            }
            Traceback.Instance.ThrowException(new BifyTypeError(ErrorMessage.InvalidTypeForOperation("Pow", GetName(), other.GetName())));
            return null;
        }

        public override BifyObject FloorDiv(BifyObject other)
        {
            if (other is BifyInteger otherInt)
            {
                if (otherInt.Value == 0)
                {
                    Traceback.Instance.ThrowException(new BifyZeroDivisionError(ErrorMessage.DivisionByZero()));
                }
                return new BifyInteger(checked((int)Math.Floor((double)this.Value / otherInt.Value)));
            }
            if (other is BifyFloat otherFloat)
            {
                if (otherFloat.Value == 0)
                {
                    Traceback.Instance.ThrowException(new BifyZeroDivisionError(ErrorMessage.DivisionByZero()));
                }
                return new BifyFloat(Math.Floor(this.Value / otherFloat.Value));
            }
            Traceback.Instance.ThrowException(new BifyTypeError(ErrorMessage.InvalidTypeForOperation("FloorDiv", GetName(), other.GetName())));
            return null;
        }

        public override BifyObject BitAnd(BifyObject other)
        {
            if (other is BifyInteger otherInt)
            {
                return new BifyInteger(checked(this.Value & otherInt.Value));
            }
            Traceback.Instance.ThrowException(new BifyTypeError(ErrorMessage.InvalidTypeForOperation("BitAnd", GetName(), other.GetName())));
            return null;
        }

        public override BifyObject BitOr(BifyObject other)
        {
            if (other is BifyInteger otherInt)
            {
                return new BifyInteger(checked(this.Value | otherInt.Value));
            }
            Traceback.Instance.ThrowException(new BifyTypeError(ErrorMessage.InvalidTypeForOperation("BitOr", GetName(), other.GetName())));
            return null;
        }

        public override BifyObject BitXor(BifyObject other)
        {
            if (other is BifyInteger otherInt)
            {
                return new BifyInteger(checked(this.Value ^ otherInt.Value));
            }
            Traceback.Instance.ThrowException(new BifyTypeError(ErrorMessage.InvalidTypeForOperation("BitXor", GetName(), other.GetName())));
            return null;
        }

        public override BifyObject BitNot()
        {
            return new BifyInteger(~this.Value);
        }

        public override BifyObject LeftShift(BifyObject other)
        {
            if (other is BifyInteger shiftAmount)
            {
                return new BifyInteger(checked(this.Value << shiftAmount.Value));
            }
            Traceback.Instance.ThrowException(new BifyTypeError(ErrorMessage.InvalidTypeForOperation("LeftShift", GetName(), other.GetName())));
            return null;
        }

        public override BifyObject RightShift(BifyObject other)
        {
            if (other is BifyInteger shiftAmount)
            {
                return new BifyInteger(checked(this.Value >> shiftAmount.Value));
            }
            Traceback.Instance.ThrowException(new BifyTypeError(ErrorMessage.InvalidTypeForOperation("RightShift", GetName(), other.GetName())));
            return null;
        }

        public override BifyObject Eq(BifyObject other)
        {
            if (other is BifyInteger otherInt)
            {
                return new BifyBoolean(this.Value == otherInt.Value);
            }
            if (other is BifyFloat otherFloat)
            {
                return new BifyBoolean(this.Value == otherFloat.Value);
            }
            return new BifyBoolean(false);
        }

        public override BifyObject Neq(BifyObject other)
        {
            if (other is BifyInteger otherInt)
            {
                return new BifyBoolean(this.Value != otherInt.Value);
            }
            if (other is BifyFloat otherFloat)
            {
                return new BifyBoolean(this.Value != otherFloat.Value);
            }
            return new BifyBoolean(true);
        }

        public override BifyObject Lt(BifyObject other)
        {
            if (other is BifyInteger otherInt)
            {
                return new BifyBoolean(this.Value < otherInt.Value);
            }
            if (other is BifyFloat otherFloat)
            {
                return new BifyBoolean(this.Value < otherFloat.Value);
            }
            Traceback.Instance.ThrowException(new BifyTypeError(ErrorMessage.InvalidTypeForOperation("Lt", GetName(), other.GetName())));
            return null;
        }

        public override BifyObject Gt(BifyObject other)
        {
            if (other is BifyInteger otherInt)
            {
                return new BifyBoolean(this.Value > otherInt.Value);
            }
            if (other is BifyFloat otherFloat)
            {
                return new BifyBoolean(this.Value > otherFloat.Value);
            }
            Traceback.Instance.ThrowException(new BifyTypeError(ErrorMessage.InvalidTypeForOperation("Gt", GetName(), other.GetName())));
            return null;
        }

        public override BifyObject Lte(BifyObject other)
        {
            if (other is BifyInteger otherInt)
            {
                return new BifyBoolean(this.Value <= otherInt.Value);
            }
            if (other is BifyFloat otherFloat)
            {
                return new BifyBoolean(this.Value <= otherFloat.Value);
            }
            Traceback.Instance.ThrowException(new BifyTypeError(ErrorMessage.InvalidTypeForOperation("Lte", GetName(), other.GetName())));
            return null;
        }

        public override BifyObject Gte(BifyObject other)
        {
            if (other is BifyInteger otherInt)
            {
                return new BifyBoolean(this.Value >= otherInt.Value);
            }
            if (other is BifyFloat otherFloat)
            {
                return new BifyBoolean(this.Value >= otherFloat.Value);
            }
            Traceback.Instance.ThrowException(new BifyTypeError(ErrorMessage.InvalidTypeForOperation("Gte", GetName(), other.GetName())));
            return null;
        }

        public static BifyInteger Convert(object other)
        {
            if (other is BifyInteger integer)
            {
                return integer;
            }
            else if (other is BifyFloat bifyFloat)
            {
                try
                {
                    int converted = checked((int)bifyFloat.Value);
                    return new BifyInteger(converted);
                }
                catch (OverflowException)
                {
                    Traceback.Instance.ThrowException(new BifyOverflowError("Float value too large to convert to Integer"));
                    return null;
                }
            }
            else if (other is BifyString bifyString)
            {
                if (int.TryParse(bifyString.Value, out int result))
                {
                    return new BifyInteger(result);
                }
                else
                {
                    Traceback.Instance.ThrowException(new BifyOverflowError("String cannot be converted to Integer"));
                    return null;
                }
            }
            Traceback.Instance.ThrowException(new BifyOverflowError("Unsupported conversion to Integer"));
            return null;
        }

        public override BifyObject Not()
        {
            return new BifyBoolean(this.Value == 0);
        }

  
    }
}
