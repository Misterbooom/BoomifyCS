using BoomifyCS.Assembly.BifyObject;
using BoomifyCS.Ast;
using BoomifyCS.Exceptions;

namespace BoomifyCS.Assembly.NodeHandlers
{
    class CastNodeHandler(AssemblyCompiler compiler) : NodeHandler(compiler)
    {
        public override void HandleNode(AstNode node)
        {
            AstCast castNode = (AstCast)node;
            IValue loadedValue;

            Compiler.Visit(castNode.TypeNode);
            loadedValue = Compiler.StackIValuePop();
            if (loadedValue is not BifyType bifyType)
            {
                new BifyTypeError($"Expected a type but received '{loadedValue?.GetType().Name ?? "null"}'.").Throw();
                return; 
            }

            Compiler.Visit(castNode.ValueNode);
            loadedValue = Compiler.StackIValuePop();
            if (loadedValue is not BifyValue bifyValue)
            {
                new BifyTypeError($"Expected a variable value but received '{loadedValue?.GetType().Name ?? "null"}'.").Throw();
                return; 
            }

            BifyValue castedValue = bifyValue.ExplicitCast(bifyType, Compiler.Builder);
            if (castedValue == null)
            {
                new BifyCastError(
                    $"Failed to cast value of type '{bifyValue.GetTypeName()}' to target type '{bifyType.Name}'. "
                ).Throw();
            }

            Compiler.StackPush(castedValue);
        }
    }
}
