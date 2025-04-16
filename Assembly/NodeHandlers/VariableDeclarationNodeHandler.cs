using System;
using BoomifyCS.Assembly.BifyObject;
using BoomifyCS.Ast;
using BoomifyCS.Exceptions;
using BoomifyCS.Lexer;
using LLVMSharp.Interop;

namespace BoomifyCS.Assembly.NodeHandlers
{
    class VariableDeclarationNodeHandler : NodeHandler
    {
        public VariableDeclarationNodeHandler(AssemblyCompiler compiler) : base(compiler) { }

        public override void HandleNode(AstNode node)
        {
            var varDecl = (AstVarDecl)node;
            var varName = varDecl.AssignmentNode.Left.Token.Value;

            BifyType declaredType;
            if (varDecl.Type is AstIdentifier && varDecl.Type.Token.Value == "var")
            {
                if (varDecl.AssignmentNode.Right == null)
                {
                    Traceback.Instance.ThrowException(new BifyTypeError(
                        $"Cannot use 'var' without an initializer for variable '{varName}'."));
                    return;
                }

                compiler.Visit(varDecl.AssignmentNode.Right);
                var inferredValue = compiler.StackIValuePop();

                if (inferredValue is not BifyValue inferredBifyValue)
                {
                    Traceback.Instance.ThrowException(new BifyTypeError(
                        $"Cannot infer type for variable '{varName}' from the initializer."));
                    return;
                }

                declaredType = inferredBifyValue.GetBifyType();
            }
            else
            {
                compiler.Visit(varDecl.Type);
                var typeResult = compiler.StackIValuePop();

                if (typeResult is not BifyType explicitType)
                {
                    var invalidType = (BifyValue)typeResult;
                    Traceback.Instance.ThrowException(new BifyTypeError(
                        $"{invalidType.GetTypeName()} cannot be used as a type for variable '{varName}'."));
                    return;
                }

                declaredType = explicitType;
            }

            if (declaredType is ArrayType)
            {
                compiler.StackPush(declaredType);
            }

            var alloca = compiler.Builder.BuildAlloca(declaredType.LLVMType, varName);
            var allocaPointer = new AllocaType(declaredType).CreateValueRef(alloca);

            if (varDecl.AssignmentNode.Right != null)
            {
                compiler.Visit(varDecl.AssignmentNode.Right);
                var initValue = compiler.StackIValuePop();

                if (initValue is BifyType invalidValueType)
                {
                    Traceback.Instance.ThrowException(new BifyTypeError(
                        $"Cannot assign type '{invalidValueType.Name}' as value for variable '{varName}'."));
                    return;
                }

                var runtimeValue = (BifyValue)initValue;
                BifyDebug.Log($"Declaring '{varName}' of type {declaredType} with value type {runtimeValue.GetTypeName()}");

                var castedValue = runtimeValue.ExplicitCast(declaredType, compiler.Builder);
                compiler.Builder.BuildStore(castedValue.GetLLVMValue(), alloca);
                allocaPointer = new AllocaType(declaredType).CreateValueRef(alloca);

                if (varDecl.Flag?.Token.Type == TokenType.CONST)
                {
                    allocaPointer.ValueFlag = ValueFlag.Constant;
                }
            }
            else
            {
                if (declaredType is not BifyPointerType)
                {
                    Traceback.Instance.ThrowException(new BifyTypeError(
                        $"Variable '{varName}' must be a pointer type when no initializer is provided, but got '{declaredType.Name}'."));
                    return;
                }
            }

            allocaPointer.ValueFlag |= ValueFlag.Variable;
            compiler.VariableManager.RegisterLocalVariable(varName, allocaPointer);
        }
    }
}
