using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomifyCS.Lexer;

namespace BoomifyCS.Objects
{
    public class BifyObject
    {
        public Token Token;
        public BifyObject(Token token) 
        {
            this.Token = token; 
        }
    }
}
