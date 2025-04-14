using System;
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
                    Traceback.Instance.ThrowException(
                        new BifyTypeError(ErrorMessage.InvalidFunctionReturnType("void", compiler.ReturnType.Name)));
                    return;
                }
                compiler.Builder.BuildRetVoid();
                return;
            }
            compiler.Visit(returnNode.ArgumentsNode);
            IValue returnIValue = compiler.StackIValuePop();
            if (returnIValue is BifyType)
            {
                Traceback.Instance.ThrowException(
                    new BifyTypeError("Invalid return: a type was provided instead of a runtime value."));
                return;
            }
            BifyValue returnValue = (BifyValue)returnIValue;
            if (!compiler.ReturnType.CompareType(returnValue.GetBifyType()))
            {
                Traceback.Instance.ThrowException(
                    new BifyTypeError(ErrorMessage.InvalidFunctionReturnType(returnValue.GetTypeName(), compiler.ReturnType.Name)));
                return;
            }
            compiler.Builder.BuildRet(returnValue.GetLLVMValue());
        }
    }
}
