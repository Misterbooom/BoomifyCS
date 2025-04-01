using System;
using BoomifyCS.Assembly;
using BoomifyCS.Assembly.BifyObject;
using BoomifyCS.Assembly.Builtin;
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
                BifyValue result = value.Not(compiler.Builder);
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
                    return lhs.Add(rhs, compiler.Builder);
                case TokenType.SUB:
                    return lhs.Sub(rhs, compiler.Builder);
                case TokenType.MUL:
                    return lhs.Mul(rhs, compiler.Builder);
                case TokenType.DIV:
                    if (rhs.CompareType(typeof(IntegerType)) || rhs.CompareType(typeof(FloatType)))
                    {
                        return SafeDiv.Instance(lhs.GetBifyType())
                            .Call([lhs, rhs,new IntegerType().Create(Traceback.Instance.Line)]);
                    }
                    
                    var res = lhs.Div(rhs, compiler.Builder);
                    //if (res.GetLLVMValue().IsPoison)
                    //{
                    //    Traceback.Instance.ThrowException(new BifyZeroDivisionError("Division by zero!"));
                    //    return res;
                    //}
                    
                    
                    return res;

                case TokenType.EQ:
                    return lhs.Equal(rhs, compiler.Builder);
                case TokenType.NEQ:
                    return lhs.NotEqual(rhs, compiler.Builder);
                case TokenType.LT:
                    return lhs.LessThan(rhs, compiler.Builder);
                case TokenType.GT:
                    return lhs.GreaterThan(rhs, compiler.Builder);
                case TokenType.LTEQ:
                    return lhs.LessThanOrEqual(rhs, compiler.Builder);
                case TokenType.GTEQ:
                    return lhs.GreaterThanOrEqual(rhs, compiler.Builder);
                case TokenType.OR:
                    if (!lhs.CompareType(typeof(BoolType)) || !rhs.CompareType(typeof(BoolType)))
                    {
                        Traceback.Instance.ThrowException(new BifyTypeError($"Cannot compare {lhs.GetTypeName()} with {rhs.GetTypeName()}"));
                    }

                    return new BoolValue(compiler.Builder.BuildOr(lhs.GetLLVMValue(), rhs.GetLLVMValue(), "or"));
                case TokenType.AND:
                    if (!lhs.CompareType(typeof(BoolType)) || !rhs.CompareType(typeof(BoolType)))
                    {
                        Traceback.Instance.ThrowException(new BifyTypeError($"Cannot compare {lhs.GetTypeName()} with {rhs.GetTypeName()}"));
                    }
                    return new BoolValue(compiler.Builder.BuildAnd(lhs.GetLLVMValue(), rhs.GetLLVMValue(), "and"));
                default:
                    throw new NotImplementedException($"Not implemented binary operator.Type: {type}");
            }
        }

    }



}
