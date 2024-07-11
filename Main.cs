using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomifyCS.Ast;
using BoomifyCS.Lexer;

namespace BoomifyCS
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string code =
@"
2 + 2       
"; 
            MyLexer lexer = new MyLexer(code);
            List<Token> tokens = lexer.Tokenize();
            Console.WriteLine(tokens.ToArray());
        }


    }
}