using System;
using System.Collections.Generic;
using BoomifyCS.Ast.Handlers;
using BoomifyCS.Lexer;
using BoomifyCS.Ast.Validators;
using BoomifyCS.Exceptions;
namespace BoomifyCS.Ast 
{
    class IfHandler : TokenHandler
    {
        public IfHandler(AstBuilder builder) : base(builder) { }

        public override void HandleToken(Token token)
        {
            List<Token> conditionTokens = builder.GetConditionTokens();
            List<Token> blockTokens = builder.GetBlockTokens();
            AstNode conditionNode = builder.ParseCondition(conditionTokens);
            AstNode blockNode = builder.ParseBlock(blockTokens);
            ConditionStatementValidator.ValidateIfStatement(blockTokens, conditionTokens);
            AstIf ifNode = new(conditionTokens[0], conditionNode, blockNode);
            builder.AddOperand(ifNode);
        }
    }
    class ElseStatementHandler : TokenHandler
    {
        public ElseStatementHandler(AstBuilder builder) : base(builder) { }

        public override void HandleToken(Token token)
        {
            Token elseToken = builder.tokens[builder.tokenIndex];
            Token nextToken = TokensFormatter.GetTokenOrNull(builder.tokens,builder.tokenIndex + 1);
            if (nextToken != null && nextToken.Type == TokenType.IF)
            {
                new ElseIfHandler(builder).HandleToken(token);
                return;
            }
            int line = Traceback.Instance.line;

            List<Token> blockTokens = builder.GetBlockTokens();
            AstNode blockNode = builder.ParseBlock(blockTokens);

            AstElse astElse = new(elseToken, blockNode);
            Traceback.Instance.SetCurrentLine(line);
            ConditionStatementValidator.ValidateElseStatement(builder.operandStack, astElse);

            AstIf astIf = (AstIf)builder.operandStack.Peek();
            astIf.ElseNode = astElse;
        }
    }
    class ElseIfHandler : TokenHandler
    {
        public ElseIfHandler(AstBuilder builder) : base(builder) { }

        public override void HandleToken(Token token)
        {
            Token elseToken = builder.tokens[builder.tokenIndex];
            int line = Traceback.Instance.line;

            List<Token> conditionTokens = builder.GetConditionTokens();
            List<Token> blockTokens = builder.GetBlockTokens();
            AstNode conditionNode = builder.ParseCondition(conditionTokens);
            AstNode blockNode = builder.ParseBlock(blockTokens);

            AstElseIf astElseIf = new(elseToken, blockNode, conditionNode);
            Traceback.Instance.SetCurrentLine(line);
            ConditionStatementValidator.ValidateElseIfStatement(conditionTokens, builder.operandStack, astElseIf);

            AstIf astIf = (AstIf)builder.operandStack.Peek();
            astIf.AddElseIfNode(astElseIf);
        }
    }
}
