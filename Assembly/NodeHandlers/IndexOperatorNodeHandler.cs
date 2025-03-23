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

            if (operandValue is BifyType operandType)
            {
                ArrayType arrayType = new ArrayType(operandType, indexValue);
                compiler.StackPush(arrayType);
            }
            else if (operandValue is BifyValue operandBifyValue)
            {
                compiler.Visit(indexOperatorNode.IndexNode);
                BifyValue indexedResult = operandBifyValue.Index(compiler.StackPop(), compiler.Builder);

                if (!indexedResult.CompareType(typeof(BifyPointerType)))
                {
                    Traceback.Instance.ThrowException(new BifyTypeError("Index operator must return a pointer!"));
                }
                BifyPointerType indexedPointerType = (BifyPointerType)indexedResult.GetBifyType();

                if (!compiler.Flag.HasFlag(NodeVisitFlag.DONT_LOAD_INDEX))
                {
                    var loadedValue = compiler.Builder.BuildLoad2(indexedPointerType.PointedType.LLVMType,
                        indexedResult.GetLLVMValue(), "loadedValue");
                    BifyValue bifyValue = indexedPointerType.PointedType.CreateValueRef(loadedValue);
                    BifyDebug.Log($"Loading Value from pointer: {bifyValue}");

                    compiler.StackPush(bifyValue);
                }
                else
                {
                    compiler.StackPush(indexedResult);
                    compiler.Flag &= ~NodeVisitFlag.DONT_LOAD_INDEX;

                }
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
