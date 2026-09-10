using System.Collections.Generic;
using BoomifyCS.Exceptions;
using BoomifyCS.Lexer;

namespace BoomifyCS.Ast.Validators
{
    internal static class VariableDeclarationValidator
    {
        public static void Validate(AstNode identifierNode, AstNode typeNode,
            AstNode valueNode, List<Token> valueTokens, AstNode flagNode, Token assignmentToken)
        {
            BifyDebug.Assert(identifierNode != null && typeNode != null, "identifierNode or typeNode is null");
            if (identifierNode is not AstIdentifier)
            {
                BifyNameError error = new(ErrorMessage.InvalidVariableName(identifierNode?.Token.Value),
                    "", identifierNode?.Token.Value);
                Traceback.Instance.ThrowException(error, identifierNode?.Token.Column ?? assignmentToken.Column);
            }

            if (typeNode != null && (typeNode is AstIdentifier or AstIndexOperator ||
                                     typeNode.Token.Type == TokenType.POINTER)) return;
            BifyTypeError bifyTypeError = new(ErrorMessage.InvalidVariableType(typeNode?.Token.Value),
                "", typeNode?.Token.Value);
            Traceback.Instance.ThrowException(bifyTypeError, typeNode?.Token.Column ?? assignmentToken.Column);

        }
    }

    internal class AssignmentOperatorValidator
    {
        public static bool Validate(Token variableToken, Token assignmentToken, List<Token> valueTokens)
        {
            if (valueTokens.Count == 0)
            {
                BifySyntaxError error = new(ErrorMessage.EmptyValueAssigned());
                Traceback.Instance.ThrowException(error, assignmentToken.Column);
            }
            if (variableToken != null && (variableToken.Type == TokenType.IDENTIFIER || variableToken.Type == TokenType.INDEX_OPERATOR))
            {
                if (assignmentToken != null)
                {
                    return true;
                }
                else
                {
                    BifySyntaxError error = new(ErrorMessage.MissingAssignmentOperator());
                    Traceback.Instance.ThrowException(error, variableToken.Column);
                }
            }
            else
            {
                BifySyntaxError error = new(ErrorMessage.InvalidVariableName(variableToken?.Value));
                Traceback.Instance.ThrowException(error, assignmentToken.Column);
            }

            return false;
        }
    }


}
