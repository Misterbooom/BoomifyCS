using BoomifyCS.Assembly.BifyObject;
using BoomifyCS.Ast;
using BoomifyCS.Exceptions;

namespace BoomifyCS.Assembly.NodeHandlers
{
    internal class VariableDeclarationNodeHandler(AssemblyCompiler compiler) : NodeHandler(compiler)
    {

        public override void HandleNode(AstNode node)
        {
            var varDecl = (AstVarDecl)node;
            var varName = varDecl.AssignmentNode.Left.Token.Value;
            if (Compiler.VariableManager.VariableExists(varName))
            {
                new BifyNameError($"Variable redefinded: {varName}").Throw();
                return;
            }
            var (declaredType, initialValue) = DetermineVariableType(varDecl, varName);
            if (declaredType == null) return;

            var allocaPointer = AllocateVariable(varDecl, varName, declaredType, initialValue);
            if (allocaPointer == null) return;

            Compiler.VariableManager.RegisterLocalVariable(varName, allocaPointer);
        }

        public (BifyType, BifyValue) DetermineVariableType(AstVarDecl varDecl, string varName)
        {
            BifyType declaredType;

            if (varDecl.Type is AstIdentifier && varDecl.Type.Token.Value == "var")
            {
                if (varDecl.AssignmentNode.Right == null)
                {
                    return new BifyTypeError(
                        $"Cannot use 'var' without an initializer for variable '{varName}'.").Throw< (BifyType, BifyValue) >();
                }

                Compiler.Visit(varDecl.AssignmentNode.Right);
                var inferredValue = Compiler.StackIValuePop();

                if (inferredValue is not BifyValue inferredBifyValue)
                {
                    return new BifyTypeError(
                        $"Cannot infer type for variable '{varName}' from the initializer.").Throw< (BifyType, BifyValue) >();

                }

                declaredType = inferredBifyValue.GetBifyType();
                return (declaredType, inferredBifyValue);
            }
            Compiler.Visit(varDecl.Type);
            var typeResult = Compiler.StackIValuePop();

            if (typeResult is not BifyType explicitType)
            {
                var invalidType = (BifyValue)typeResult;
                return new BifyTypeError(
                    $"{invalidType.GetTypeName()} cannot be used as a type for variable '{varName}'.").Throw< (BifyType, BifyValue) >();
                
            }

            declaredType = explicitType;
            return (declaredType, null);
        }

        private BifyValue? AllocateVariable(AstVarDecl varDecl, string varName, BifyType declaredType, BifyValue? initialValue)
        {
            var insertBlock = Compiler.Builder.InsertBlock;
            Compiler.PositionBeforeTerminator(Compiler.FunctionEntryBb);
            var alloca = Compiler.Builder.BuildAlloca(declaredType.LlvmType, varName);
            Compiler.Builder.PositionAtEnd(insertBlock);
            var allocaPointer = new AllocaType(declaredType).CreateValueRef(alloca);
           
            BifyDebug.Log($"Initial value : {initialValue}");
            var variableValue = initialValue ?? GetVariableValue(varDecl, varName, declaredType);

            Compiler.Builder.BuildStore(variableValue.GetLlvmValue(), alloca);
            FlagProcessor.SetFlags(FlagContext.VARIABLE, allocaPointer.GetBifyType(), ((AstFlag)varDecl.Flag).Flags);
            allocaPointer.ValueFlag |= ValueFlag.VARIABLE;
            return allocaPointer;
        }

     
        public BifyValue GetVariableValue(AstVarDecl varDecl, string varName, BifyType declaredType)
        {
            if (varDecl.AssignmentNode.Right == null)
                return !declaredType.ValueFlag.HasFlag(ValueFlag.CONSTANT)
                    ? declaredType.DefaultValue()
                    : new BifyTypeError($"A const variable requires a value to be provided").Throw<BifyValue>();
            Compiler.Visit(varDecl.AssignmentNode.Right);
            var initValue = Compiler.StackIValuePop();
                
            if (initValue is BifyType invalidValueType)
            {
                Traceback.Instance.ThrowException(new BifyTypeError(
                    $"Cannot assign type '{invalidValueType.Name}' as value for field '{varName}'."));
                return null;
            }

            var runtimeValue = (BifyValue)initValue;

            BifyValue castedValue = runtimeValue.ExplicitCast(declaredType, Compiler.Builder);
            if (castedValue != null) return castedValue;
            new BifyTypeError($"Failed to explicitly cast field '{varName}' from type '{runtimeValue.GetTypeName()}' to the target type '{declaredType.Name}'. Ensure the types are compatible or provide a valid cast.").Throw();
            return null;

        }

    }
}
