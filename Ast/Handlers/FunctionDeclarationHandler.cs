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

            AstParam[] paramNodes = GetParameters(parameterGroups);
            AstNode connectedParams = null;

            if (paramNodes.Length > 0)
            {
                connectedParams = paramNodes[0];
                for (int i = 1; i < paramNodes.Length; i++)
                {
                    connectedParams = new AstBinaryOp(new Token(TokenType.COMMA, ","), connectedParams, paramNodes[i]);
                }
            }

            AstNode blockNode = builder.ParseBlock(blockTokens);
            AstNode nameNode = NodeConventer.TokenToNode(token);
            FunctionDeclarationValidator.Validate(token, connectedParams, blockNode, typeNode);
            AstFunctionDecl astFunctionDecl = new(token, typeNode, (AstIdentifier)nameNode, 
                connectedParams, (AstBlock)blockNode);
            builder.operatorStack.Clear();
            builder.operandStack.Clear();
            builder.AddOperand(astFunctionDecl);


        }
        private AstParam[] GetParameters(List<List<Token>> parameterGroups)
        {
            List<AstParam> astParams = new List<AstParam>();
            foreach (var group in parameterGroups)
            {
                BifyDebug.Log($"Group: {group.TokensToString()}");

                AstNode[] paramNodes = new AstBuilder(group).BuildWithoutConnecting();
                //foreach (var node in paramNodes)
                //{
                //    BifyDebug.Log($"ParamNode: {node}");
                //}


                AstParam astParam = new AstParam(null, null, null);
                if (paramNodes.Length > 3 || paramNodes.Length < 2)
                {
                    Traceback.Instance.ThrowException(new BifySyntaxError("Invalid Parameter Declaration!"));
                }
                AstNode paramName = paramNodes[0];
                
                AstNode paramType = paramNodes[1];
                BifyDebug.Log($"Pointer paramName: {paramName}\nParamType: {paramType}");
                if (paramName.Token.Type == TokenType.POINTER)
                {
                    (AstNode lastOperand, AstNode typeNode) = VariableDeclarationHandler.
                        SwitchLastPointerOperand(paramName, paramType);
                    paramName = lastOperand;
                    paramType = typeNode;

                }
                if (paramName is not AstIdentifier)
                {
                    Traceback.Instance.ThrowException(new BifySyntaxError("Invalid Parameter Name!"));
                }
                if (paramType is not AstIdentifier && paramType.Token.Type != TokenType.POINTER)
                {
                    Traceback.Instance.ThrowException(new BifySyntaxError("Invalid Parameter Type!"));
                }
                astParam.Name = paramName;
                astParam.Type = paramType;
                if (paramNodes.Length == 3)
                {
                    AstNode paramFlag = paramNodes[2];
                    if (paramFlag is AstIdentifier)
                    {
                        astParam.Flag = paramFlag;
                    }
                    else
                    {
                        Traceback.Instance.ThrowException(new BifySyntaxError("Invalid Parameter Flag!"));
                    }
                }
              
                astParams.Add(astParam);
            }
            return astParams.ToArray();
        }
    }
}
