using BoomifyCS.Ast;
using BoomifyCS.Exceptions;
using BoomifyCS.Lexer;
using System.Collections.Generic;
using System;
using System.Linq;

public static class AstNodeConnector
{
    public static AstNode ConnectNodes(Stack<AstNode> operatorStack, Stack<AstNode> operandStack)
    {
        while (operatorStack.Count > 0)
        {
            var right = operandStack.Pop();
            var left = operandStack.Pop();
            var op = operatorStack.Pop();
            op.Left = left;
            op.Right = right;
            operandStack.Push(op);
        }

        if (operandStack.Count > 1)
        {
            throw new BifySyntaxError("Not enough operators");
        }

        return operandStack.First();
    }

    public static AstNode SetMaxRightNode(AstNode node, AstNode setNode)
    {
        if (node == null) throw new ArgumentNullException(nameof(node));

        var current = node;

        while (current.Right != null)
        {
            current = current.Right;
        }

        current.Right = setNode;

        return node;
    }

    public static int CountCommaNode(AstNode node)
    {
        int count = 0;

        while (node != null)
        {
            if (node is AstBinaryOp binaryOp && binaryOp.Token.Type == TokenType.COMMA)
            {
                count++;
            }

            node = node.Right;
        }

        return count;
    }
}
