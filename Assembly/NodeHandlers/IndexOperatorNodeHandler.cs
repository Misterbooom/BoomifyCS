using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomifyCS.Assembly.BifyObject;
using BoomifyCS.Ast;
using BoomifyCS.Exceptions;

namespace BoomifyCS.Assembly.NodeHandlers
{
    class IndexOperatorNodeHandler : NodeHandler
    {
        public IndexOperatorNodeHandler(AssemblyCompiler compiler) : base(compiler)
        {
        }

        public override void HandleNode(AstNode node)
        {
            AstIndexOperator indexOperatorNode = (AstIndexOperator)node;

            compiler.Visit(indexOperatorNode.OperandNode);
            IValue operandValue = compiler.StackIValuePop();
            if (operandValue is BifyType operandType)
            {
                uint indexValue = 0;

                if (indexOperatorNode.IndexNode != null)
                {
                    if (indexOperatorNode.IndexNode is AstNumber indexNumber)
                    {
                        indexValue = (uint)(int)indexNumber.Value;
                    }
                    else if (operandValue is BifyType)
                    {
                        Traceback.Instance.ThrowException(new BifyParsingError("Index operator currently supports only numbers as index"));
                    }
                }
                ArrayType arrayType = new ArrayType(operandType, indexValue);
                compiler.StackPush(arrayType);
            }
            else if (operandValue is BifyValue operandBifyValue)
            {
                compiler.Visit(indexOperatorNode.IndexNode);
                var indexValue = compiler.StackPop();


                if (!compiler.Flag.HasFlag(NodeVisitFlag.ASSIGNMENT_INDEX))
                {
                    BifyValue indexedResult = operandBifyValue.Index(indexValue, compiler.Builder);
                    if (!indexedResult.CompareType(typeof(BifyPointerType)))
                    {
                        Traceback.Instance.ThrowException(new BifyTypeError("Indexing operation must return a pointer type"));
                    }
                    PointerValue pointerValue = (PointerValue)indexedResult;
                    BifyDebug.Log($"Return value while indexing: {indexedResult} index result type: {indexedResult.GetBifyType()}");
                    compiler.StackPush(pointerValue.Dereference());
                }
                else
                {
                    BifyDebug.Log($"res: {operandBifyValue}");

                    BifyValue indexedResult = operandBifyValue.Index(indexValue, compiler.Builder);
                    compiler.StackPush(indexedResult);
                    compiler.Flag &= ~NodeVisitFlag.ASSIGNMENT_INDEX;

                }
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
