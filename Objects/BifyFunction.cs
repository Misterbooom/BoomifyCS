using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomifyCS.Objects;
using BoomifyCS.Lexer;
namespace BoomifyCS.Objects
{
    public class BifyFunction : BifyObject
    {
        public  string Name;
        public  BifyObject returnObject;
        public List<string> arguments = [];
        public BifyFunction(string name) : base()
        {
            Name = name;
            returnObject = new BifyNull();
            ExpectedArgCount = -1;

        }
        public override BifyObject Call(List<BifyObject> arguments)
        {
            return returnObject;
        }
        public override BifyString Repr()
        {
            if (arguments.Count > 0)
            {
                return new BifyString($"<{Name} Function, Args - [{string.Join(",",arguments)}]>");
            }
            return new BifyString($"<{Name} Function>");
        }
        public override string ToString()
        {
            return ObjectToString().Value;
        }
    }
}
