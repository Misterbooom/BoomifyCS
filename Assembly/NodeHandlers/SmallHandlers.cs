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
                Compiler.Visit(child);
            }
        }
    }
    class BlockNodeHandler(AssemblyCompiler compiler) : NodeHandler(compiler)
    {
        public override void HandleNode(AstNode node)
        {
            AstBlock blockNode = (AstBlock)node;
            var locals = Compiler.VariableManager.GetLocals();
            Compiler.VariableManager.SetCurrentLocalScope(locals);
            for (int i = 0; i < blockNode.ChildNodes.Count; i++)
            {
                AstNode child = blockNode.ChildNodes[i];
                Compiler.IsLastNode = i + 1 > blockNode.ChildNodes.Count;
                Compiler.Visit(child);
                if (child is AstContinue || child is AstBreak || child is AstReturn)
                {
                    return;
                }
            }
        }
    }
    class IdentifierNodeHandler(AssemblyCompiler compiler) : NodeHandler(compiler)
    {
        public override void HandleNode(AstNode node)
        {
            var variable = Compiler.VariableManager.GetVariable(node.Token.Value);

            if (variable is BifyType bifyType)
            {
                Compiler.StackPush(bifyType);
                return;
            }
            else if (Compiler.Flag.HasFlag(NodeVisitFlag.ASSIGNMENT_INDEX))
            {
                Compiler.StackPush(variable);
                Compiler.Flag &= ~NodeVisitFlag.ASSIGNMENT_INDEX;
                return;
            }
            else if (((BifyValue)variable).GetBifyType() is AllocaType allocaType)
            {
                if (allocaType.PointedType is ArrayType arrayType) {
                    Compiler.StackPush(arrayType.CreateValueRef(variable.GetLlvmValue()));
                    return;
                }
                LLVMValueRef valueRef = Compiler.Builder.BuildLoad2(allocaType.PointedType.LlvmType, variable.GetLlvmValue(), "loaded_" + node.Token.Value);
                Compiler.StackPush(allocaType.PointedType.CreateValueRef(valueRef));

            }
            else
            {
                Compiler.StackPush(variable);
            }
        }
    }
    class ConstantNodeHandler(AssemblyCompiler compiler) : NodeHandler(compiler)
    {
        public override void HandleNode(AstNode node)
        {
            unsafe
            {
                if (node is AstNumber astNumber)
                {


                    Compiler.StackPush(new IntegerType().Create(astNumber.Value));

                }
                else if (node is AstFloat astFloat)
                {
                    Compiler.StackPush(new FloatType().Create(astFloat.Value));
                }
                else if (node is AstString astString)
                {
                    if (astString.Token.Type == TokenType.STRING)
                        Compiler.StackPush(new ConstStringType().Create((string)astString.Value));
                    else
                    {
                        if (astString.Token.Value.Length > 1)
                        {
                            Traceback.Instance.ThrowException(new BifyTypeError("Char type can only have one character"));
                            return;
                        }
                        Compiler.StackPush(new CharType().Create(astString.Token.Value[0]));
                    }
                }
            }
        }
    }
}
