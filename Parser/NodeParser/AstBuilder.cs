using BoomifyCS.Ast;
using BoomifyCS.Lexer;
using System.Collections.Generic;
using BoomifyCS.Exceptions;
using BoomifyCS.Interpreter.VM;
using BoomifyCS.Interpreter;
using System.Diagnostics.Metrics;
using BoomifyCS.Parser.StatementParser;

namespace BoomifyCS.Parser.NodeParser
{
    public static class AstBuilder
    {
        public static AstNode TokenToAst(List<Token> tokens, string modulePath, ref int lineCount)
        {
            var ast = new AstTree(modulePath)
            {
                lineCount = lineCount
            };
            var resultNode = ast.ParseTokens(tokens);
            lineCount = ast.lineCount;
            return resultNode;

        }

        public static AstNode TokenToAst(Token token, string modulePath)
        {
            var ast = new AstTree(modulePath);
            return ast.ParseTokens([token]);
        }

        public static AstNode BuildTokens(Token token, string modulePath)
        {
            var ast = new AstTree(modulePath);
            return ((AstModule)ast.ParseTokens([token])).ChildNode;
        }

        public static AstNode BuildTokens(List<Token> tokens, string modulePath)
        {
            var ast = new AstTree(modulePath);
            return ((AstModule)ast.ParseTokens(tokens)).ChildNode;
        }
        public static void BuildFromStringAndRunVM(string str)
        {

            MyLexer lexer = new(str);


            List<Token> tokens = lexer.Tokenize();

            string[] codeByLine = str.Split('\n');
            //for (int i = 0;i < codeByLine.Length;i++) {
            //    Console.WriteLine($"{i}:{codeByLine[i]}");  
            //}

            AstTree astParser = new("1", codeByLine);
            AstNode node = astParser.ParseTokens(tokens);



            //Console.WriteLine(string.Join("\n", codeByLine))
            MyCompiler interpreter = new(codeByLine);
            interpreter.RunVM(node);






        }
        public static AstNode ParseBlock(List<Token> tokens, ref int currentPos, AstTree astParser)
        {
            var (blockToken, blockEnd) = TokenFinder.FindTokenSafe(TokenType.BLOCK, tokens, currentPos);
            currentPos = blockEnd + 1;
            return astParser.BuildAstTree([blockToken]);
        }
        public static AstNode ParseCondition(List<Token> tokens, ref int currentPos, AstTree astParser)
        {
            var (conditionTokens, conditionEnd) = TokenFinder.FindTokensInBracketsSafe(tokens, currentPos,ErrorMessage.ConditionIsRequired());
            currentPos = conditionEnd + 1;
            return astParser.BuildAstTree(conditionTokens);
        }
    }
}