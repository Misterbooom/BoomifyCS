using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomifyCS.Lexer;
namespace BoomifyCS.Objects
{
    public class BifyVar(string name) : BifyObject(new Token(TokenType.IDENTIFIER,name))
    {
        public string Name { get; set; } = name;

        public override string ToString()
        {
            return Name;
        }

    }
}
