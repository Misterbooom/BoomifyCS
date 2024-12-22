using System;
using BoomifyCS.Lexer;
using BoomifyCS.Exceptions;

namespace BoomifyCS.Objects
{
    public class BifyInteger : BifyObject
    {
        public int Value;
        public BifyInteger(int value) : base() => this.Value = value;

        public override BifyString ObjectToString()
        {
            return new BifyString($"{Value}");
        }

        public override BifyString Repr()
        {
            return new BifyString($"BifyInteger({Value})");
        }

        public override BifyObject Add(BifyObject other)
        {
            if (other is BifyInteger otherInt)
            {
                return new BifyInteger(this.Value + otherInt.Value);
            }
            if (other is BifyFloat otherFloat)
            {
                return new BifyFloat(this.Value + otherFloat.Value);
            }
            Traceback.Instance.ThrowException(new BifyTypeError(ErrorMessage.InvalidTypeForOperation("Add", GetName(), other.GetName())));
            return null; // will not be reached but is required for the method signature
        }

        public override BifyObject Sub(BifyObject other)
        {
            if (other is BifyInteger otherInt)
            {
                return new BifyInteger(this.Value - otherInt.Value);
            }
            if (other is BifyFloat otherFloat)
            {
                return new BifyFloat(this.Value - otherFloat.Value);
            }
            Traceback.Instance.ThrowException(new BifyTypeError(ErrorMessage.InvalidTypeForOperation("Sub", GetName(), other.GetName())));
            return null; // will not be reached but is required for the method signature
        }

        public override BifyObject Mul(BifyObject other)
        {
            if (other is BifyInteger otherInt)
            {
                return new BifyInteger(this.Value * otherInt.Value);
            }
            if (other is BifyFloat otherFloat)
            {
                return new BifyFloat(this.Value * otherFloat.Value);
            }
            Traceback.Instance.ThrowException(new BifyTypeError(ErrorMessage.InvalidTypeForOperation("Mul", GetName(), other.GetName())));
            return null; // will not be reached but is required for the method signature
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
            return null; // will not be reached but is required for the method signature
        }

        public override BifyObject Mod(BifyObject other)
        {
            if (other is BifyInteger otherInt)
            {
                return new BifyInteger(this.Value % otherInt.Value);
            }
            if (other is BifyFloat otherFloat)
            {
                return new BifyFloat(this.Value % otherFloat.Value);
            }
            Traceback.Instance.ThrowException(new BifyTypeError(ErrorMessage.InvalidTypeForOperation("Mod", GetName(), other.GetName())));
            return null; // will not be reached but is required for the method signature
        }

        public override BifyObject Pow(BifyObject other)
        {
            if (other is BifyInteger otherInt)
            {
                double result = Math.Pow(this.Value, otherInt.Value);
                if (result == (int)result)
                {
                    return new BifyInteger((int)result);
                }
                return new BifyFloat(result);
            }
            if (other is BifyFloat otherFloat)
            {
                return new BifyFloat(Math.Pow(this.Value, otherFloat.Value));
            }
            Traceback.Instance.ThrowException(new BifyTypeError(ErrorMessage.InvalidTypeForOperation("Pow", GetName(), other.GetName())));
            return null; // will not be reached but is required for the method signature
        }

        public override BifyObject FloorDiv(BifyObject other)
        {
            if (other is BifyInteger otherInt)
            {
                if (otherInt.Value == 0)
                {
                    Traceback.Instance.ThrowException(new BifyZeroDivisionError(ErrorMessage.DivisionByZero()));
                }
                return new BifyInteger((int)Math.Floor((double)this.Value / otherInt.Value));
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
            return null; // will not be reached but is required for the method signature
        }

        // Bitwise AND operator (&)
        public override BifyObject BitAnd(BifyObject other)
        {
            if (other is BifyInteger otherInt)
            {
                return new BifyInteger(this.Value & otherInt.Value);
            }
            Traceback.Instance.ThrowException(new BifyTypeError(ErrorMessage.InvalidTypeForOperation("BitAnd", GetName(), other.GetName())));
            return null; // will not be reached but is required for the method signature
        }

        // Bitwise OR operator (|)
        public override BifyObject BitOr(BifyObject other)
        {
            if (other is BifyInteger otherInt)
            {
                return new BifyInteger(this.Value | otherInt.Value);
            }
            Traceback.Instance.ThrowException(new BifyTypeError(ErrorMessage.InvalidTypeForOperation("BitOr", GetName(), other.GetName())));
            return null; // will not be reached but is required for the method signature
        }

        // Bitwise XOR operator (^)
        public override BifyObject BitXor(BifyObject other)
        {
            if (other is BifyInteger otherInt)
            {
                return new BifyInteger(this.Value ^ otherInt.Value);
            }
            Traceback.Instance.ThrowException(new BifyTypeError(ErrorMessage.InvalidTypeForOperation("BitXor", GetName(), other.GetName())));
            return null; // will not be reached but is required for the method signature
        }

        // Bitwise NOT operator (~)
        public override BifyObject BitNot()
        {
            return new BifyInteger(~this.Value);
        }

        // Left shift operator (<<)
        public override BifyObject LeftShift(BifyObject other)
        {
            if (other is BifyInteger shiftAmount)
            {
                return new BifyInteger(this.Value << shiftAmount.Value);
            }
            Traceback.Instance.ThrowException(new BifyTypeError(ErrorMessage.InvalidTypeForOperation("LeftShift", GetName(), other.GetName())));
            return null; // will not be reached but is required for the method signature
        }

        // Right shift operator (>>)
        public override BifyObject RightShift(BifyObject other)
        {
            if (other is BifyInteger shiftAmount)
            {
                return new BifyInteger(this.Value >> shiftAmount.Value);
            }
            Traceback.Instance.ThrowException(new BifyTypeError(ErrorMessage.InvalidTypeForOperation("RightShift", GetName(), other.GetName())));
            return null; // will not be reached but is required for the method signature
        }

        // Equality operator (==)
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

        // Inequality operator (!=)
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

        // Less than operator (<)
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
            return null; // will not be reached but is required for the method signature
        }

        // Greater than operator (>)
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
            return null; // will not be reached but is required for the method signature
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
            return null; // will not be reached but is required for the method signature
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
            return null; // will not be reached but is required for the method signature
        }
    }
}
