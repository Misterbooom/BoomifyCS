using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomifyCS.Exceptions;
using BoomifyCS.Lexer;
using NUnit.Framework.Constraints;

namespace BoomifyCS.Ast.Validators
{
    class ConditionStatementValidator
    {
        public static void ValidateIfStatement(List<Token> blockTokens,List<Token> conditionTokens)
        {
            if (conditionTokens.Count == 0)
            {
                BifySyntaxError error = new(ErrorMessage.ConditionIsRequired());
                Traceback.Instance.ThrowException(error, 0);
            }
        }
        
        public static void ValidateElseIfStatement(List<Token> conditionTokens, Stack<AstNode> operandStack, AstElseIf astElseIf)
        {
            if (operandStack.Count == 0)
            {
                ThrowUnmatchedIfError(astElseIf);
            }
            else if (operandStack.TryPeek(out AstNode operand))
            {
                if (operand is AstIf astIf)
                {
                    if (astIf.ElseNode != null)
                    {
                        ThrowElseIfAfterElseError(astElseIf);
                    }
                }
                else
                {
                    ThrowUnmatchedIfError(astElseIf);
                }
            }

            if (conditionTokens.Count == 0)
            {
                BifySyntaxError error = new(ErrorMessage.ConditionIsRequired());
                Traceback.Instance.ThrowException(error, astElseIf.Token.Column);
            }
        }
        public static void ValidateElseStatement(Stack<AstNode> operandStack,AstElse astElse)
        {
            if (operandStack.Count == 0)
            {
                ThrowUnmatchedIfError(astElse);

            }
            else if (operandStack.TryPeek(out AstNode operand)) { 
                if (operand is AstIf astIf)
                {
                    if (astIf.ElseNode != null)
                    {
                        ThrowUnmatchedIfError(astElse);

                    }
                }
                else
                {
                    ThrowUnmatchedIfError(astElse);
                }
            }
        }
        public static void ThrowUnmatchedIfError(AstElse astElse)
        {
            BifySyntaxError error = new(ErrorMessage.ElseWithoutMatchingIf(), "", astElse.Token.Value);
            Traceback.Instance.ThrowException(error, astElse.Token.Column);
        }
        public static void ThrowUnmatchedIfError(AstElseIf astElseIf)
        {
            BifySyntaxError error = new(ErrorMessage.ElseIfWithoutMatchingIf(), "", astElseIf.Token.Value);
            Traceback.Instance.ThrowException(error, astElseIf.Token.Column);
        }

        public static void ThrowElseIfAfterElseError(AstElseIf astElseIf)
        {
            BifySyntaxError error = new(ErrorMessage.ElseIfCannotFollowElseDirectly(), "", astElseIf.Token.Value);
            Traceback.Instance.ThrowException(error, astElseIf.Token.Column);
        }
    }
}
