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
            builder.tokenIndex++;
            AstNode conditionNode = builder.ParseTokens(builder.GetConditionTokens());
            if (builder.GetNextToken()?.Type != TokenType.LCUR)
            {
                new BifySyntaxError("While body not found!").Throw();
            }
            AstNode blockNode = builder.ParseBlock(builder.GetBlockTokens());
            AstWhile astWhile = new AstWhile(token, blockNode, conditionNode);
            LoopValidator.ValidateWhileStatement(astWhile);
            
            builder.CurrentNode = astWhile;
            builder.MoveToEnd();



        }
    }
}
