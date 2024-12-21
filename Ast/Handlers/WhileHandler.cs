using System;
using System.Collections.Generic;
using BoomifyCS.Ast.Handlers;
using BoomifyCS.Lexer;
using BoomifyCS.Ast.Validators;

namespace BoomifyCS.Ast
{
    class WhileHandler : TokenHandler
    {
        public WhileHandler(AstBuilder builder) : base(builder) { }

        public override void HandleToken(Token token)
        {
            Token whileToken = builder.tokens[builder.tokenIndex];
            int line = Traceback.Traceback.Instance.line;

            List<Token> conditionTokens = builder.GetConditionTokens();
            List<Token> blockTokens = builder.GetBlockTokens();
            AstNode conditionNode = builder.ParseCondition(conditionTokens);
            AstNode blockNode = builder.ParseBlock(blockTokens);
            AstWhile astWhile = new(whileToken, blockNode, conditionNode);
            Traceback.Traceback.Instance.SetCurrentLine(line);
            IteratorStatementValidator.ValidateWhileStatement(conditionTokens, astWhile);
            builder.AddOperand(astWhile);
        }
    }
}
