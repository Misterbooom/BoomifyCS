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
            BifyValue variable = compiler.variableManager.GetBifyValue(assignmentOperator.IdentifierNode.Token.Value);
            BifyValue value = compiler.StackPop();
            value = value.AutoCast(variable.GetBifyType(),compiler.builder);
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
                    variable.SetLLVMValue(value.GetLLVMValue());
                    result = variable;
                    break;
                default:
                    throw new NotImplementedException($"{assignmentOperator.Token}");

            }
            compiler.variableManager.SetLocalVariable(assignmentOperator.IdentifierNode.Token.Value, result);

        }

    }
}
