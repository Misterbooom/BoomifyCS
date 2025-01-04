using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomifyCS.Ast.Validators;
using BoomifyCS.Exceptions;
using BoomifyCS.Lexer;
using BoomifyCS.Parser;
using NUnit.Framework;

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
            AstReturn returnNode = new(token,builder.ParseCondition(returnTokens));
            builder.AddOperand(returnNode);
            builder.tokenIndex += returnTokens.Count;
            ReturnValidator.Validate(returnNode);
        }
    }
}
