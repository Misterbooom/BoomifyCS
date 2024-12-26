using System;
using BoomifyCS.Exceptions;
using BoomifyCS.Lexer;

namespace BoomifyCS.Ast.Validators
{
    class IndexOperatorValidator
    {
        public static bool Validate(AstNode previousNode, AstNode indexNode)
        {
            if (previousNode == null || !IsIndexableNode(previousNode))
            {
                BifySyntaxError error = new(ErrorMessage.InvalidIndexTarget(), "", previousNode.Token.Value);
                Traceback.Instance.ThrowException(error, previousNode.Token.Column);
            }

            if (indexNode == null || !IsValidIndexNode(indexNode))
            {
                Console.WriteLine(indexNode.ToString());
                BifySyntaxError error = new(ErrorMessage.InvalidIndexExpression(), "", indexNode.Token.Value);
                Traceback.Instance.ThrowException(error, indexNode.Token.Column);
            }

            return true; // Validation successful
        }

        private static bool IsIndexableNode(AstNode node) => node is AstArray || node is AstIdentifier || node is AstCall;

        // Helper method to check if the index expression is valid
        private static bool IsValidIndexNode(AstNode node) => node is AstNumber || node is AstIdentifier ||
                node is AstRangeOperator ||
                node is AstBinaryOp && node.Token.Type != TokenType.COMMA;
    }
}
