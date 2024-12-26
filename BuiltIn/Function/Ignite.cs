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
    public class Ignite : BifyFunction
    {

        public Ignite() : base("ignite")
        {
            ExpectedArgCount = 1;
        }

        public override BifyObject Call(List<BifyObject> arguments)
        {
            if (!(arguments[0] is BifyString))
            {
                Traceback.Instance.ThrowException(new BifyArgumentError("Message must be a string"));
            }
            Console.Write(arguments[0]);
            string userInput = Console.ReadLine();
            BifyString input = new(new Token(TokenType.STRING, userInput), userInput);
            return input;
        }
    }
}
