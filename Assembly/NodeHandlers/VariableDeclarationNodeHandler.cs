using System;
using BoomifyCS.Assembly.BifyObject;
using BoomifyCS.Ast;
using BoomifyCS.Exceptions;
using LLVMSharp.Interop;

namespace BoomifyCS.Assembly.NodeHandlers
{
    class VariableDeclarationNodeHandler : NodeHandler
    {
        public VariableDeclarationNodeHandler(AssemblyCompiler compiler) : base(compiler) { }

        public override void HandleNode(AstNode node)
        {
            AstVarDecl varDeclNode = node as AstVarDecl;
            string varName = varDeclNode.AssignmentNode.Left.Token.Value;
            string typeName = varDeclNode.Type.Token.Value;

            BifyType bifyType = compiler.variableManager.GetBifyType(typeName);


            var alloca = compiler.builder.BuildAlloca(bifyType.LLVMType, varName);
            compiler.Visit(varDeclNode.AssignmentNode.Right);
            BifyValue variableValue = compiler.stack.Pop();

            if (!bifyType.CompareType(variableValue))
            {

                if (variableValue is IntegerValue && bifyType is FloatType)
                {
                    LLVMValueRef floatValue = compiler.builder.BuildSIToFP(variableValue.GetLLVMValue(), LLVMTypeRef.Float, "cast_int_to_float");
                    variableValue = new FloatValue(floatValue);
                }
                else if (variableValue is FloatValue && bifyType is IntegerType)
                {
                    LLVMValueRef intValue = compiler.builder.BuildFPToSI(variableValue.GetLLVMValue(), LLVMTypeRef.Int32, "cast_float_to_int");
                    variableValue = new IntegerValue(intValue);
                }
                else
                {
                    Traceback.Instance.ThrowException(new BifyTypeError($"Cannot assign {variableValue.GetTypeName()} to {typeName}"));
                    return;
                }
            }
            compiler.builder.BuildStore(variableValue.GetLLVMValue(), alloca);

            compiler.variableManager.RegisterLocalVariable(varName, variableValue);




        }
    }
}

