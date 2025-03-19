using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomifyCS.Assembly.BifyObject;
using BoomifyCS.Ast;
using BoomifyCS.Exceptions;
using NUnit.Framework.Constraints;

namespace BoomifyCS.Assembly.NodeHandlers
{
    class IndexOperatorNodeHandler:NodeHandler
    {
        public IndexOperatorNodeHandler(AssemblyCompiler compiler):base(compiler) {
        }
        public override void HandleNode(AstNode node)
        {
            AstIndexOperator indexOperator = (AstIndexOperator)node;

            compiler.Visit(indexOperator.OperandNode);
            IValue loadedValue = compiler.StackIValuePop();
            uint elementCount = 0;
            if (indexOperator.IndexNode != null)
            {
                if (indexOperator.IndexNode is AstNumber number)
                {
                    elementCount = (uint)(int)number.Value;
                }
                else
                {
                    Traceback.Instance.ThrowException(new BifyParsingError("\"Index operator currently support only number as index"));
                }
            }
            if (loadedValue is BifyType bifyType)
            {
                
                ArrayType arrayType = new ArrayType(bifyType,elementCount);
                compiler.StackPush(arrayType);
            }
            else if (loadedValue is BifyValue bifyValue)
            {
                compiler.Visit(indexOperator.IndexNode);
                BifyValue result = bifyValue.Index(compiler.StackPop(),compiler.builder);
                compiler.StackPush(result);

            }
            else
            {
                throw new NotImplementedException();
            }
            
        }
    }
}
