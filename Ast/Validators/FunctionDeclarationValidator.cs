using BoomifyCS.Exceptions;
using BoomifyCS.Lexer;

namespace BoomifyCS.Ast.Validators
{
    class FunctionDeclarationValidator
    {
        public static void Validate(Token nameToken, AstNode parametersNode, AstNode blockNode, AstNode typeNode)
        {
            if (nameToken == null || nameToken.Type != TokenType.IDENTIFIER)
            {
                BifyError bifyError = new BifySyntaxError(ErrorMessage.ExpectedFunctionName(), "", nameToken == null ? typeNode.Token.Value : nameToken.Value);
                Traceback.Instance.ThrowException(bifyError, nameToken == null ? typeNode.Token.Column : nameToken.Column);
            }

            if (typeNode is not AstIdentifier && typeNode.Token.Type != TokenType.POINTER && typeNode is not AstIndexOperator)
            {
                BifyTypeError bifyTypeError = new(ErrorMessage.InvalidFunctionType());
                Traceback.Instance.ThrowException(bifyTypeError, typeNode?.Token.Column ?? nameToken.Column);
            }

          
        }

       
    }
}
