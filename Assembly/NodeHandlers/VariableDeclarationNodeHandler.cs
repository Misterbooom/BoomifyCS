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

            var declaredType = DetermineVariableType(varDecl, varName);
            if (declaredType == null) return;

            var allocaPointer = AllocateVariable(varDecl, varName, declaredType);
            if (allocaPointer == null) return;

            compiler.VariableManager.RegisterLocalVariable(varName, allocaPointer);
        }

        public BifyType? DetermineVariableType(AstVarDecl varDecl, string varName)
        {
            BifyType declaredType;

            if (varDecl.Type is AstIdentifier && varDecl.Type.Token.Value == "var")
            {
                if (varDecl.AssignmentNode.Right == null)
                {
                    Traceback.Instance.ThrowException(new BifyTypeError(
                        $"Cannot use 'var' without an initializer for variable '{varName}'."));
                    return null;
                }

                compiler.Visit(varDecl.AssignmentNode.Right);
                var inferredValue = compiler.StackIValuePop();

                if (inferredValue is not BifyValue inferredBifyValue)
                {
                    Traceback.Instance.ThrowException(new BifyTypeError(
                        $"Cannot infer type for variable '{varName}' from the initializer."));
                    return null;
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
                    return null;
                }

                declaredType = explicitType;
            }

            if (declaredType is ArrayType)
            {
                compiler.StackPush(declaredType);
            }

            return declaredType;
        }

        private BifyValue? AllocateVariable(AstVarDecl varDecl, string varName, BifyType declaredType)
        {
            var alloca = compiler.Builder.BuildAlloca(declaredType.LLVMType, varName);
            var allocaPointer = new AllocaType(declaredType).CreateValueRef(alloca);
            var variableValue = GetVariableValue(varDecl, varName,declaredType);
            allocaPointer = new AllocaType(declaredType).CreateValueRef(alloca);

            compiler.Builder.BuildStore(variableValue.GetLLVMValue(), alloca);

            if (varDecl.Flag?.Token.Type == TokenType.CONST)
            {
                allocaPointer.ValueFlag = ValueFlag.Constant;
            }
            allocaPointer.ValueFlag |= ValueFlag.Variable;
            return allocaPointer;
        }
        public BifyValue GetVariableValue(AstVarDecl varDecl, string varName, BifyType declaredType)
        {
            if (varDecl.AssignmentNode.Right != null)
            {
                compiler.Visit(varDecl.AssignmentNode.Right);
                var initValue = compiler.StackIValuePop();

                if (initValue is BifyType invalidValueType)
                {
                    Traceback.Instance.ThrowException(new BifyTypeError(
                        $"Cannot assign type '{invalidValueType.Name}' as value for variable '{varName}'."));
                    return null;
                }

                var runtimeValue = (BifyValue)initValue;

                BifyValue castedValue = runtimeValue.ExplicitCast(declaredType, compiler.Builder);
                if (castedValue == null)
                {
                    new BifyTypeError($"Failed to explicitly cast variable '{varName}' from type '{runtimeValue.GetTypeName()}' to the target type '{declaredType.Name}'. Ensure the types are compatible or provide a valid cast.").Throw();
                    return null;
                }
                return castedValue;
            }
            else
            {
                return declaredType.DefaultValue();
            }

        }

    }
}
