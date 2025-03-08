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
    class ReturnNodeHandler(AssemblyCompiler compiler): NodeHandler(compiler)
    {
        public override void HandleNode(AstNode node)
        {
            AstReturn returnNode = node as AstReturn;

            compiler.Visit(returnNode.ArgumentsNode);
            BifyValue returnValue = compiler.stack.Pop();
            if (!compiler.returnType.CompareType(returnValue))
            {
                Traceback.Instance.ThrowException(new BifyTypeError(ErrorMessage.InvalidFunctionReturnType(returnValue.GetTypeName(),
                    compiler.returnType.GetTypeName())));
            }
            compiler.builder.BuildRet(returnValue.GetLLVMValue());

        }
    }
}
