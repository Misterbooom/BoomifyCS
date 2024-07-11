using BoomifyCS.Lexer;
using System;

namespace BoomifyCS.Objects
{
    public class BifyObject
    {
        public Token Token;
        public object Value;

        public BifyObject(Token token)
        {
            this.Token = token;
        }

        public virtual BifyObject Add(BifyObject other)
        {
            throw new NotImplementedException("Add operation not implemented for this type.");
        }

        public virtual BifyObject Sub(BifyObject other)
        {
            throw new NotImplementedException("Sub operation not implemented for this type.");
        }

        public virtual BifyObject Mul(BifyObject other)
        {
            throw new NotImplementedException("Mul operation not implemented for this type.");
        }

        public virtual BifyObject Div(BifyObject other)
        {
            throw new NotImplementedException("Div operation not implemented for this type.");
        }

        public virtual BifyObject Mod(BifyObject other)
        {
            throw new NotImplementedException("Mod operation not implemented for this type.");
        }

        public virtual BifyObject Pow(BifyObject other)
        {
            throw new NotImplementedException("Pow operation not implemented for this type.");
        }
        
        public virtual BifyObject FloorDiv(BifyObject other)
        {
            throw new NotImplementedException("Pow operation not implemented for this type.");
        }
        public virtual BifyObject Eq(BifyObject other)
        {
            throw new NotImplementedException("Eq operation not implemented for this type.");
        }

        public virtual BifyObject Neq(BifyObject other)
        {
            throw new NotImplementedException("Neq operation not implemented for this type.");
        }
    }
}

