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
            string code =
@"
var a = '12';
";
            MyLexer lexer = new MyLexer(code);
            List<Token> tokens = lexer.Tokenize();
            tokens.WriteTokens();
            AstTree astParser = new AstTree();
            string[] callStack = {"Main"};
            
            BifyException exception =  new BifyException("Test Exception", 1, callStack, tokens, tokens.GetRange(1,3));
            exception.PrintException();
            //astParser.ParseTokens(tokens);
        }


    }
}