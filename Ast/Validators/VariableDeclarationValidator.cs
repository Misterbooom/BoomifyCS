using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomifyCS.Exceptions;
using BoomifyCS.Lexer;
using BoomifyCS.Parser;
namespace BoomifyCS.Ast.Validators
{
    class VariableDeclarationValidator
    {
        public static void Validate(AstNode identifierNode, AstNode typeNode,
            AstNode valueNode, List<Token> valueTokens, Token assignmentToken)
        {
            if (valueTokens.Count == 0 || valueNode == null)
            {
                BifySyntaxError error = new(ErrorMessage.EmptyValueAssigned());
                Traceback.Instance.ThrowException(error, assignmentToken.Column);

            }
            if (identifierNode is not AstIdentifier)
            {
                BifyNameError error = new(ErrorMessage.InvalidVariableName(identifierNode?.Token.Value),
                    "",identifierNode?.Token.Value);
                Traceback.Instance.ThrowException(error, identifierNode?.Token.Column ?? assignmentToken.Column);
            }
            if (typeNode is not AstIdentifier)
            {
                BifyTypeError bifyTypeError = new(ErrorMessage.InvalidVariableType(typeNode?.Token.Type.ToString().ToLower()),
                    "",typeNode?.Token.Value);
                Traceback.Instance.ThrowException(bifyTypeError, typeNode?.Token.Column ?? assignmentToken.Column);
            }

        }
       
    }
    class AssignmentOperatorValidator
    {
        public static bool Validate(Token variableToken, Token assignmentToken, List<Token> valueTokens)
        {
            if (valueTokens.Count == 0)
            {
                BifySyntaxError error = new(ErrorMessage.EmptyValueAssigned());
                Traceback.Instance.ThrowException(error, assignmentToken.Column);

            }
            if (variableToken != null && variableToken.Type == TokenType.IDENTIFIER || variableToken.Type == TokenType.INDEX_OPERATOR)
            {
                if (assignmentToken != null && assignmentToken.Type == TokenType.ASSIGN)
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
