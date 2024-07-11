using System;
using BoomifyCS.Lexer;

namespace BoomifyCS.Objects
{
    public class BifyBoolean : BifyObject
    {
        public bool Value;

        public BifyBoolean(Token token, bool value) : base(token)
        {
            this.Value = value;
        }

        public override string ToString()
        {
            return $"{Value}";
        }

        public string Repr()
        {
            return $"BifyBoolean({Value})";
        }
    }
}
