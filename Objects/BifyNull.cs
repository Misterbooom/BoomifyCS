using System;
using BoomifyCS.Lexer;

namespace BoomifyCS.Objects
{
    public class BifyNull : BifyObject
    {
        public object Value;

        public BifyNull() : base()
        {
            this.Value = null;
        }

        public override BifyString ObjectToString() => new BifyString($"{Value}");

        public override BifyString Repr() => new BifyString($"BifyNull({Value})");

        public override BifyObject Eq(BifyObject other)
        {
            if (other is BifyNull)
            {
                return new BifyBoolean(true);
            }
            return new BifyBoolean(false);
        }

        public override BifyObject Neq(BifyObject other)
        {
            if (other is BifyNull)
            {
                return new BifyBoolean(false);
            }
            return new BifyBoolean(true);
        }
        public override BifyBoolean Bool() => new BifyBoolean(false);
        public override BifyInteger Int() => new BifyInteger(0);
        public override string ToString() => ObjectToString().Value;
    }
}
