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
            
        public Explode() :base("explode") { 
        }
        public override BifyObject Call(List<BifyObject> arguments)
        {
            foreach (BifyObject bifyObject in arguments)
            {
                Console.Write($"{bifyObject.ToString()} ");
            }
            Console.WriteLine();
            return new BifyNull();
        }
    }
}
