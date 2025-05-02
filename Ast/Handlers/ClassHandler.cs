using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomifyCS.Exceptions;
using BoomifyCS.Lexer;

namespace BoomifyCS.Ast.Handlers
{
    class ClassHandler(AstBuilder builder) : TokenHandler(builder)
    {
        public override void HandleToken(Token token)
        {
            Token identifierToken = builder.GetNextToken();
            if (identifierToken?.Type != TokenType.IDENTIFIER)
            {
                new BifySyntaxError($"Invalid class name: '{identifierToken?.Value ?? "null"}'.").Throw();
            }
            AstNode nameNode = new AstIdentifier(identifierToken, identifierToken.Value);
            builder.NextToken();
            AstNode bodyNode = builder.HandleBody("Class");
            VerifyClassBody((AstBlock)bodyNode);
            builder.CurrentNode = new AstClass(token, nameNode, bodyNode);
            builder.AddType(identifierToken.Value);
        }
        private void VerifyClassBody(AstBlock body)
        {
            int methodsCount = 0;
            foreach (AstNode child in body.ChildNodes)
            {
                Traceback.Instance.SetCurrentLine(child.LineNumber);
                if (child is AstVarDecl)
                {
                    if (methodsCount > 0)
                        new BifySyntaxError("Class attributes must be defined before methods in the class body.").Throw();
                }
                else if (child is AstFunctionDecl)
                {
                    methodsCount++;
                }
                else
                {
                    new BifySyntaxError($"Invalid statement in class body. Only methods and variables are allowed. " +
                        $"Not a {child.GetType().Name.Replace("Ast", "").ToLower()}.").Throw();
                }
            }
        }
    }
}
