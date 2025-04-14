using System;
using BoomifyCS.Assembly;
using BoomifyCS.Assembly.BifyObject;
using BoomifyCS.Assembly.Builtin;
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
                IValue ivalue = compiler.StackIValuePop();
                if (ivalue is BifyType)
                {
                    Traceback.Instance.ThrowException(new BifyTypeError("Invalid operand: type provided instead of value"));
                    return;
                }
                BifyValue value = (BifyValue)ivalue;
                BifyValue result = value.Not(compiler.Builder);
                compiler.StackPush(result);
                return;
            }
            else
            {
                compiler.Visit(node.Left);
                compiler.Visit(node.Right);
                IValue rhsI = compiler.StackIValuePop();
                if (rhsI is BifyType)
                {
                    Traceback.Instance.ThrowException(new BifyTypeError("Invalid operand: type provided instead of value"));
                    return;
                }
                BifyValue rhs = (BifyValue)rhsI;
                IValue lhsI = compiler.StackIValuePop();
                if (lhsI is BifyType)
                {
                    Traceback.Instance.ThrowException(new BifyTypeError("Invalid operand: type provided instead of value"));
                    return;
                }
                BifyValue lhs = (BifyValue)lhsI;
                BifyDebug.Log($"{rhs},{lhs}");
                BifyValue result = BinaryVal(lhs, rhs, node.Token.Type);
                compiler.StackPush(result);
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
                            .Call(new BifyValue[] { lhs, rhs, new IntegerType().Create(Traceback.Instance.Line) });
                    }
                    return lhs.Div(rhs, compiler.Builder);
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
                    throw new NotImplementedException($"Not implemented binary operator. Type: {type}");
            }
        }
    }
}
