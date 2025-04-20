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
            bool loadResultPointer = true;
            if (compiler.Flag.HasFlag(NodeVisitFlag.ASSIGNMENT_INDEX))
            {
                loadResultPointer = false;
                compiler.Flag &= ~NodeVisitFlag.ASSIGNMENT_INDEX;
            }
            compiler.Visit(indexOperatorNode.TargetNode);
            IValue iValue = compiler.StackIValuePop();
            if (iValue is BifyType type)
            {
                HandleArrayType(type, indexOperatorNode.IndexNode);
            }
            else if (iValue is BifyValue targetValue)
            {
                HandleIndexing(targetValue, indexOperatorNode.IndexNode,loadResultPointer);
            }
        }
        private void HandleIndexing(BifyValue targetValue,AstNode indexNode,bool loadResultPointer)
        {
            compiler.Visit(indexNode);
            IValue iValue = compiler.StackIValuePop();

            if (iValue is BifyType)
            {
                new BifyTypeError("Invalid index: a type was provided instead of a runtime value.").Throw();
            }
            BifyValue indexValue = (BifyValue)iValue;
            PointerValue indexedResult = targetValue.Index(indexValue, compiler.Builder) as PointerValue;
            if (indexedResult == null)
            {
                new BifyTypeError("Indexing operation must return a pointer type.").Throw(); 
            }
            
            compiler.StackPush(loadResultPointer ? indexedResult.Dereference() : indexedResult);
            

        }
        private void HandleArrayType(BifyType targetType,AstNode indexNode)
        {
            uint elementCount = 0;
            if (indexNode != null)
            {
                if (indexNode is AstNumber numberNode)
                {
                    elementCount = (uint)(int)numberNode.Value;
                }
                else
                {
                    new BifyParsingError("Index operator currently supports only numeric literal indexes.").Throw();
                }
            }
            ArrayType arrayType = new ArrayType(targetType, elementCount);
            compiler.StackPush(arrayType);
            
        }
    }
}
