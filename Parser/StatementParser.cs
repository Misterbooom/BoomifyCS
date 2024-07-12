using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomifyCS.Ast;
using BoomifyCS.Lexer;

namespace BoomifyCS.Parser
{
    public static class StatementParser
    {
        public static Tuple<AstNode,int> ParseVarDecl(Token token, List<Token> tokens, int currentPos)
        {
            int i;
            Token assignmentToken;
            Tuple<Token, int> result = TokensParser.FindTokenByTT(TokenType.ASSIGN, tokens, currentPos);
            assignmentToken = result.Item1;
            if (assignmentToken == null)
            {
                List<Token> invalidTokens = new List<Token> { token };
                throw new BifySyntaxError("Assignment token not found", tokens, invalidTokens, 0);
            }
            i = result.Item2;
            currentPos += i + 1;
            AstAssignment assignmentNode = new AstAssignment(assignmentToken);
            Tuple<List<Token>,int> resultList = TokensParser.AllTokensToEol(tokens, currentPos);
            
            List<Token> varValueTokens = resultList.Item1;

            i = resultList.Item2;
            AstNode varValueNode = AstParser.TokenToAst(varValueTokens);
            assignmentNode.Right = varValueNode;
            AstVarDecl astVarDecl = new AstVarDecl(token, assignmentNode);
            return new Tuple<AstNode, int>(astVarDecl,i);
        }
    }
}
