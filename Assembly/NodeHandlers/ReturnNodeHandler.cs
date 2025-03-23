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
    class ReturnNodeHandler(AssemblyCompiler compiler) : NodeHandler(compiler)
    {
        public override void HandleNode(AstNode node)
        {
            AstReturn returnNode = node as AstReturn;
            if (returnNode.ArgumentsNode == null)
            {
                if (!compiler.ReturnType.CompareType(typeof(VoidType)))
                {
                    Traceback.Instance.ThrowException(new BifyTypeError(ErrorMessage.InvalidFunctionReturnType("void",
                        compiler.ReturnType.Name)));
                }
                compiler.Builder.BuildRetVoid();
                return;
            }
            compiler.Visit(returnNode.ArgumentsNode);

            BifyValue returnValue = compiler.StackPop();
            if (!compiler.ReturnType.CompareType(returnValue.GetBifyType()))
            {
                Traceback.Instance.ThrowException(new BifyTypeError(ErrorMessage.InvalidFunctionReturnType(returnValue.GetTypeName(),
                    compiler.ReturnType.Name)));
            }

            compiler.Builder.BuildRet(returnValue.GetLLVMValue());

        }
    }
}
