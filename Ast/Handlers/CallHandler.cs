using System;
using System.Collections.Generic;
using System.Linq;
using BoomifyCS.Ast.Handlers;
using BoomifyCS.Exceptions;
using BoomifyCS.Lexer;
using BoomifyCS.Parser;

namespace BoomifyCS.Ast
{
    class CallHandler : TokenHandler
    {
        public CallHandler(AstBuilder builder) : base(builder) { }

        public override void HandleToken(Token token)
        {
            if (builder.operandStack.Count == 1 && builder.operatorStack.Count == 0 && builder.operandStack.Peek() is AstIdentifier)
            {
                new FunctionDeclarationHandler(builder).HandleToken(token);
            }
            else if (builder.operandStack.Count == 1 && builder.operatorStack.Count == 1 && builder.operandStack.Peek() is AstIdentifier)
            {
                int i = builder.tokenIndex;
                bool isFunc = false;
                while (i < builder.tokens.Count)
                {
                    if (builder.tokens[i].Type == TokenType.RPAREN)
                    {
                        if (i + 1 < builder.tokens.Count && builder.tokens[i + 1].Type == TokenType.LCUR)
                        {
                            isFunc = true;
                            break;
                        }
                    }
                    i++;
                }
                if (!isFunc)
                {
                    HandleCall();
                    return;
                }
                AstNode op = builder.operatorStack.Pop();

                if (op.Token.Type == TokenType.MUL)
                {
                    AstUnaryOperator pointer = new AstUnaryOperator(new Token(TokenType.POINTER, "pointer*"), builder.operandStack.Pop());
                    builder.operandStack.Push(pointer);
                    new FunctionDeclarationHandler(builder).HandleToken(token);
                }
            }

            else
            {
                HandleCall();
            }


        }
        private void HandleCall()
        {
            AstIdentifier identifier = (AstIdentifier)NodeConventer.TokenToNode(builder.tokens[builder.tokenIndex]);
            List<Token> parenthesisTokens = TokensFormatter.GetTokensBetween(builder.tokens, ref builder.tokenIndex, TokenType.LPAREN, TokenType.RPAREN);
            AstNode argumentsNode = new AstBuilder(parenthesisTokens).BuildNode();
            identifier.Token.Type = TokenType.CALL;
            AstCall astCall = new(identifier.Token, identifier, argumentsNode);
            builder.AddOperand(astCall);
        }
    }
}
