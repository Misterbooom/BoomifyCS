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
    class WhileHandler(AstBuilder builder) : TokenHandler(builder)
    {
        public override void HandleToken(Token token)
        {
            AstNode conditionNode = builder.ParseTokens(builder.GetConditionTokens());
            AstNode blockNode = builder.HandleBody("While");
            AstWhile astWhile = new AstWhile(token, blockNode, conditionNode);
            LoopValidator.ValidateWhileStatement(astWhile);

            builder.CurrentNode = astWhile;
            builder.MoveToEnd();
        }

        
    }
}
