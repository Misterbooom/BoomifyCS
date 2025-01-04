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

            compiler.Visit(leftNode);
            var leftValue = leftNode.LlvmValue;

            compiler.Visit(rightNode);
            var rightValue = rightNode.LlvmValue;
            LLVMValueRef result;
            switch (operatorType)
            {
                case TokenType.ADD:
                    result = compiler.builder.BuildAdd(leftValue, rightValue, "addtmp");
                    break;
                case TokenType.SUB:
                    result = compiler.builder.BuildFSub(leftValue, rightValue, "subtmp");
                    break;
                case TokenType.MUL:

                    result = compiler.builder.BuildMul(leftValue, rightValue, "multmp");
                    break;
                case TokenType.DIV:
                    result = compiler.builder.BuildSDiv(leftValue, rightValue, "divtmp");
                    break;
                default:
                    throw new InvalidOperationException($"Unsupported operator: {operatorType}");
            }

            node.LlvmValue = result;
        }
    }



}
