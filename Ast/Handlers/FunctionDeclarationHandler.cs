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
            List<List<Token>> parameterGroups = new List<List<Token>>();
            List<Token> currentGroup = new List<Token>();

            foreach (var t in parametersTokens)
            {
                if (t.Type == TokenType.COMMA)
                {
                    if (currentGroup.Count > 0)
                    {
                        parameterGroups.Add(currentGroup);
                        currentGroup = new List<Token>();
                    }
                }
                else
                {
                    currentGroup.Add(t);
                }
            }

            if (currentGroup.Count > 0)
            {
                parameterGroups.Add(currentGroup);
            }

            AstNode parametersNode = null;
            List<AstBinaryOp> concatNodes = new List<AstBinaryOp>();

            foreach (var group in parameterGroups)
            {
                if (group.Count < 2)
                    continue;

                List<Token> typeTokens = group.Take(group.Count - 1).ToList();
                Token nameToken = group.Last();

                AstNode typeAstNode = builder.ParseTokens(typeTokens);
                AstIdentifier nameIdentifier = new AstIdentifier(nameToken, nameToken.Value);

                Token concatToken = new Token(TokenType.ADD, "concat");
                AstBinaryOp concatOp = new AstBinaryOp(concatToken, typeAstNode, nameIdentifier);
                concatNodes.Add(concatOp);
            }

            if (concatNodes.Count > 0)
            {
                parametersNode = concatNodes[0];
                for (int i = 1; i < concatNodes.Count; i++)
                {
                    Token commaToken = new Token(TokenType.COMMA, ",");
                    parametersNode = new AstBinaryOp(commaToken, parametersNode, concatNodes[i]);
                }
            }

            AstNode blockNode = builder.ParseBlock(blockTokens);

            Traceback.Instance.SetCurrentLine(token.Line);

            FunctionDeclarationValidator.Validate(token, parametersNode, blockNode, typeNode);
            AstFunctionDecl functionNode = new AstFunctionDecl(
                token,
                typeNode,
                new AstIdentifier(token, token.Value),
                parametersNode,
                (AstBlock)blockNode
            );
            builder.operatorStack.Clear();
            builder.operandStack.Clear();
            builder.AddOperand(functionNode);
            builder.tokenIndex++;
        }
    }
}
