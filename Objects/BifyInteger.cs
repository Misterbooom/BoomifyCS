using System;
using BoomifyCS.Lexer;
using BoomifyCS.Exceptions;

namespace BoomifyCS.Objects
{
    public class BifyInteger : BifyObject
    {
        public int Value;
        public BifyInteger(Token token, int value) : base(token)
        {
            this.Value = value;
        }


        public BifyInteger(int value) : base(new Token(TokenType.NUMBER, value.ToString()))
        {
            this.Value = value;
        }

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
                return new BifyInteger(this.Token, this.Value + otherInt.Value);
            }
            if (other is BifyFloat otherFloat)
            {
                return new BifyFloat(this.Token, this.Value + otherFloat.Value);
            }
            throw new BifyTypeError(ErrorMessage.InvalidTypeForOperation("Add", GetName(), other.GetName()));
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
            throw new BifyTypeError(ErrorMessage.InvalidTypeForOperation("Sub", GetName(), other.GetName()));
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
            throw new BifyTypeError(ErrorMessage.InvalidTypeForOperation("Mul", GetName(), other.GetName()));
        }

        public override BifyObject Div(BifyObject other)
        {
            if (other is BifyInteger otherInt)
            {
                if (otherInt.Value == 0)
                {
                    throw new BifyZeroDivisionError(ErrorMessage.DivisionByZero());
                }
                return new BifyFloat(this.Token, (double)this.Value / otherInt.Value);
            }
            if (other is BifyFloat otherFloat)
            {
                if (otherFloat.Value == 0)
                {
                    throw new BifyZeroDivisionError(ErrorMessage.DivisionByZero());
                }
                return new BifyFloat(this.Token, this.Value / otherFloat.Value);
            }
            throw new BifyTypeError(ErrorMessage.InvalidTypeForOperation("Div", GetName(), other.GetName()));
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
            throw new BifyTypeError(ErrorMessage.InvalidTypeForOperation("Mod", GetName(), other.GetName()));
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
            throw new BifyTypeError(ErrorMessage.InvalidTypeForOperation("Pow", GetName(), other.GetName()));
        }

        public override BifyObject FloorDiv(BifyObject other)
        {
            if (other is BifyInteger otherInt)
            {
                if (otherInt.Value == 0)
                {
                    throw new DivideByZeroException(ErrorMessage.DivisionByZero());
                }
                return new BifyInteger(this.Token, (int)Math.Floor((double)this.Value / otherInt.Value));
            }
            if (other is BifyFloat otherFloat)
            {
                if (otherFloat.Value == 0)
                {
                    throw new DivideByZeroException(ErrorMessage.DivisionByZero());
                }
                return new BifyFloat(this.Token, Math.Floor(this.Value / otherFloat.Value));
            }
            throw new BifyTypeError(ErrorMessage.InvalidTypeForOperation("FloorDiv", GetName(), other.GetName()));
        }

        // Bitwise AND operator (&)
        public override BifyObject BitAnd(BifyObject other)
        {
            if (other is BifyInteger otherInt)
            {
                return new BifyInteger(this.Token, this.Value & otherInt.Value);
            }
            throw new BifyTypeError(ErrorMessage.InvalidTypeForOperation("BitAnd", GetName(), other.GetName()));
        }

        // Bitwise OR operator (|)
        public override BifyObject BitOr(BifyObject other)
        {
            if (other is BifyInteger otherInt)
            {
                return new BifyInteger(this.Token, this.Value | otherInt.Value);
            }
            throw new BifyTypeError(ErrorMessage.InvalidTypeForOperation("BitOr", GetName(), other.GetName()));
        }

        // Bitwise XOR operator (^)
        public override BifyObject BitXor(BifyObject other)
        {
            if (other is BifyInteger otherInt)
            {
                return new BifyInteger(this.Token, this.Value ^ otherInt.Value);
            }
            throw new BifyTypeError(ErrorMessage.InvalidTypeForOperation("BitXor", GetName(), other.GetName()));
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
            throw new BifyTypeError(ErrorMessage.InvalidTypeForOperation("LeftShift", GetName(), other.GetName()));
        }

        // Right shift operator (>>)
        public override BifyObject RightShift(BifyObject other)
        {
            if (other is BifyInteger shiftAmount)
            {
                return new BifyInteger(this.Token, this.Value >> shiftAmount.Value);
            }
            throw new BifyTypeError(ErrorMessage.InvalidTypeForOperation("RightShift", GetName(), other.GetName()));
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
            throw new BifyTypeError(ErrorMessage.InvalidTypeForOperation("Lt", GetName(), other.GetName()));
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
            throw new BifyTypeError(ErrorMessage.InvalidTypeForOperation("Gt", GetName(), other.GetName()));
        }

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
            throw new BifyTypeError(ErrorMessage.InvalidTypeForOperation("Lte", GetName(), other.GetName()));
        }

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
            throw new BifyTypeError(ErrorMessage.InvalidTypeForOperation("Gte", GetName(), other.GetName()));
        }
        public override BifyBoolean Bool()
        {
            return new BifyBoolean(Value != 0);
        }
        public override BifyInteger Int()
        {
            return this;
        }
        public override string ToString()
        {
            return ObjectToString().Value;
        }
    }
}
