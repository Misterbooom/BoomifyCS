using System;
using BoomifyCS.Assembly;
using BoomifyCS.Ast;
using BoomifyCS.Exceptions;
using BoomifyCS.Lexer;
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
                compiler.Visit(leftNode);
                var leftValue = compiler.stack.Pop();

                compiler.Visit(rightNode);
                var rightValue = compiler.stack.Pop();
                LLVMValueRef result;

                switch (operatorType)
                {
                    case TokenType.ADD:
                        result = compiler.builder.BuildAdd(leftValue.GetValueRef(), rightValue.GetValueRef(), "addtmp");
                        break;
                    case TokenType.SUB:
                        result = compiler.builder.BuildFSub(leftValue.GetValueRef(), rightValue.GetValueRef(), "subtmp");
                        break;
                    case TokenType.MUL:

                        result = compiler.builder.BuildMul(leftValue.GetValueRef(), rightValue.GetValueRef(), "multmp");
                        break;
                    case TokenType.DIV:
                        result = compiler.builder.BuildSDiv(leftValue.GetValueRef(), rightValue.GetValueRef(), "divtmp");
                        break;



                    default:
                        throw new InvalidOperationException($"Unsupported operator: {operatorType}");
                }

               compiler.stack.Push(new BifyValue(leftValue.GetBifyObject(),result));
            }
           
        }
    }



}
