using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomifyCS.Ast;
using BoomifyCS.Lexer;
using BoomifyCS.Parser.NodeParser;
namespace BoomifyCS.Parser.StatementParser
{
    public class ConditionalStatementParser
    {
        public static (AstNode, int) ParseIf(Token token, List<Token> tokens, int currentPos, AstTree astParser)
        {
            var conditionNode = AstBuilder.ParseCondition(tokens, ref currentPos, astParser);
            var blockNode = AstBuilder.ParseBlock(tokens, ref currentPos, astParser);

            return (new AstIf(token, conditionNode, (AstBlock)blockNode), currentPos);
        }

        public static (AstNode, int) ParseElse(Token token, List<Token> tokens, int currentPos, AstTree astParser)
        {
            AstNode elseNode;

            if (TokenFinder.IsNextToken(tokens, currentPos,TokenType.IF))
            {

                elseNode = ParseElseIf(token, tokens,ref currentPos, astParser).Item1;
            }
            else
            {
                currentPos--;
                elseNode = ParseElseBlock(token, tokens, ref currentPos, astParser);
            }
            return (elseNode, currentPos);
        }
        private static AstElse ParseElseBlock(Token token, List<Token> tokens, ref int currentPos, AstTree astParser)
        {
            var blockNode = AstBuilder.ParseBlock(tokens, ref currentPos, astParser);
            return new AstElse(token, (AstBlock)blockNode);
        }

        private static (AstNode, int) ParseElseIf(Token token, List<Token> tokens, ref int currentPos, AstTree astParser)
        {
            var conditionNode = AstBuilder.ParseCondition(tokens, ref currentPos, astParser);
            
            var blockNode = AstBuilder.ParseBlock(tokens, ref currentPos, astParser);

            return (new AstElseIf(token, (AstBlock)blockNode, conditionNode), currentPos);
        }
    }
}
