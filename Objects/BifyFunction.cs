using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomifyCS.Objects;
using BoomifyCS.Lexer;
namespace BoomifyCS.BuiltIn.Function
{
    public class BifyFunction : BifyObject
    {
        public  string Name;
        public  BifyObject returnObject;
        public BifyFunction(string name) : base(new Token(TokenType.CALL,name))
        {
            Name = name;
            returnObject = new BifyNull(new Token(TokenType.NULL,"null") );
            ExpectedArgCount = -1;

        }
        public override BifyObject Call(List<BifyObject> arguments)
        {
            return returnObject;
        }
        public override BifyString Repr()
        {
            return new BifyString($"<{Name} Function>");
        }
        public override string ToString()
        {
            return ObjectToString().Value;
        }
    }
}
