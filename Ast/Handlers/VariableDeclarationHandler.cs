using System.Collections.Generic;
using BoomifyCS.Ast.Validators;
using BoomifyCS.Exceptions;
using BoomifyCS.Lexer;

namespace BoomifyCS.Ast.Handlers
{
    class VariableDeclarationHandler(AstBuilder builder) : TokenHandler(builder)
    {
        public override void HandleToken(Token token)
        {
            GetVariableInfo(Builder,out AstNode identifierNode, out AstNode typeNode, out AstFlag flagNode);
          
            AstNode valueNode = null;
            List<Token> valueTokens = [];

            if (!Builder.IsAtEnd())
            {
                valueTokens = Builder.Tokens[Builder.TokenIndex..];
                valueNode = Builder.ParseTokens(valueTokens);
                if (valueNode == null)
                {
                    new BifySyntaxError(ErrorMessage.EmptyValueAssigned()).Throw();
                }

            }
            Builder.MoveToEnd();

            VariableDeclarationValidator.Validate(identifierNode, typeNode, valueNode, valueTokens, flagNode, token);
            AstAssignment astAssignment = new(token, identifierNode, valueNode);
            Builder.Nodes.Clear();
            Builder.CurrentNode = new AstVarDecl(token,astAssignment,typeNode,flagNode);




        }


    }
}
