﻿using System;
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
        public override BifyString ObjectToString()
        {
            return new BifyString($"{Value}");
        }

        public override BifyString Repr()
        {
            return new BifyString($"BifyNull({Value})");
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
        public override BifyInteger Int()
        {
            return new BifyInteger(0);
        }
        public override string ToString()
        {
            return ObjectToString().Value;
        }
    }
}
