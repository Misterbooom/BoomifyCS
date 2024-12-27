using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomifyCS.Exceptions;
using BoomifyCS.Lexer;
using BoomifyCS.Objects;

namespace BoomifyCS.BuiltIn.Function
{
    public class Len : BifyFunction
    {

        public Len() : base("len")
        {
            ExpectedArgCount = 1;
        }

        public override BifyObject Call(List<BifyObject> arguments)
        {
            BifyObject value = arguments[0];
        
            if (value is BifyArray array)
            {
                return array.Len();
            }
            return new BifyInteger(0);
        }
    }
}
