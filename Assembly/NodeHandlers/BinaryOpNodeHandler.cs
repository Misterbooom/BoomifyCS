using System;
using BoomifyCS.Assembly;
using BoomifyCS.Assembly.BifyObject;
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
                    return lhs.Div(rhs, compiler.builder);
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
