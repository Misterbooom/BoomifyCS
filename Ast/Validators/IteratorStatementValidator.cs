using System;
using System.Collections.Generic;
using BoomifyCS.Exceptions;
using BoomifyCS.Lexer;

namespace BoomifyCS.Ast.Validators
{
    class IteratorStatementValidator
    {
        public static void ValidateWhileStatement(List<Token> conditionTokens, AstWhile astWhile)
        {
            if (conditionTokens.Count == 0)
            {
                BifySyntaxError error = new(ErrorMessage.ConditionIsRequired(), "", astWhile.Token.Value);
                Traceback.Instance.ThrowException(error, astWhile.Token.Column);
            }
        }

        public static void ValidateForStatement(List<List<Token>> splitedTokens, AstFor astFor)
        {
            // Ensure the for-loop structure has exactly three parts: init, condition, increment
            if (splitedTokens.Count != 3)
            {
                BifySyntaxError error = new(ErrorMessage.InvalidForLoopStructure(), "", astFor.Token.Value);
                Traceback.Instance.ThrowException(error, astFor.Token.Column);
            }

            List<Token> initTokens = splitedTokens[0];
            List<Token> conditionTokens = splitedTokens[1];
            List<Token> incrementTokens = splitedTokens[2];

            // Validate each part is not empty
            if (initTokens.Count == 0)
            {
                BifySyntaxError error = new(ErrorMessage.InitStatementIsRequired(), "", astFor.Token.Value);
                Traceback.Instance.ThrowException(error, astFor.Token.Column);
            }
            if (conditionTokens.Count == 0)
            {
                BifySyntaxError error = new(ErrorMessage.ConditionIsRequired(), "", astFor.Token.Value);
                Traceback.Instance.ThrowException(error, astFor.Token.Column);
            }
            if (incrementTokens.Count == 0)
            {
                BifySyntaxError error = new(ErrorMessage.IncrementStatementIsRequired(), "", astFor.Token.Value);
                Traceback.Instance.ThrowException(error, astFor.Token.Column);
            }

            // Validate the nodes are set
            if (astFor.InitNode == null)
            {
                BifySyntaxError error = new(ErrorMessage.InvalidInitNode(), "", astFor.Token.Value);
                Traceback.Instance.ThrowException(error, astFor.Token.Column);
            }
            else if (!IsValidInitNode(astFor.InitNode))
            {
                BifySyntaxError error = new(ErrorMessage.InvalidInitNodeType(), "", astFor.Token.Value);
                Traceback.Instance.ThrowException(error, astFor.Token.Column);
            }

            if (astFor.ConditionNode == null)
            {
                BifySyntaxError error = new(ErrorMessage.InvalidConditionNode(), "", astFor.Token.Value);
                Traceback.Instance.ThrowException(error, astFor.Token.Column);
            }
            else if (!IsValidConditionNode(astFor.ConditionNode))
            {
                BifySyntaxError error = new(ErrorMessage.InvalidConditionNodeType(), "", astFor.Token.Value);
                Traceback.Instance.ThrowException(error, astFor.Token.Column);
            }

            if (astFor.IncrementNode == null)
            {
                BifySyntaxError error = new(ErrorMessage.InvalidIncrementNode(), "", astFor.Token.Value);
                Traceback.Instance.ThrowException(error, astFor.Token.Column);
            }
            else if (!IsValidIncrementNode(astFor.IncrementNode))
            {
                BifySyntaxError error = new(ErrorMessage.InvalidIncrementNodeType(), "", astFor.Token.Value);
                Traceback.Instance.ThrowException(error, astFor.Token.Column);
            }

            // Validate the block node is set
            if (astFor.BlockNode == null)
            {
                BifySyntaxError error = new(ErrorMessage.InvalidForLoopStructure(), "", astFor.Token.Value);
                Traceback.Instance.ThrowException(error, astFor.Token.Column);
            }
        }

        public static bool IsValidInitNode(AstNode node)
        {
            return node is AstVarDecl || node is AstAssignmentOperator;
        }

        public static bool IsValidIncrementNode(AstNode node)
        {
            return node is AstAssignmentOperator || node is AstUnaryOperator;
        }

        public static bool IsValidConditionNode(AstNode node)
        {
            return node is AstBinaryOp || node is AstUnaryOperator ||
                   node is AstIdentifier || node is AstCall || node is AstIndexOperator;
        }

    }
}

