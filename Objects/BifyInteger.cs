﻿using System;
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
    }
}
