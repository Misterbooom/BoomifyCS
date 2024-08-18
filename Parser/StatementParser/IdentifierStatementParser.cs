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
    public class IdentifierStatementParser
    {
        public static (AstNode, int) ParseIdentifier(Token token, List<Token> tokens, int currentPos, AstTree astParser)
        {
      

            var argumentsTokens = TokenFinder.ParseOptionalArguments(tokens, ref currentPos,"");
            
            if (argumentsTokens == null)
            {
                return HandleIdentifierWithoutArguments(token, tokens, ref currentPos);
            }
            return ParseCall(token, argumentsTokens, ref currentPos, astParser);
        }


        private static (AstNode, int) HandleIdentifierWithoutArguments(Token token, List<Token> tokens, ref int currentPos)
        {

            if (tokens.Count < currentPos + 2)
            {
                return (NodeConverter.TokenToNode(token), currentPos);
            }

            var nextToken = tokens[currentPos + 1];
            if (nextToken.Type == TokenType.INCREMENT || nextToken.Type == TokenType.DECREMENT)
            {
                AstUnaryOperator unaryOperator = new(nextToken, NodeConverter.TokenToNode(token), 1);
                return (unaryOperator, currentPos + 1);
            }

            return (NodeConverter.TokenToNode(token), currentPos);
        }

        private static (AstCall, int) ParseCall(Token token, List<Token> argumentsTokens, ref int currentPos, AstTree astParser)
        {
            

            AstNode identifierNode = NodeConverter.TokenToNode(token);
            AstNode argumentsNode = astParser.BuildAstTree(argumentsTokens);
            Token callToken = new(TokenType.CALL, token.Value + "(" + argumentsTokens.TokensToString() + ")");
            AstCall callNode = new(callToken, identifierNode, argumentsNode);

            return (callNode, currentPos - 1);
        }

    }
}
