using System;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using BoomifyCS.Assembly.BifyObject;
using BoomifyCS.Ast;
using BoomifyCS.Exceptions;
using BoomifyCS.Lexer;
using LLVMSharp.Interop;

namespace BoomifyCS.Assembly.NodeHandlers
{
    class ModuleHandler(AssemblyCompiler compiler) : NodeHandler(compiler)
    {
        public override void HandleNode(AstNode node)
        {
            AstModule moduleNode = (AstModule)node;
            foreach (AstNode child in moduleNode.ChildNodes)
            {
                compiler.Visit(child);
            }
        }
    }
    class BlockNodeHandler(AssemblyCompiler compiler) : NodeHandler(compiler)
    {
        public override void HandleNode(AstNode node)
        {
            AstBlock blockNode = (AstBlock)node;
            var locals = compiler.VariableManager.GetLocals();
            compiler.VariableManager.EnterLocalScope();
            compiler.VariableManager.SetCurrentLocalScope(locals);
            for (int i = 0; i < blockNode.ChildNodes.Count; i++)
            {
                AstNode child = blockNode.ChildNodes[i];
                compiler.NextNode = blockNode.ChildNodes.ElementAtOrDefault(i + 1);
                compiler.Visit(child);
                if (child is AstContinue || child is AstBreak || child is AstReturn)
                {
                    return;
                }
            }
            compiler.VariableManager.ExitLocalScope();
        }
    }
    class IdentifierNodeHandler(AssemblyCompiler compiler) : NodeHandler(compiler)
    {
        public override void HandleNode(AstNode node)
        {
            var variable = compiler.VariableManager.GetVariable(node.Token.Value);

            if (variable is BifyType bifyType)
            {
                compiler.StackPush(bifyType);
                return;
            }
            else if (compiler.Flag.HasFlag(NodeVisitFlag.ASSIGNMENT_INDEX))
            {
                compiler.StackPush(variable);
                compiler.Flag &= ~NodeVisitFlag.ASSIGNMENT_INDEX;
                return;
            }
            else if (((BifyValue)variable).GetBifyType() is AllocaType allocaType)
            {
                if (allocaType.PointedType is ArrayType arrayType) {
                    compiler.StackPush(arrayType.CreateValueRef(variable.GetLLVMValue()));
                    return;
                }
                LLVMValueRef valueRef = compiler.Builder.BuildLoad2(allocaType.PointedType.LLVMType, variable.GetLLVMValue(), "loaded_" + node.Token.Value);
                compiler.StackPush(allocaType.PointedType.CreateValueRef(valueRef));

            }
            else
            {
                compiler.StackPush(variable);
            }
        }
    }
    class ConstantNodeHandler : NodeHandler
    {
        public ConstantNodeHandler(AssemblyCompiler compiler) : base(compiler) { }

        public override void HandleNode(AstNode node)
        {
            unsafe
            {
                if (node is AstNumber astNumber)
                {


                    compiler.StackPush(new IntegerType().Create(astNumber.Value));

                }
                else if (node is AstFloat astFloat)
                {
                    compiler.StackPush(new FloatType().Create(astFloat.Value));
                }
                else if (node is AstString astString)
                {
                    if (astString.Token.Type == TokenType.STRING)
                        compiler.StackPush(new ConstStringType().Create((string)astString.Value));
                    else
                    {
                        if (astString.Token.Value.Length > 1)
                        {
                            Traceback.Instance.ThrowException(new BifyTypeError("Char type can only have one character"));
                            return;
                        }
                        compiler.StackPush(new CharType().Create(astString.Token.Value[0]));
                    }
                }
            }
        }
    }
}
