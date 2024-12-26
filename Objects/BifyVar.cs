using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomifyCS.Lexer;
namespace BoomifyCS.Objects
{
    public class BifyVar(string name) : BifyObject()
    {
        public string Name { get; set; } = name;

        public override BifyString ObjectToString() => new BifyString($"{Name}");
        public override string ToString() => ObjectToString().Value;
    }
}
