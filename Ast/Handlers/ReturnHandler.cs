using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomifyCS.Lexer;
using BoomifyCS.Parser;

namespace BoomifyCS.Ast.Handlers
{
    class ReturnHandler : TokenHandler
    {
        public ReturnHandler(AstBuilder builder) : base(builder)
        {
        }
        public override void HandleToken(Token token)
        {
            builder.tokenIndex++;
            List<Token> returnTokens = builder.tokens[builder.tokenIndex..];
            returnTokens.WriteTokens();
            AstReturn returnNode = new(token,builder.ParseCondition(returnTokens));
            builder.AddOperand(returnNode);
            builder.tokenIndex += returnTokens.Count;
        }
    }
}
