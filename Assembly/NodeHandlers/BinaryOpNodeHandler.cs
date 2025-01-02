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
            // Assuming node has fields: Left, Right, and Operator
            var leftNode = node.Left; // Left operand
            var rightNode = node.Right; // Right operand
            var operatorType = node.Token.Type; // Operator (+, -, *, /)

            compiler.Visit(leftNode);
            var leftValue = leftNode.LlvmValue;

            compiler.Visit(rightNode);
            var rightValue = rightNode.LlvmValue;
            LLVMValueRef result;
            switch (operatorType)
            {
                case TokenType.ADD:
                    BifyDebug.Log($"Left - {leftNode.LlvmValue} Right - {rightNode.LlvmValue}");
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

    class ConstantNodeHandler : NodeHandler
    {
        public ConstantNodeHandler(AssemblyCompiler compiler) : base(compiler) { }

        public override void HandleNode(AstNode node)
        {
            if (node is AstConstant astConstant)
            {
                node.LlvmValue = astConstant.BifyValue.ToLLVM();
            }

        }
    }

}
