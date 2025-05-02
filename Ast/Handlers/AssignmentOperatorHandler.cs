using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomifyCS.Exceptions;
using BoomifyCS.Lexer;
using BoomifyCS.Parser;

namespace BoomifyCS.Ast.Handlers
{
    class AssignmentOperatorHandler(AstBuilder builder) : TokenHandler(builder)
    {
        public override void HandleToken(Token token)
        {
            if (builder.Nodes.Count == 0)
            {
                new BifySyntaxError("Assignment operator used without a preceding expression. Please ensure an identifier is present before '='.").Throw();
            }
            if (builder.Nodes.Count == 2 || builder.Nodes.Count == 3)
            {
                new VariableDeclarationHandler(builder).HandleToken(token);
            }
            else if (builder.Nodes.Count == 1)
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
            var node = builder.Nodes[0];
            if (node is AstIdentifier || node is AstIndexOperator || node is AstMemberAccess)
            {
                var identifierNode = node; // Assign the node to a variable named identifierNode
                builder.NextToken();
                var valueTokens = builder.tokens[builder.tokenIndex..];
                var valueNode = builder.ParseTokens(valueTokens);
                if (valueNode == null)
                {
                    new BifySyntaxError(ErrorMessage.EmptyValueAssigned()).Throw();
                }
                builder.Nodes.Clear();
                builder.CurrentNode = new AstAssignmentOperator(token, identifierNode, valueNode);
                builder.MoveToEnd();
            }
            else
            {
                new BifySyntaxError("Assignment operator must follow an identifier or index operator or member access.").Throw();
            }
        }
    }
}
