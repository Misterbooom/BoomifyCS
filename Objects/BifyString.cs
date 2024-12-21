using System;
using System.Text;
using BoomifyCS.Ast;
using BoomifyCS.Lexer;
using BoomifyCS.Exceptions;
namespace BoomifyCS.Objects
{
    public class BifyString : BifyObject
    {
        public string Value;

        public BifyString(Token token, string value) : base() => this.Value = value;
        public BifyString(string value) : base() => this.Value = value;


        public override BifyString ObjectToString()
        {
            return new BifyString($"{Value}");
        }

        public override BifyString Repr()
        {
            return new BifyString($"BifyString({Value})");
        }
        public override BifyObject Mul(BifyObject other)
        {
            if (other is BifyInteger otherInt)
            {
                StringBuilder builder = new();
                for (int i = 0; i < otherInt.Value; i++)
                {
                    builder.Append(Value);
                }
                return new BifyString( builder.ToString());
            }
            return new BifyBoolean(false);
        }

        public override BifyObject Add(BifyObject other)
        {
            
            return new BifyString(this.Value + other.ObjectToString().Value);

        }

        public override BifyObject Eq(BifyObject other)
        {
            if (other is BifyString otherStr)
            {
                return new BifyBoolean(this.Value == otherStr.Value);
            }
            return new BifyBoolean(false);

        }

        public override BifyObject Neq(BifyObject other)
        {
            if (other is BifyString otherStr)
            {
                return new BifyBoolean(this.Value != otherStr.Value);
            }
            return new BifyBoolean(true);

        }
        public override BifyBoolean Bool()
        {
            if (Value == "")
            {
                return new BifyBoolean(false);
            }
            return new BifyBoolean(true);

        }
        public override BifyInteger Int()
        {
            try
            {
                return new BifyInteger(int.Parse(Value));
            }
            catch (FormatException )
            {
                throw new BifyCastError($"The value '{Value}' is not in a correct format for an integer. ");
            }
            catch (OverflowException)
            {
                throw new BifyCastError($"The value '{Value}' is too large or too small for an integer.");
            }
            catch (ArgumentException)
            {
                throw new BifyCastError($"An argument exception occurred while parsing '{Value}'.");
            }
            catch (Exception )
            {
                throw new BifyCastError($"An unexpected error occurred while parsing '{Value}'.");
            }
        }
        public override string ToString()
        {
            return ObjectToString().Value;
        }

    }
}

