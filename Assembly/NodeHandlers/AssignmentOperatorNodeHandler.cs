using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomifyCS.Assembly.BifyObject;
using BoomifyCS.Ast;
using BoomifyCS.Exceptions;
using BoomifyCS.Lexer;
using LLVMSharp.Interop;
using NUnit.Framework.Constraints;

namespace BoomifyCS.Assembly.NodeHandlers
{
    class AssignmentOperatorNodeHandler(AssemblyCompiler compiler) : NodeHandler(compiler)
    {
        public override void HandleNode(AstNode node)
        {
            AstAssignmentOperator assignmentOperator = node as AstAssignmentOperator;
            compiler.Visit(assignmentOperator.ValueNode);
            BifyValue value = compiler.StackPop();

            compiler.flag |= NodeVisitFlag.DONT_LOAD_INDEX;
            compiler.Visit(assignmentOperator.IdentifierNode);
            BifyValue variable = compiler.StackPop();

            BifyType targetType = variable.GetBifyType();
            if (targetType is BifyPointerType pointerType)
            {
                targetType = pointerType.PointedType;
            }

            value = value.AutoCast(targetType, compiler.builder);
            BifyValue result;

            switch (assignmentOperator.Token.Type)
            {
                case TokenType.ADDE:
                    result = variable.Add(value, compiler.builder);
                    break;
                case TokenType.SUBE:
                    result = variable.Sub(value, compiler.builder);
                    break;
                case TokenType.MULE:
                    result = variable.Mul(value, compiler.builder);
                    break;
                case TokenType.DIVE:
                    result = variable.Div(value, compiler.builder);
                    break;
                case TokenType.ASSIGN:
                    result = targetType.CreateByValueRef(value.GetLLVMValue());
                    break;
                default:
                    throw new NotImplementedException($"{assignmentOperator.Token}");
            }

            BifyDebug.Log($"Result - {result}");
            compiler.builder.BuildStore(result.GetLLVMValue(), variable.GetLLVMValue());
        }
    }

}
