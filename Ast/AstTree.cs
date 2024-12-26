using System;
using System.Collections.Generic;
using System.IO;
using BoomifyCS.Ast;
using BoomifyCS.Exceptions;
using BoomifyCS.Lexer;
using BoomifyCS.Parser;
namespace BoomifyCS.Ast
{
    class AstTree
    {
        public AstTree(string[] sourceCode)
        {
            Traceback.Instance.InitializeSource(sourceCode);
        }

        public AstNode ParseTokens(List<Token> tokens)
        {
            int tokenIndex = 0;
            int lineCount = 0;
            List<AstNode> lines = [];
            while (tokenIndex < tokens.Count)
            {
                lines.Add(ParseLine(tokens, ref tokenIndex, ref lineCount));
            }
            //lines.WriteNodes();
            AstModule module = new("","",lines);
            return module;
        }
        private AstNode ParseLine(List<Token> tokens, ref int tokenIndex, ref int lineCount)
        {
            List<Token> line = TokensFormatter.NextLine(tokens, ref tokenIndex);
            AstBuilder builder = new(line);
            return builder.BuildNode();
        }

    }
}