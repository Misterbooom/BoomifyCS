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

        public override string ToString()
        {
            return $"null";
        }

        public string Repr()
        {
            return $" BifyNull({Value})";
        }
    }
}
