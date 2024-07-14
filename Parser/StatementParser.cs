using System;
using System.Collections.Generic;
using BoomifyCS.Ast;
using BoomifyCS.Lexer;

namespace BoomifyCS.Parser
{
    public static class StatementParser
    {
        public static Tuple<AstNode, int> ParseVarDecl(Token token, List<Token> tokens, int currentPos)
        {
            try
            {
                var (varNameToken, varNameIndex) = TokensParser.FindTokenByTT(TokenType.IDENTIFIER, tokens, currentPos);
                var (assignmentToken, assignmentIndex) = TokensParser.FindTokenByTT(TokenType.ASSIGN, tokens, currentPos);

                if (assignmentToken == null)
                {
                    List<Token> invalidTokens = new List<Token> { token };
                    throw new BifySyntaxError("Assignment token not found", tokens, invalidTokens,0);
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

                AstNode varNameNode = NodeParser.TokenToNode(varNameToken);
                AstNode varValueNode = NodeParser.TokenToAst(varValueTokens);

                assignmentNode.Left = varNameNode;
                assignmentNode.Right = varValueNode;

                AstVarDecl astVarDecl = new AstVarDecl(token, assignmentNode);
                return Tuple.Create<AstNode, int>(astVarDecl, currentPos);
            }
            catch (NullReferenceException e)
            {
                List<Token> invalidTokens = new List<Token> { token };
                throw new BifySyntaxError("Identifier token not found", tokens, invalidTokens, 0);
            }
        }

        public static Tuple<AstNode, int> ParseIf(Token token, List<Token> tokens, int currentPos)
        {
            var (conditionTokens, conditionEnd) = TokensParser.TokensInBrackets(tokens, currentPos);
            currentPos = conditionEnd + 1;
            AstNode conditionNode = NodeParser.TokenToAst(conditionTokens);
            var (blockToken, blockEnd) = TokensParser.FindTokenByTT(TokenType.OBJECT, tokens, currentPos);
            currentPos = blockEnd;
            List<Token> blockTokens = new List<Token> { blockToken };
            AstNode blockNode = NodeParser.TokenToAst(blockTokens);
            AstIf astIf = new AstIf(token, conditionNode, (AstBlock)blockNode);
            return Tuple.Create<AstNode, int>(astIf, currentPos);
        }

        public static Tuple<AstNode, int> ParseElse(Token token, List<Token> tokens, int currentPos)
        {
            var (blockToken, blockEnd) = TokensParser.FindTokenByTT(TokenType.OBJECT, tokens, currentPos);
            currentPos = blockEnd;
            List<Token> blockTokens = new List<Token> { blockToken };
            AstNode blockNode = NodeParser.TokenToAst(blockTokens);
            AstElse astElse = new AstElse(token, (AstBlock)blockNode);
            return Tuple.Create<AstNode, int>(astElse, currentPos);
        }
    }
}
