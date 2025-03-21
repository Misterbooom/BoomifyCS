using System;
using BoomifyCS.Assembly;
using BoomifyCS.Assembly.BifyObject;
using BoomifyCS.Ast;
using BoomifyCS.Exceptions;
using BoomifyCS.Lexer;
using LLVMSharp.Interop;
using NUnit.Framework.Constraints;
namespace BoomifyCS.Assembly.NodeHandlers
{
    class BinaryOpNodeHandler : NodeHandler
    {
        public BinaryOpNodeHandler(AssemblyCompiler compiler) : base(compiler) { }

        public override void HandleNode(AstNode node)
        {
            if (node is AstBinaryOp && node.Token.Type == TokenType.COMMA)
            {
                compiler.Visit(node.Left);
                compiler.Visit(node.Right);
                return;
            }

            else if (node.Token.Type == TokenType.NOT)
            {
                compiler.Visit(node.Left);
                BifyValue value = compiler.StackPop();
                BifyValue result = value.Not(compiler.builder);
                compiler.StackPush(result);
                return;
            }
            else
            {
                compiler.Visit(node.Left);
                compiler.Visit(node.Right);
                BifyValue rhs = compiler.StackPop();
                BifyValue lhs = compiler.StackPop();
                BifyDebug.Log($"{rhs},{lhs}");
                BifyValue value = BinaryVal(lhs, rhs, node.Token.Type);
                compiler.StackPush(value);
            }

        }
        private BifyValue BinaryVal(BifyValue lhs, BifyValue rhs, TokenType type)
        {
            switch (type)
            {
                case TokenType.ADD:
                    return lhs.Add(rhs, compiler.builder);
                case TokenType.SUB:
                    return lhs.Sub(rhs, compiler.builder);
                case TokenType.MUL:
                    return lhs.Mul(rhs, compiler.builder);
                case TokenType.DIV:
                    var res = lhs.Div(rhs, compiler.builder);
                    if (res.GetLLVMValue().IsPoison)
                    {
                        Traceback.Instance.ThrowException(new BifyZeroDivisionError("Division by zero!"));
                        return res;
                    }
                    return res;

                case TokenType.EQ:
                    return lhs.Equal(rhs, compiler.builder);
                case TokenType.NEQ:
                    return lhs.NotEqual(rhs, compiler.builder);
                case TokenType.LT:
                    return lhs.LessThan(rhs, compiler.builder);
                case TokenType.GT:
                    return lhs.GreaterThan(rhs, compiler.builder);
                case TokenType.LTEQ:
                    return lhs.LessThanOrEqual(rhs, compiler.builder);
                case TokenType.GTEQ:
                    return lhs.GreaterThanOrEqual(rhs, compiler.builder);
                case TokenType.OR:
                    if (!lhs.CompareType(typeof(BoolType)) || !rhs.CompareType(typeof(BoolType)))
                    {
                        Traceback.Instance.ThrowException(new BifyTypeError($"Cannot compare {lhs.GetTypeName()} with {rhs.GetTypeName()}"));
                    }

                    return new BoolValue(compiler.builder.BuildOr(lhs.GetLLVMValue(), rhs.GetLLVMValue(), "or"));
                case TokenType.AND:
                    if (!lhs.CompareType(typeof(BoolType)) || !rhs.CompareType(typeof(BoolType)))
                    {
                        Traceback.Instance.ThrowException(new BifyTypeError($"Cannot compare {lhs.GetTypeName()} with {rhs.GetTypeName()}"));
                    }
                    return new BoolValue(compiler.builder.BuildAnd(lhs.GetLLVMValue(), rhs.GetLLVMValue(), "and"));
                default:
                    throw new NotImplementedException($"Not implemented binary operator.Type: {type}");
            }
        }

    }



}
