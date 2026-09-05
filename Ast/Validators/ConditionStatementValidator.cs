using BoomifyCS.Exceptions;

namespace BoomifyCS.Ast.Validators
{
    class ConditionStatementValidator
    {
    
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
