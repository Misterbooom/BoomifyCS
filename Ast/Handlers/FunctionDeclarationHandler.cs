using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomifyCS.Ast.Validators;
using BoomifyCS.Exceptions;
using BoomifyCS.Lexer;
using BoomifyCS.Parser;

namespace BoomifyCS.Ast.Handlers
{
    class FunctionDeclarationHandler : TokenHandler
    {
        public FunctionDeclarationHandler(AstBuilder builder) : base(builder) { }
        public override void HandleToken(Token token)
        {
            Token nameToken = TokensFormatter.GetTokenOrNull(builder.tokens, builder.tokenIndex + 1);
            builder.tokenIndex++;
            List<Token> parametersTokens = builder.GetConditionTokens();
            List<Token> blockTokens = builder.GetBlockTokens();
            AstNode argumentsNode = builder.ParseCondition(parametersTokens);
            AstNode blockNode = builder.ParseBlock(blockTokens);
            Traceback.Instance.SetCurrentLine(token.Line);
            FunctionDeclarationValidator.Validate(token,nameToken, argumentsNode, blockNode);
            AstFunctionDecl functionNode = new AstFunctionDecl(token, (AstIdentifier)NodeConventer.TokenToNode(nameToken),argumentsNode, blockNode);
            builder.AddOperand(functionNode);
        }
    }
}
