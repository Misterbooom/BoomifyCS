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
            string code = @"var a = null;";
            MyLexer lexer = new MyLexer(code);
            try
            {
                List<Token> tokens = lexer.Tokenize();
                //tokens.WriteTokens();
                AstTree astParser = new AstTree();
                string[] callStack = { "Main" };

                AstNode node = astParser.ParseTokens(tokens);
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
// Add separating lines by AstLine node,Add Base constant node , Add AstInt,AstString,AstBoolean,AstNull,AstAssignment,AstVarDecl,AstLine Node ,Fixed EOL token Add BifyBoolean Create StatementParser add some exceptions