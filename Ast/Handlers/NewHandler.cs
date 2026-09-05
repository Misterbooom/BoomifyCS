using BoomifyCS.Exceptions;
using BoomifyCS.Lexer;

namespace BoomifyCS.Ast.Handlers
{
    class NewHandler(AstBuilder builder):TokenHandler(builder)
    {
        public override void HandleToken(Token token)
        {
            Builder.CurrentNode = ParseNewExpression(token);
        }
        public AstNode ParseNewExpression(Token token)
        {
            Builder.NextToken();
            AstNode nextNode = new BinaryOperatorHandler(Builder).ParsePrimary();
            if (nextNode is not AstCall)
            {
                new BifySyntaxError("A new expression requires an argument list.").Throw();
            }
            AstCall astCall = (AstCall)nextNode;
            AstNew astNew = new AstNew(token, astCall.CallableName, astCall.ArgumentsNode);
            return astNew;

        }
    }
}
