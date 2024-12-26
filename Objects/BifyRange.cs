using System;
using System.Collections.Generic;
using BoomifyCS.Lexer;

namespace BoomifyCS.Objects
{
    class BifyRange : BifyObject
    {
        public BifyInteger First;
        public BifyInteger Second ;

        public BifyRange(BifyInteger first, BifyInteger second)
            : base()
        {
            First = first;
            Second = second;
        }
        public BifyRange()
            : base()
        {
        }
        public override BifyString Repr() => new BifyString(ToString());
        public override string ToString() => $"BifyRange({First}..{Second})";
        public override int GetInitializerArgs() => 2;
        public override void Initialize(List<BifyObject> args)
        {
            First = args[0].Int();
            Second = args[1].Int();
            

        }

    }
}
