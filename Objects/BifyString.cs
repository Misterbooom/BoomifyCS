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

        public BifyString(Token token, string value) : base(token)
        {
            this.Value = value;
        }
        public BifyString(string value) : base(new Token(TokenType.STRING,value))
        {
            this.Value = value;
        }


        public override string ToString()
        {
            return $"{Value}";
        }

        public override string Repr()
        {
            return $"BifyString({Value})";
        }
        public override BifyObject Mul(BifyObject other)
        {
            if (other is BifyInteger otherInt)
            {
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < otherInt.Value; i++)
                {
                    builder.Append(Value);
                }
                return new BifyString(Token, builder.ToString());
            }
            return new BifyBoolean(this.Token, false);
        }

        public override BifyObject Add(BifyObject other)
        {
            if (other is BifyString otherStr)
            {
                return new BifyString(this.Token, this.Value + otherStr.Value);
            }
            return new BifyBoolean(this.Token, false);

        }

        public override BifyObject Eq(BifyObject other)
        {
            if (other is BifyString otherStr)
            {
                return new BifyBoolean(this.Token, this.Value == otherStr.Value);
            }
            return new BifyBoolean(this.Token, false);

        }

        public override BifyObject Neq(BifyObject other)
        {
            if (other is BifyString otherStr)
            {
                return new BifyBoolean(this.Token, this.Value != otherStr.Value);
            }
            return new BifyBoolean(this.Token, true);

        }
        public override BifyBoolean Bool()
        {
            if (Value == "")
            {
                return new BifyBoolean(false);
            }
            return new BifyBoolean(true);

        }
        public override BifyObject Int()
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

    }
}

