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
                try
                {
                    return new BifyFloat(double.Parse(bifyString.Value));
                }
                catch (FormatException)
                {
                    Traceback.Instance.ThrowException(new BifyCastError($"The value '{bifyString}' is not in a correct format for an float. "));
                }
                catch (OverflowException)
                {
                    Traceback.Instance.ThrowException(new BifyCastError($"The value '{bifyString}' is too large or too small for an float."));
                }
                catch (ArgumentException)
                {
                    Traceback.Instance.ThrowException(new BifyCastError($"An argument exception occurred while parsing '{bifyString}'."));
                }
                catch (Exception)
                {
                    Traceback.Instance.ThrowException(new BifyCastError($"An unexpected error occurred while parsing '{bifyString}'."));
                }
            }
            
            return new BifyNull();
        }
    }
}
