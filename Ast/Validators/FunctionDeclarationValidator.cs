using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomifyCS.Exceptions;
using BoomifyCS.Lexer;

namespace BoomifyCS.Ast.Validators
{
    class FunctionDeclarationValidator
    {
        public static void Validate(Token funcToken, Token nameToken, AstNode parametersNode, AstNode blockNode)
        {
            if (nameToken == null || nameToken.Type != TokenType.IDENTIFIER)
            {
                BifyError bifyError = new BifySyntaxError(ErrorMessage.ExpectedFunctionName(), "", nameToken == null ? funcToken.Value : nameToken.Value);
                Traceback.Instance.ThrowException(bifyError, nameToken == null ? funcToken.Column : nameToken.Column);
            }
            var result = TravelParameters(parametersNode);
            if (!result.isCorrect && parametersNode != null)
            {
                BifyError bifyError = new BifySyntaxError(ErrorMessage.InvalidParameter(result.token.Type.ToString()), "", result.token.Value);
                Traceback.Instance.ThrowException(bifyError, result.token.Column);
            }


        }
        private static (bool isCorrect, Token token) TravelParameters(AstNode node)
        {
            if (node is null)
            {
                return (false, null);
            }
            if (node is AstBinaryOp binaryOp)
            {
                if (binaryOp.Token.Type == TokenType.COMMA)
                {
                    var leftResult = TravelParameters(binaryOp.Left);
                    var rightResult = TravelParameters(binaryOp.Right);

                    if (!leftResult.isCorrect)
                        return (false, leftResult.token ?? binaryOp.Token);

                    if (!rightResult.isCorrect)
                        return (false, rightResult.token ?? binaryOp.Token);


                    return (true, binaryOp.Token);
                }
                return (false, binaryOp.Token);
            }

            return (node is AstIdentifier, node.Token);
        }

    }
}
