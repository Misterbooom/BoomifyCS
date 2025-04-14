using System;
using System.Collections.Generic;
using BoomifyCS.Ast.Handlers;
using BoomifyCS.Exceptions;
using BoomifyCS.Lexer;

namespace BoomifyCS.Ast
{
    class BinaryOperatorHandler : TokenHandler
    {
        public BinaryOperatorHandler(AstBuilder builder) : base(builder) { }

        public override void HandleToken(Token token)
        {

        }
       
    }
}
