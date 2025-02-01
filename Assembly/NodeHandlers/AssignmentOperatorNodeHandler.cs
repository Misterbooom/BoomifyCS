using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomifyCS.Ast;
using BoomifyCS.Exceptions;
using BoomifyCS.Lexer;
using BoomifyCS.Objects;

namespace BoomifyCS.Assembly.NodeHandlers
{
    class AssignmentOperatorNodeHandler(AssemblyCompiler compiler) : NodeHandler(compiler)
    {
        public override void HandleNode(AstNode node)
        {
            AstAssignmentOperator assignmentOperatorNode = node as AstAssignmentOperator;

            Variable targetVariable = compiler.variableManager.GetVariable(assignmentOperatorNode.IdentifierNode.Token.Value);
            compiler.Visit(assignmentOperatorNode.ValueNode);
            BifyValue assignedValue = compiler.stack.Pop();

            TokenType operatorType = node.Token.Type;
            BifyValue targetBifyValue = targetVariable.ToBifyValue();
            BifyObject operationResult;
            switch (operatorType)
            {
                case TokenType.ADDE:
                    operationResult = targetBifyValue.GetBifyObject().Add(assignedValue.GetBifyObject());
                    break;
                case TokenType.SUBE:
                    operationResult = targetBifyValue.GetBifyObject().Sub(assignedValue.GetBifyObject());
                    break;
                case TokenType.MULE:
                    operationResult = targetBifyValue.GetBifyObject().Mul(assignedValue.GetBifyObject());
                    break;
                case TokenType.DIVE:
                    operationResult = targetBifyValue.GetBifyObject().Div(assignedValue.GetBifyObject());
                    break;
                case TokenType.POWE:
                    operationResult = targetBifyValue.GetBifyObject().Pow(assignedValue.GetBifyObject());
                    break;
                case TokenType.FLOORDIVE:
                    operationResult = targetBifyValue.GetBifyObject().FloorDiv(assignedValue.GetBifyObject());
                    break;

                default:
                    throw new NotImplementedException($"Unexpected assignment operator {operatorType}");
            }
            if (operationResult.GetType() != targetBifyValue.GetBifyObject().GetType())
            {
                Type targetType = targetVariable.Type;

                if (operationResult is BifyFloat && targetType == typeof(BifyInteger))
                {
                    operationResult = operationResult.Int();
                }
                else if (operationResult is BifyInteger && targetType == typeof(BifyFloat))
                {
                    operationResult = BifyFloat.Convert(operationResult);
                }
                else
                {
                    Traceback.Instance.ThrowException(
                        new BifyCastError($"Cannot assign {operationResult.GetName()} to {targetVariable.BifyObject.GetName()}")
                    );
                    return;
                }
            }

            targetVariable.BifyObject = operationResult;
        }

    }
}
