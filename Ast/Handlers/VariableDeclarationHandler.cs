using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomifyCS.Ast.Validators;
using BoomifyCS.Exceptions;
using BoomifyCS.Lexer;

namespace BoomifyCS.Ast.Handlers
{
    class VariableDeclarationHandler(AstBuilder builder) : TokenHandler(builder)
    {
        public override void HandleToken(Token token)
        {

            GetVariableInfo(out AstNode identifierNode, out AstNode typeNode, out AstNode flagNode);
            AstNode valueNode = null;
            List<Token> valueTokens = [];

            if (identifierNode is AstUnaryOperator unaryOperator)
            {
                var (lastOperand,finalPointer) = SwitchLastOperand(unaryOperator, identifierNode);
                identifierNode = lastOperand;
                typeNode = finalPointer;
            }
            if (token.Type == TokenType.ASSIGN)
            {
                builder.NextToken();
                valueTokens = builder.tokens[builder.tokenIndex..];
                valueNode = builder.ParseTokens(valueTokens);
                if (valueNode == null)
                {
                    new BifySyntaxError(ErrorMessage.EmptyValueAssigned()).Throw();
                }

            }
            builder.tokenIndex = builder.tokens.Count;

            VariableDeclarationValidator.Validate(identifierNode, typeNode, valueNode, valueTokens, flagNode, token);
            AstAssignment astAssignment = new(token, identifierNode, valueNode);
            builder.Nodes.Clear();
            builder.CurrentNode = new AstVarDecl(token,astAssignment,typeNode,flagNode);


        }

        private void GetVariableInfo(out AstNode identifierNode, out AstNode typeNode, out AstNode flagNode)
        {
            if (builder.Nodes.Count == 2)
            {
                typeNode = builder.Nodes[0];
                identifierNode = builder.Nodes[1];
                flagNode = null;
            }
            else if (builder.Nodes.Count == 3)
            {
                flagNode = builder.Nodes[0];
                typeNode = builder.Nodes[1];
                identifierNode = builder.Nodes[2];
            }
            else
            {
                identifierNode = null;
                typeNode = null;
                flagNode = null;
            }
        }
    }
}
