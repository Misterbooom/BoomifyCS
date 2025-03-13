using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomifyCS.Assembly.BifyObject;
using BoomifyCS.Ast;
using BoomifyCS.Exceptions;
using BoomifyCS.Lexer;

namespace BoomifyCS.Assembly.NodeHandlers
{
    class UnaryOperatorNodeHandler : NodeHandler
    {
        public UnaryOperatorNodeHandler(AssemblyCompiler compiler) : base(compiler) { }
        public override void HandleNode(AstNode node)
        {
            AstUnaryOperator unaryOperator = (AstUnaryOperator)node;
            compiler.Visit(unaryOperator.value);
            BifyValue varValue = compiler.StackPop();
            BifyValue varPtr = compiler.variableManager.GetBifyValue(unaryOperator.value.Token.Value);
            BifyValue newValue;

            if (unaryOperator.Token.Type == TokenType.INCREMENT)
            {
                newValue = varValue.Add(new IntegerType().Create(1),compiler.builder);
            }
            else
            {
                newValue = varValue.Sub(new IntegerType().Create(1), compiler.builder);

            }
            //compiler.variableManager.SetLocalVariable(unaryOperator.value.Token.Value,newValue);
            compiler.builder.BuildStore(newValue.GetLLVMValue(),varPtr.GetLLVMValue());


        }
    }
}
