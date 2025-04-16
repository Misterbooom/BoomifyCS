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
            GetVariableInfo(builder,out AstNode identifierNode, out AstNode typeNode, out AstNode flagNode);
            AstNode valueNode = null;
            List<Token> valueTokens = [];

           
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
            builder.MoveToEnd();

            VariableDeclarationValidator.Validate(identifierNode, typeNode, valueNode, valueTokens, flagNode, token);
            AstAssignment astAssignment = new(token, identifierNode, valueNode);
            builder.Nodes.Clear();
            builder.CurrentNode = new AstVarDecl(token,astAssignment,typeNode,flagNode);




        }


    }
}
