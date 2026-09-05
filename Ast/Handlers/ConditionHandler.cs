using BoomifyCS.Ast.Validators;
using BoomifyCS.Exceptions;
using BoomifyCS.Lexer;

namespace BoomifyCS.Ast.Handlers
{
    class ConditionHandler(AstBuilder builder) : TokenHandler(builder)
    {
        AstIf _currentIfNode = null;
        public override void HandleToken(Token token)
        {
            HandleIf(token);
            Builder.CurrentNode = _currentIfNode;
        }
        private void HandleIf(Token token)
        {
            AstNode conditionNode = Builder.ParseTokens(Builder.GetConditionTokens());
            AstNode blockNode = Builder.HandleBody("If");
            if (conditionNode == null)
            {
                new BifySyntaxError(ErrorMessage.ConditionIsRequired()).Throw();
                return;
            }
            _currentIfNode = new AstIf(token, conditionNode, blockNode);

            HandleElseOrElseIf(Builder.Peek());
        }
        private void HandleElseOrElseIf(Token token)
        {

            
            if (token?.Type == TokenType.ELSE && Builder.GetNextToken()?.Type == TokenType.IF)
            {
                Builder.TokenIndex++; // skip if token
                AstNode conditionNode = Builder.ParseTokens(Builder.GetConditionTokens());
                AstNode blockNode = Builder.HandleBody("Else-If");
                if (conditionNode == null)
                {
                    new BifySyntaxError(ErrorMessage.ConditionIsRequired()).Throw();
                    return;
                }
                AstElseIf elseIfNode = new AstElseIf(token, blockNode, conditionNode);
                if (_currentIfNode.ElseNode != null)
                {
                    ConditionStatementValidator.ThrowElseIfAfterElseError(elseIfNode);
                }
                _currentIfNode.AddElseIfNode(elseIfNode);
                HandleElseOrElseIf(Builder.Peek());

            }
            else if (token?.Type == TokenType.ELSE)
            {
                AstNode blockNode = Builder.HandleBody("Else");
                AstElse elseNode = new AstElse(token, blockNode);
                if (_currentIfNode.ElseNode != null)
                {
                    new BifySyntaxError("An 'if' statement can only have one 'else'. Remove or merge the extra 'else' block.").Throw();

                }
                _currentIfNode.ElseNode = elseNode;
                HandleElseOrElseIf(Builder.Peek());

            }
        }
    }
}
