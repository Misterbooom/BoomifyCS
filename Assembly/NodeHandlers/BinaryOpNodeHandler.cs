using System;
using BoomifyCS.Assembly;
using BoomifyCS.Ast;
using BoomifyCS.Exceptions;
using BoomifyCS.Lexer;
using BoomifyCS.Objects;
using LLVMSharp.Interop;
namespace BoomifyCS.Assembly.NodeHandlers
{
    class BinaryOpNodeHandler : NodeHandler
    {
        public BinaryOpNodeHandler(AssemblyCompiler compiler) : base(compiler) { }

        public override void HandleNode(AstNode node)
        {
            var leftNode = node.Left;
            var rightNode = node.Right; 
            var operatorType = node.Token.Type; 
            if (operatorType == TokenType.COMMA)
            {
                
                compiler.Visit(leftNode);
                    
                compiler.Visit(rightNode);
                
            }
            else 
            {
                HandleBinaryOp(leftNode, rightNode, operatorType);
            }
           
        }
        private void HandleBinaryOp(AstNode leftNode, AstNode rightNode, TokenType operatorType)
        {
            compiler.Visit(leftNode);
            var leftValue = compiler.stack.Pop();
            BifyDebug.Log($"Left value: {leftValue}");

            compiler.Visit(rightNode);
            var rightValue = compiler.stack.Pop();
            BifyDebug.Log($"Right value: {rightValue}");

            BifyObject result;
            switch (operatorType)
            {
                case TokenType.ADD:
                    result = leftValue.GetBifyObject().Add(rightValue.GetBifyObject());
                    BifyDebug.Log("Operation: ADD");
                    break;
                case TokenType.SUB:
                    result = leftValue.GetBifyObject().Sub(rightValue.GetBifyObject());
                    BifyDebug.Log("Operation: SUB");
                    break;
                case TokenType.MUL:

                    result = leftValue.GetBifyObject().Mul(rightValue.GetBifyObject());
                    BifyDebug.Log("Operation: MUL");
                    break;
                case TokenType.DIV:
                    result = leftValue.GetBifyObject().Div(rightValue.GetBifyObject());
                    BifyDebug.Log("Operation: DIV");
                    break;
                default:
                    throw new InvalidOperationException($"Unsupported operator: {operatorType}");
            }

            BifyDebug.Log($"Result: {result.Repr()}");
            compiler.stack.Push(new BifyValue(result));
        }
    }



}
