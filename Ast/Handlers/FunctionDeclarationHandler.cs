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
            AstNode typeNode = builder.operandStack.Pop();
            List<Token> parametersTokens = builder.GetConditionTokens();
            List<Token> blockTokens = builder.GetBlockTokens();
            AstNode parametersNode = builder.ParseCondition(parametersTokens);
            AstNode blockNode = builder.ParseBlock(blockTokens);
            
            Traceback.Instance.SetCurrentLine(token.Line);
            FunctionDeclarationValidator.Validate(token, parametersNode, blockNode, typeNode);
            AstFunctionDecl functionNode = new AstFunctionDecl(token, typeNode,new AstIdentifier(token,token.Value), parametersNode, blockNode);
            builder.AddOperand(functionNode);
            builder.tokenIndex++; // skip the closing curly brace
        }
    }
}
