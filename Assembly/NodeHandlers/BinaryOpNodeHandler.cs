using System;
using BoomifyCS.Assembly.BifyObject;
using BoomifyCS.Assembly.Builtin;
using BoomifyCS.Ast;
using BoomifyCS.Exceptions;
using BoomifyCS.Lexer;

namespace BoomifyCS.Assembly.NodeHandlers
{
    internal class BinaryOpNodeHandler(AssemblyCompiler compiler) : NodeHandler(compiler)
    {
        public override void HandleNode(AstNode node)
        {
            if (node is AstBinaryOp && node.Token.Type == TokenType.COMMA)
            {
                Compiler.Visit(node.Left);
                Compiler.Visit(node.Right);
                return;
            }

            else if (node.Token.Type == TokenType.NOT)
            {
                Compiler.Visit(node.Left);
                IValue ivalue = Compiler.StackIValuePop();
                if (ivalue is BifyType)
                {
                    Traceback.Instance.ThrowException(new BifyTypeError("Invalid operand: type provided instead of value"));
                    return;
                }
                BifyValue value = (BifyValue)ivalue;
                BifyValue result = value.Not(Compiler.Builder);
                Compiler.StackPush(result);
                return;
            }
            else
            {
                Compiler.Visit(node.Left);
                Compiler.Visit(node.Right);
                IValue rhsI = Compiler.StackIValuePop();
                if (rhsI is BifyType)
                {
                    Traceback.Instance.ThrowException(new BifyTypeError("Invalid operand: type provided instead of value"));
                    return;
                }
                BifyValue rhs = (BifyValue)rhsI;
                IValue lhsI = Compiler.StackIValuePop();
                if (lhsI is BifyType)
                {
                    Traceback.Instance.ThrowException(new BifyTypeError("Invalid operand: type provided instead of value"));
                    return;
                }
                BifyValue lhs = (BifyValue)lhsI;
                BifyValue result = BinaryVal(lhs, rhs, node.Token.Type);
                Compiler.StackPush(result);
            }
        }

        private BifyValue BinaryVal(BifyValue lhs, BifyValue rhs, TokenType type)
        {
            switch (type)
            {
                case TokenType.ADD:
                    return lhs.Add(rhs, Compiler.Builder);
                case TokenType.SUB:
                    return lhs.Sub(rhs, Compiler.Builder);
                case TokenType.MUL:
                    return lhs.Mul(rhs, Compiler.Builder);
                case TokenType.DIV:
#if DEBUG_COMPILE
                    if (rhs.CompareType(typeof(IntegerType)) || rhs.CompareType(typeof(FloatType)))
                    {
                        return SafeDiv.Instance(lhs.GetBifyType())
                            .Call([lhs, rhs, new IntegerType().Create(Traceback.Instance.Line)]);
                    }
#endif
                    return lhs.Div(rhs, Compiler.Builder);
                case TokenType.EQ:
                    return lhs.Equal(rhs, Compiler.Builder);
                case TokenType.NEQ:
                    return lhs.NotEqual(rhs, Compiler.Builder);
                case TokenType.LT:
                    return lhs.LessThan(rhs, Compiler.Builder);
                case TokenType.GT:
                    return lhs.GreaterThan(rhs, Compiler.Builder);
                case TokenType.LTEQ:
                    return lhs.LessThanOrEqual(rhs, Compiler.Builder);
                case TokenType.GTEQ:
                    return lhs.GreaterThanOrEqual(rhs, Compiler.Builder);
                case TokenType.OR:
                    if (!lhs.CompareType(typeof(BoolType)) || !rhs.CompareType(typeof(BoolType)))
                    {
                        Traceback.Instance.ThrowException(new BifyTypeError($"Cannot compare {lhs.GetTypeName()} with {rhs.GetTypeName()}"));
                    }
                    return new BoolValue(Compiler.Builder.BuildOr(lhs.GetLlvmValue(), rhs.GetLlvmValue(), "or"));
                case TokenType.AND:

                    
                    if (!lhs.CompareType(typeof(BoolType)) || !rhs.CompareType(typeof(BoolType)))
                    {
                        Traceback.Instance.ThrowException(new BifyTypeError($"Cannot 'and' {lhs.GetTypeName()} with {rhs.GetTypeName()}"));
                    }
                    return new BoolValue(Compiler.Builder.BuildAnd(lhs.GetLlvmValue(), rhs.GetLlvmValue(), "and"));
                default:
                    throw new NotImplementedException($"Not implemented binary operator. Type: {type}");
            }
        }
    }
}
