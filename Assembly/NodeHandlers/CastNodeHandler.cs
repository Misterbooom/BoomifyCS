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
    class CastNodeHandler : NodeHandler
    {
        public CastNodeHandler(AssemblyCompiler compiler) : base(compiler)
        {
        }

        public override void HandleNode(AstNode node)
        {
            AstCast castNode = (AstCast)node;
            IValue loadedValue;

            compiler.Visit(castNode.TypeNode);
            loadedValue = compiler.StackIValuePop();
            if (loadedValue is not BifyType bifyType)
            {
                new BifyTypeError($"Expected a type but received '{loadedValue?.GetType().Name ?? "null"}'.").Throw();
                return; 
            }

            compiler.Visit(castNode.ValueNode);
            loadedValue = compiler.StackIValuePop();
            if (loadedValue is not BifyValue bifyValue)
            {
                new BifyTypeError($"Expected a variable value but received '{loadedValue?.GetType().Name ?? "null"}'.").Throw();
                return; 
            }

            BifyValue castedValue = bifyValue.ExplicitCast(bifyType, compiler.Builder);
            if (castedValue == null)
            {
                new BifyCastError(
                    $"Failed to cast value of type '{bifyValue.GetTypeName()}' to target type '{bifyType.Name}'. "
                ).Throw();
            }

            compiler.StackPush(castedValue);
        }
    }
}
