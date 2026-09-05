using System.Collections.Generic;
using BoomifyCS.Exceptions;
using BoomifyCS.Lexer;

namespace BoomifyCS.Ast
{
    class AstTree
    {
        public AstTree(string[] sourceCode)
        {
            Traceback.Instance.InitializeSource(sourceCode);
        }

        public AstModule ParseTokens(List<Token> tokens)
        {
            int tokenIndex = 0;
            int lineCount = 0;
            List<AstNode> lines = [];
            foreach(var lineTokens in TokensFormatter.SplitLines(tokens))
            {
                lines.Add(ParseLine(lineTokens, ref tokenIndex, ref lineCount));
            }
            AstModule module = new("","",lines);
            return module;
        }
        private AstNode ParseLine(List<Token> line, ref int tokenIndex, ref int lineCount)
        {
            AstBuilder builder = new(line);
            return builder.BuildNode();
        }

    }
}