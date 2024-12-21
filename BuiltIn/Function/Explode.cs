using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomifyCS.Objects;

namespace BoomifyCS.BuiltIn.Function
{
    public class Explode : BifyFunction
    {

        public Explode() : base("explode") => ExpectedArgCount = -1;
        public override BifyObject Call(List<BifyObject> arguments)
        {
            foreach (BifyObject bifyObject in arguments)
            {
                Console.Write($"{bifyObject} ");
            }
            Console.WriteLine();
            return new BifyNull();
        }
    }
}
