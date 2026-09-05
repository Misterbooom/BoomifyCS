using BoomifyCS.Exceptions;
using BoomifyCS.Lexer;

namespace BoomifyCS.Ast.Handlers
{
    class AssignmentOperatorHandler(AstBuilder builder) : TokenHandler(builder)
    {
        public override void HandleToken(Token token)
        {
            if (Builder.Nodes.Count == 0)
            {
                new BifySyntaxError("Assignment operator used without a preceding expression. Please ensure an identifier is present before '='.").Throw();
            }
            if (Builder.Nodes.Count == 2 || Builder.Nodes.Count == 3)
            {
                new VariableDeclarationHandler(Builder).HandleToken(token);
            }
            else if (Builder.Nodes.Count == 1)
            {
                HandleAssignment(token);
            }
            else
            {
                new BifySyntaxError("Unexpected use of the assignment operator. Check the syntax and try again.").Throw();
            }
        }
        private void HandleAssignment(Token token)
        {
            var node = Builder.Nodes[0];
            if (node is AstIdentifier || node is AstIndexOperator || node is AstMemberAccess)
            {
                var identifierNode = node; // Assign the node to a variable named identifierNode
                Builder.NextToken();
                var valueTokens = Builder.Tokens[Builder.TokenIndex..];
                var valueNode = Builder.ParseTokens(valueTokens);
                if (valueNode == null)
                {
                    new BifySyntaxError(ErrorMessage.EmptyValueAssigned()).Throw();
                }
                Builder.Nodes.Clear();
                Builder.CurrentNode = new AstAssignmentOperator(token, identifierNode, valueNode);
                Builder.MoveToEnd();
            }
            else
            {
                new BifySyntaxError("Assignment operator must follow an identifier or index operator or member access.").Throw();
            }
        }
    }
}
