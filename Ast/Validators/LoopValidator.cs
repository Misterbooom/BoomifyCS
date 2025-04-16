using System;
using System.Collections.Generic;
using BoomifyCS.Exceptions;
using BoomifyCS.Lexer;

namespace BoomifyCS.Ast.Validators
{
    class LoopValidator
    {
        public static void ValidateWhileStatement(AstWhile astWhile)
        {
            if (astWhile.ConditionNode == null)
            {
                ThrowSyntaxError(ErrorMessage.ConditionIsRequired(), astWhile.Token);
            }
        }

        public static void ValidateForStatement(List<List<Token>> splitedTokens, AstFor astFor)
        {
            if (splitedTokens.Count != 3)
            {
                ThrowSyntaxError(ErrorMessage.InvalidForLoopStructure(), astFor.Token);
            }

            ValidateTokens(splitedTokens[0], ErrorMessage.InitStatementIsRequired(), astFor.Token);
            ValidateTokens(splitedTokens[1], ErrorMessage.ConditionIsRequired(), astFor.Token);
            ValidateTokens(splitedTokens[2], ErrorMessage.IncrementStatementIsRequired(), astFor.Token);

            ValidateNode(astFor.InitNode, IsValidInitNode, ErrorMessage.InvalidInitNode(), ErrorMessage.InvalidInitNodeType(), astFor.Token);
            ValidateNode(astFor.ConditionNode, IsValidConditionNode, ErrorMessage.InvalidConditionNode(), ErrorMessage.InvalidConditionNodeType(), astFor.Token);
            ValidateNode(astFor.IncrementNode, IsValidIncrementNode, ErrorMessage.InvalidIncrementNode(), ErrorMessage.InvalidIncrementNodeType(), astFor.Token);

            if (astFor.BlockNode == null)
            {
                ThrowSyntaxError(ErrorMessage.InvalidForLoopStructure(), astFor.Token);
            }
        }

        private static void ValidateTokens(List<Token> tokens, string errorMessage, Token token)
        {
            if (tokens.Count == 0)
            {
                ThrowSyntaxError(errorMessage, token);
            }
        }

        private static void ValidateNode(AstNode node, Func<AstNode, bool> isValid, string nullError, string typeError, Token token)
        {
            if (node == null)
            {
                ThrowSyntaxError(nullError, token);
            }
            else if (!isValid(node))
            {
                ThrowSyntaxError(typeError, token);
            }
        }

        private static void ThrowSyntaxError(string message, Token token)
        {
            var error = new BifySyntaxError(message, "", token.Value);
            Traceback.Instance.ThrowException(error, token.Column);
        }

        public static bool IsValidInitNode(AstNode node) =>
            node is AstVarDecl || node is AstAssignmentOperator;

        public static bool IsValidIncrementNode(AstNode node) =>
            node is AstAssignmentOperator || node is AstUnaryOperator;

        public static bool IsValidConditionNode(AstNode node) =>
            node is AstBinaryOp || node is AstUnaryOperator ||
            node is AstIdentifier || node is AstCall ||
            node is AstIndexOperator;
    }
}