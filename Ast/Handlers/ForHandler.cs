using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomifyCS.Ast.Validators;
using BoomifyCS.Exceptions;
using BoomifyCS.Lexer;
using BoomifyCS.Parser;

namespace BoomifyCS.Ast.Handlers
{
    class ForHandler(AstBuilder builder):TokenHandler(builder)
    {
        public override void HandleToken(Token token)
        {
            builder.tokenIndex++;
            var splittedTokens = TokensFormatter.SplitTokensByType(builder.GetConditionTokens(),TokenType.SEMICOLON);
            BifyDebug.Log($"SplittedTokens: {string.Join(", ", splittedTokens.Select(t => t.TokensToString()))}");

            AstNode initNode = builder.ParseTokens(splittedTokens.ElementAtOrDefault(0));
            BifyDebug.Log($"InitNode: {initNode}");

            AstNode conditionNode = builder.ParseTokens(splittedTokens.ElementAtOrDefault(1));
            BifyDebug.Log($"ConditionNode: {conditionNode}");

            AstNode incrementNode = builder.ParseTokens(splittedTokens.ElementAtOrDefault(2));
            BifyDebug.Log($"IncrementNode: {incrementNode}");
            if (builder.GetNextToken()?.Type != TokenType.LCUR)

            {
                new BifySyntaxError("For body not found!").Throw();
            }
            var blockNode = builder.ParseBlock(builder.GetBlockTokens());
            Traceback.Instance.SetCurrentLine(token.Line);
            AstFor astFor = new AstFor(token, blockNode, conditionNode, incrementNode,initNode);
            LoopValidator.ValidateForStatement(splittedTokens,astFor);
            builder.MoveToEnd();
            builder.CurrentNode = astFor;



        }
    }
}
