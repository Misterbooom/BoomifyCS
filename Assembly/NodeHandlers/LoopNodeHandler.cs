using LLVMSharp.Interop;
using BoomifyCS.Assembly.BifyObject;
using BoomifyCS.Ast;
using BoomifyCS.Exceptions;
using BoomifyCS.Lexer;

namespace BoomifyCS.Assembly.NodeHandlers
{
    class LoopNodeHandler(AssemblyCompiler compiler) : NodeHandler(compiler)
    {
        public override void HandleNode(AstNode node)
        {
            if (node is AstFor astFor)
                HandleFor(astFor);
            else if (node is AstWhile astWhile)
                HandleWhile(astWhile);
        }

        public  unsafe LLVMValueRef EvaluateBooleanCondition(AstNode conditionNode, string conditionName)
        {
            Compiler.Visit(conditionNode);
            IValue conditionIValue = Compiler.StackIValuePop();
            if (conditionIValue is BifyType)
            {
                Traceback.Instance.ThrowException(new BifyTypeError("Condition expression provided a type instead of a runtime value."));
                return default;
            }
            BifyValue conditionValue = (BifyValue)conditionIValue;
            if (conditionValue.GetBifyType().GetType() != typeof(BoolType))
            {
                Traceback.Instance.ThrowException(new BifyTypeError("Condition expression must evaluate to a boolean value."));
                return default;
            }
            return conditionValue.GetLlvmValue();
        }

        private unsafe bool IsCurrentBlockTerminated()
        {
            var currentBlock = LLVM.GetInsertBlock(Compiler.Builder);
            var lastInstruction = LLVM.GetLastInstruction(currentBlock);
            var terminator = LLVM.GetBasicBlockTerminator(currentBlock);
            return terminator != null;
        }

        private unsafe void PositionBuilderAt(LLVMBasicBlockRef block)
        {
            Compiler.Builder.PositionAtEnd(block);
        }

        private unsafe void HandleWhile(AstWhile astWhile)
        {
            LLVMValueRef func = LLVM.GetBasicBlockParent(LLVM.GetInsertBlock(Compiler.Builder));
            LLVMBasicBlockRef conditionBb = func.AppendBasicBlock("while.cond");
            LLVMBasicBlockRef bodyBb = func.AppendBasicBlock("while.body");
            LLVMBasicBlockRef mergeBb = func.AppendBasicBlock("while.end");
            LoopContext loopContext = new LoopContext(conditionBb, mergeBb, bodyBb);
            Compiler.LoopManager.AddLoop(loopContext);
            Compiler.Builder.BuildBr(conditionBb);
            PositionBuilderAt(bodyBb);

            Compiler.VariableManager.EnterLocalScope();
            Compiler.Visit(astWhile.BlockNode);
            Compiler.VariableManager.ExitLocalScope();
            Compiler.LoopManager.PopLoop();
            Compiler.Builder.BuildBr(conditionBb);
            PositionBuilderAt(conditionBb);
            LLVMValueRef conditionLlvm = EvaluateBooleanCondition(astWhile.ConditionNode, "while_condition");
            Compiler.Builder.BuildCondBr(conditionLlvm, bodyBb, mergeBb);
            PositionBuilderAt(mergeBb);
        }

        private unsafe (LLVMBasicBlockRef conditionBB, LLVMBasicBlockRef bodyBB, LLVMBasicBlockRef mergeBB, LLVMBasicBlockRef incrementBB)
            CreateForLoopBlocks(LLVMValueRef func)
        {
            var conditionBb = func.AppendBasicBlock("for.cond");
            var bodyBb = func.AppendBasicBlock("for.body");
            var incrementBb = func.AppendBasicBlock("for.inc");
            var mergeBb = func.AppendBasicBlock("for.end");
            return (conditionBb, bodyBb, mergeBb, incrementBb);
        }

        private unsafe void HandleFor(AstFor astFor)
        {
            LLVMValueRef func = LLVM.GetBasicBlockParent(LLVM.GetInsertBlock(Compiler.Builder));
            var (conditionBb, bodyBb, mergeBb, incrementBb) = CreateForLoopBlocks(func);
            ((AstBlock)astFor.BlockNode).ChildNodes.Add(new AstContinue(new Token(TokenType.CONTINUE, "continue")));
            LoopContext loopContext = new LoopContext(conditionBb, mergeBb, incrementBb);
            Compiler.LoopManager.AddLoop(loopContext);
            Compiler.VariableManager.EnterLocalScope();
            Compiler.Visit(astFor.InitNode);
            Compiler.Builder.BuildBr(conditionBb);
            PositionBuilderAt(bodyBb);
            Compiler.Visit(astFor.BlockNode);
            Compiler.LoopManager.PopLoop();
            if (Compiler.LoopManager.GetCurrentBranch() == 0)
                Compiler.Builder.BuildBr(incrementBb);
            PositionBuilderAt(incrementBb);
            Compiler.Visit(astFor.IncrementNode);
            Compiler.Builder.BuildBr(conditionBb);
            PositionBuilderAt(conditionBb);
            LLVMValueRef conditionLlvm = EvaluateBooleanCondition(astFor.ConditionNode, "for_condition");
            Compiler.Builder.BuildCondBr(conditionLlvm, bodyBb, mergeBb);
            PositionBuilderAt(mergeBb);
            Compiler.VariableManager.ExitLocalScope();
        }
    }
}
