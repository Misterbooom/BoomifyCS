using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomifyCS.Lexer;
namespace BoomifyCS.Objects
{
    public class BifyVar:BifyObject
    {
        public string Name { get; set; }
        public BifyVar(string name):base(new Token(TokenType.IDENTIFIER,name)) {
            Name = name;
            
        }
        public override string ToString()
        {
            return Name;
        }

    }
}
