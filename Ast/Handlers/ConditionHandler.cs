using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomifyCS.Ast.Validators;
using BoomifyCS.Exceptions;
using BoomifyCS.Lexer;

namespace BoomifyCS.Ast.Handlers
{
    class ConditionHandler(AstBuilder builder) : TokenHandler(builder)
    {
        AstIf currentIfNode = null;
        public override void HandleToken(Token token)
        {
            HandleIf(token);
            builder.CurrentNode = currentIfNode;
        }
        private void HandleIf(Token token)
        {
            AstNode conditionNode = builder.ParseTokens(builder.GetConditionTokens());
            AstNode blockNode = builder.HandleBody("If");
            if (conditionNode == null)
            {
                new BifySyntaxError(ErrorMessage.ConditionIsRequired()).Throw();
                return;
            }
            currentIfNode = new AstIf(token, conditionNode, blockNode);

            HandleElseOrElseIf(builder.Peek());
        }
        private void HandleElseOrElseIf(Token token)
        {

            
            if (token?.Type == TokenType.ELSE && builder.GetNextToken()?.Type == TokenType.IF)
            {
                builder.tokenIndex++; // skip if token
                AstNode conditionNode = builder.ParseTokens(builder.GetConditionTokens());
                AstNode blockNode = builder.HandleBody("Else-If");
                if (conditionNode == null)
                {
                    new BifySyntaxError(ErrorMessage.ConditionIsRequired()).Throw();
                    return;
                }
                AstElseIf elseIfNode = new AstElseIf(token, blockNode, conditionNode);
                if (currentIfNode.ElseNode != null)
                {
                    ConditionStatementValidator.ThrowElseIfAfterElseError(elseIfNode);
                }
                currentIfNode.AddElseIfNode(elseIfNode);
                HandleElseOrElseIf(builder.Peek());

            }
            else if (token?.Type == TokenType.ELSE)
            {
                AstNode blockNode = builder.HandleBody("Else");
                AstElse elseNode = new AstElse(token, blockNode);
                if (currentIfNode.ElseNode != null)
                {
                    new BifySyntaxError("An 'if' statement can only have one 'else'. Remove or merge the extra 'else' block.").Throw();

                }
                currentIfNode.ElseNode = elseNode;
                HandleElseOrElseIf(builder.Peek());

            }
        }
    }
}
