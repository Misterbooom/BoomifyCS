using System;
using BoomifyCS.Lexer;

namespace BoomifyCS.Ast.Handlers
{
    abstract class TokenHandler
    {
        protected AstBuilder builder;

        protected TokenHandler(AstBuilder builder)
        {
            this.builder = builder;
        }

        public abstract void HandleToken(Token token);
    }
}
