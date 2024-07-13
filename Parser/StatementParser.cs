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
        public static Tuple<AstNode, int> ParseVarDecl(Token token, List<Token> tokens, int currentPos)
        {
            var (varNameToken, varNameIndex) = TokensParser.FindTokenByTT(TokenType.IDENTIFIER, tokens, currentPos);
            var (assignmentToken, assignmentIndex) = TokensParser.FindTokenByTT(TokenType.ASSIGN, tokens, currentPos);

            if (assignmentToken == null)
            {
                List<Token> invalidTokens = new List<Token> { token };
                throw new BifySyntaxError("Assignment token not found", tokens, invalidTokens, currentPos);
            }

            currentPos = assignmentIndex + 1;
            AstAssignment assignmentNode = new AstAssignment(assignmentToken);

            var (varValueTokens, tokensProcessed) = TokensParser.AllTokensToEol(tokens, currentPos);
            currentPos += tokensProcessed;


            if (varNameToken == null)
            {
                List<Token> invalidTokens = new List<Token> { token };
                throw new BifySyntaxError("Identifier token not found", tokens, invalidTokens, currentPos - tokensProcessed);
            }

            AstNode varNameNode = AstParser.TokenToNode(varNameToken);
            AstNode varValueNode = AstParser.TokenToAst(varValueTokens);

            assignmentNode.Left = varNameNode;
            assignmentNode.Right = varValueNode;

            AstVarDecl astVarDecl = new AstVarDecl(token, assignmentNode);

            return new Tuple<AstNode, int>(astVarDecl, currentPos);
        }

    }
}
