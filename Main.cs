using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomifyCS.Ast;
using BoomifyCS.Lexer;
using BoomifyCS.Parser;

namespace BoomifyCS
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string code = @"(2 + 2) * 2;";
            MyLexer lexer = new MyLexer(code);
            try
            {
                List<Token> tokens = lexer.Tokenize();
                //tokens.WriteTokens();
                AstTree astParser = new AstTree();
                string[] callStack = { "Main" };

                AstNode node = astParser.ParseTokens(tokens);
                Console.WriteLine(AstParser.SimpleEval(node));
                Console.WriteLine(node);
            }
            catch (BifyException e) 
            {
                e.PrintException();
            }

            
            //astParser.ParseTokens(tokens);
        }


    }
}
