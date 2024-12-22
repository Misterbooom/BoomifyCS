using System;
using System.Collections.Generic;
using BoomifyCS.Ast.Handlers;
using BoomifyCS.Lexer;
using BoomifyCS.Ast.Validators;
using BoomifyCS.Parser;
using BoomifyCS.Exceptions;

namespace BoomifyCS.Ast
{
    class IteratorHandlers : TokenHandler
    {
        public IteratorHandlers(AstBuilder builder) : base(builder) { }

        public override void HandleToken(Token token)
        {

            List<Token> conditionTokens = builder.GetConditionTokens();
            List<Token> blockTokens = builder.GetBlockTokens();
            AstNode conditionNode = builder.ParseCondition(conditionTokens);
            AstNode blockNode = builder.ParseBlock(blockTokens);
            AstWhile astWhile = new(token, blockNode, conditionNode);
            IteratorStatementValidator.ValidateWhileStatement(conditionTokens, astWhile);
            builder.AddOperand(astWhile);
        }
    }
    class ForHandler : TokenHandler { 
        public ForHandler(AstBuilder builder) : base(builder) { }
        public override void HandleToken(Token token) { 
            List<Token> conditionTokens = builder.GetConditionTokens();
            List<Token> blockTokens = builder.GetBlockTokens();
            var splitedTokens = TokensFormatter.SplitTokensByType(conditionTokens, TokenType.SEMICOLON); 
            if (splitedTokens.Count != 3)
            {
                BifySyntaxError error = new(ErrorMessage.InvalidForLoopStructure(), "", token.Value);
            }
            List<Token> initTokens = splitedTokens[0];
            List<Token> condition = splitedTokens[1];
            List<Token> increment = splitedTokens[2];
            AstFor astFor = new(token, builder.ParseBlock(blockTokens), builder.ParseCondition(condition), builder.ParseCondition(increment), builder.ParseCondition(initTokens));
            IteratorStatementValidator.ValidateForStatement(splitedTokens, astFor);
            builder.AddOperand(astFor);

        }
    }
}
