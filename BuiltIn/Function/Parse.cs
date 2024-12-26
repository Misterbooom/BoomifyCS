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
    public class Parse : BifyFunction
    {

        public Parse() : base("parse") => ExpectedArgCount = 2;
        public override BifyObject Call(List<BifyObject> arguments)
        {
            BifyObject value = arguments[0];
            string type = ((BifyString)arguments[1]).Value;
            if (type == "string")
            {
                return new BifyString(value.ToString());
            }
            else if (type == "integer")
            {
                return value.Int();
            }
            else if (type == "float" && arguments[0] is BifyString bifyString)
            {
                
                 return BifyFloat.Convert(bifyString);
               
            }
            
            return new BifyNull();
        }
    }
}
