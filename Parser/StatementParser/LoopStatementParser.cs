using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomifyCS.Ast;
using BoomifyCS.Exceptions;
using BoomifyCS.Lexer;
using BoomifyCS.Parser.NodeParser;

namespace BoomifyCS.Parser.StatementParser
{
    public class LoopStatementParser
    {


        public static (AstNode, int) ParseWhile(Token token, List<Token> tokens, int currentPos, AstTree astParser)
        {
            var conditionNode = AstBuilder.ParseCondition(tokens, ref currentPos, astParser);
            var blockNode = AstBuilder.ParseBlock(tokens, ref currentPos, astParser);

            return (new AstWhile(token, (AstBlock)blockNode, conditionNode), currentPos);
        }

        public static (AstNode, int) ParseFor(Token token, List<Token> tokens, int currentPos, AstTree astParser)
        {
            var (initNode, conditionNode, incrementNode) = ParseForLoopHeader(tokens, ref currentPos, astParser);
            var blockNode = AstBuilder.ParseBlock(tokens, ref currentPos, astParser);

            ForLoopValidator.ValidateForLoop(initNode, conditionNode, incrementNode);

            return (new AstFor(token, (AstBlock)blockNode, conditionNode, incrementNode, initNode), currentPos);
        }
        private static (AstNode, AstNode, AstNode) ParseForLoopHeader(List<Token> tokens, ref int currentPos, AstTree astParser)
        {
            var (tokensInBrackets, bracketEnd) = TokenFinder.FindTokensInBracketsSafe(tokens, currentPos,ErrorMessage.InvalidInitializationStatement());
            currentPos = bracketEnd + 1;
            List<List<Token>> bracketTokensSplit = TokensParser.SplitTokensByTT(tokensInBrackets, TokenType.SEMICOLON);

            ForLoopValidator.ValidateForLoopHeader(bracketTokensSplit);

            AstNode initNode = astParser.BuildAstTree(bracketTokensSplit[0]);
            AstNode conditionNode = astParser.BuildAstTree(bracketTokensSplit[1]);
            AstNode incrementNode = astParser.BuildAstTree(bracketTokensSplit[2]);

            return (initNode, conditionNode, incrementNode);
        }
    }
}
