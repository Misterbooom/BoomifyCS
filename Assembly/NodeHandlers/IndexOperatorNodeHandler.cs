using System;
using System.Collections.Generic;
using BoomifyCS.Ast;
using BoomifyCS.Lexer;
using LLVMSharp.Interop;
using BoomifyCS.Exceptions;
using BoomifyCS.Assembly.BifyObject;

namespace BoomifyCS.Assembly.NodeHandlers
{
    class IndexOperatorNodeHandler : NodeHandler
    {
        public IndexOperatorNodeHandler(AssemblyCompiler compiler) : base(compiler) { }

        public override void HandleNode(AstNode node)
        {
            AstIndexOperator indexOperatorNode = (AstIndexOperator)node;
            compiler.Visit(indexOperatorNode.OperandNode);
            IValue operandIValue = compiler.StackIValuePop();
            if (operandIValue is BifyType operandType)
            {
                uint indexValue = 0;
                if (indexOperatorNode.IndexNode != null)
                {
                    if (indexOperatorNode.IndexNode is AstNumber indexNumber)
                        indexValue = (uint)(int)indexNumber.Value;
                    else
                    {
                        Traceback.Instance.ThrowException(new BifyParsingError("Index operator currently supports only numeric literal indexes."));
                        return;
                    }
                }
                ArrayType arrayType = new ArrayType(operandType, indexValue);
                compiler.StackPush(arrayType);
            }
            else if (operandIValue is BifyValue operandBifyValue)
            {
                compiler.Visit(indexOperatorNode.IndexNode);
                IValue indexIValue = compiler.StackIValuePop();
                if (indexIValue is BifyType)
                {
                    Traceback.Instance.ThrowException(new BifyTypeError("Invalid index: a type was provided instead of a runtime value."));
                    return;
                }
                BifyValue indexValue = (BifyValue)indexIValue;
                BifyValue indexedResult = operandBifyValue.Index(indexValue, compiler.Builder);
              
                if (!indexedResult.CompareType(typeof(BifyPointerType)))
                {
                    Traceback.Instance.ThrowException(new BifyTypeError("Indexing operation must return a pointer type."));
                    return;
                }
                PointerValue pointerValue = (PointerValue)indexedResult;
                BifyDebug.Log($"Return value while indexing: {indexedResult} index result type: {indexedResult.GetBifyType()}");
                if (!compiler.Flag.HasFlag(NodeVisitFlag.ASSIGNMENT_INDEX))
                    compiler.StackPush(pointerValue.Dereference());
                else
                {
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
