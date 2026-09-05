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
                if (!Compiler.ReturnType.CompareType(typeof(VoidType)))
                {
                    Traceback.Instance.ThrowException(
                        new BifyTypeError(ErrorMessage.InvalidFunctionReturnType("void", Compiler.ReturnType.Name)));
                    return;
                }
                Compiler.Builder.BuildRetVoid();
                return;
            }
            Compiler.Visit(returnNode.ArgumentsNode);
            IValue returnIValue = Compiler.StackIValuePop();
            if (returnIValue is BifyType)
            {
                Traceback.Instance.ThrowException(
                    new BifyTypeError("Invalid return: a type was provided instead of a runtime value."));
                return;
            }
            BifyValue returnValue = (BifyValue)returnIValue;
            if (!Compiler.ReturnType.CompareType(returnValue.GetBifyType()))
            {
                Traceback.Instance.ThrowException(
                    new BifyTypeError(ErrorMessage.InvalidFunctionReturnType(returnValue.GetTypeName(), Compiler.ReturnType.Name)));
                return;
            }
            Compiler.Builder.BuildRet(returnValue.GetLlvmValue());
        }
    }
}
