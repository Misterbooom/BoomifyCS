using System;
using BoomifyCS.Lexer;

namespace BoomifyCS.Objects
{
    public class BifyNull : BifyObject
    {
        public object Value;

        public BifyNull(Token token) : base(token)
        {
            this.Value = null;
        }
        public BifyNull() : base(new Token(TokenType.NULL,"null"))
        {
            this.Value = null;
        }
        public override string ToString()
        {
            return $"null";
        }

        public override string Repr()
        {
            return $"BifyNull({Value})";
        }

        public override BifyObject Eq(BifyObject other)
        {
            if (other is BifyNull)
            {
                return new BifyBoolean(this.Token, true);
            }
            return new BifyBoolean(this.Token, false);
        }

        public override BifyObject Neq(BifyObject other)
        {
            if (other is BifyNull)
            {
                return new BifyBoolean(this.Token, false);
            }
            return new BifyBoolean(this.Token, true);
        }
        public override BifyBoolean Bool()
        {
            return new BifyBoolean(false);
        }
        public override BifyObject Int()
        {
            return new BifyInteger(0);
        }
    }
}
