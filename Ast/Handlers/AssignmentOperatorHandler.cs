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
            if (node is AstIdentifier identifierNode)
            {
                builder.NextToken();
                var valueTokens = builder.tokens[builder.tokenIndex..];
                BifyDebug.Log($"ValueTokens: {valueTokens.TokensToString()}");
                var valueNode = builder.ParseTokens(valueTokens);
                if (valueNode == null)
                {
                    new BifySyntaxError(ErrorMessage.EmptyValueAssigned()).Throw();
                }
                builder.Nodes.Clear();
                builder.CurrentNode = new AstAssignmentOperator(identifierNode.Token, identifierNode, valueNode);
                builder.tokenIndex = builder.tokens.Count;

            }
            else
            {
                new BifySyntaxError("Assignment operator must follow an identifier.").Throw();
            }
           
        }
    }
}
