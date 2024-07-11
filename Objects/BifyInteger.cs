using System;
using BoomifyCS.Lexer;

namespace BoomifyCS.Objects
{
    public class BifyInteger : BifyObject
    {
        public int Value;
        public BifyInteger(Token token,int value) : base(token)
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
        

    }
}
