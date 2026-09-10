using System.Collections.Generic;
using BoomifyCS.Exceptions;
using BoomifyCS.Lexer;

namespace BoomifyCS.Ast.Handlers
{
    internal class FunctionHandler(AstBuilder builder) : TokenHandler(builder)
    {
        public override void HandleToken(Token token)
        {

            GetFieldInfo(Builder, out var identifierNode, out var typeNode, out var flagNode);
        
            if (identifierNode is not AstIdentifier)
            {
                new BifySyntaxError($"Invalid identifier name '{identifierNode?.Token?.Value}'. Expected a valid name like `x`, `value`, etc.").Throw();
            }
            if (typeNode is not (AstIdentifier or AstIndexOperator) && typeNode.Token.Type != TokenType.POINTER)
            {
                new BifySyntaxError($"Invalid type '{typeNode?.Token?.Value}'. Make sure the function has a valid type like `int`, `float`, etc.").Throw();
            }
            


            List<List<Token>> splitedTokens = TokensFormatter.SplitTokensByType(Builder.GetConditionTokens(), TokenType.COMMA);
            AstParam[] parametersNodes = BuildParameters(splitedTokens);
            AstBinaryOp parameters = ConnectParameters(parametersNodes);
            AstBlock blockNode = Builder.HandleBody("Function");

            AstNode functionNode = new AstFunctionDecl(token,typeNode,(AstIdentifier)identifierNode,parameters,blockNode,flagNode);
            Builder.CurrentNode = functionNode;
            Builder.TokenIndex++;
            Builder.Nodes.Clear();


        }
        private AstBinaryOp ConnectParameters(AstParam[] parametersNodes)
        {
            AstBinaryOp binaryOp = null;
            Token commaToken = new Token(TokenType.COMMA, ",");
            for (int i = 0; i < parametersNodes.Length; i++)
            {
                if (i == 0)
                {
                    binaryOp = new AstBinaryOp(commaToken, parametersNodes[i], null);
                }
                else
                {
                    binaryOp = new AstBinaryOp(commaToken, parametersNodes[i], binaryOp);
                }
            }
            return binaryOp;
        }
        private AstParam[] BuildParameters(List<List<Token>> splitedTokens)
        {
            int paramIndex = 0;
            List<AstParam> parameters = new List<AstParam>();
            foreach (var group in splitedTokens)
            {
                AstBuilder b = new AstBuilder(group);
                AstVarDecl parametersNode = b.BuildNode() as AstVarDecl;

                if (parametersNode == null)
                {
                    new BifySyntaxError($"Parameter #{paramIndex + 1}: Invalid parameter declaration.").Throw();
                }

                AstParam param = new AstParam(parametersNode.Type, parametersNode.AssignmentNode.Left, parametersNode.Flag);
                parameters.Add(param);
                paramIndex++;
            }
            return parameters.ToArray();
        }
    }
}
