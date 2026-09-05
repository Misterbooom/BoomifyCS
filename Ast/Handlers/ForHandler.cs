using System.Linq;
using BoomifyCS.Ast.Validators;
using BoomifyCS.Exceptions;
using BoomifyCS.Lexer;

namespace BoomifyCS.Ast.Handlers
{
    class ForHandler(AstBuilder builder):TokenHandler(builder)
    {
        public override void HandleToken(Token token)
        {
            Builder.TokenIndex++;
            var splittedTokens = TokensFormatter.SplitTokensByType(Builder.GetConditionTokens(),TokenType.SEMICOLON);

            AstNode initNode = Builder.ParseTokens(splittedTokens.ElementAtOrDefault(0));

            AstNode conditionNode = Builder.ParseTokens(splittedTokens.ElementAtOrDefault(1));

            AstNode incrementNode = Builder.ParseTokens(splittedTokens.ElementAtOrDefault(2));
            if (Builder.GetNextToken()?.Type != TokenType.LCUR)

            {
                new BifySyntaxError("For body not found!").Throw();
            }
            var blockNode = Builder.HandleBody("For");
            Traceback.Instance.SetCurrentLine(token.Line);
            AstFor astFor = new AstFor(token, blockNode, conditionNode, incrementNode,initNode);
            LoopValidator.ValidateForStatement(splittedTokens,astFor);
            Builder.MoveToEnd();
            Builder.CurrentNode = astFor;



        }
    }
}
