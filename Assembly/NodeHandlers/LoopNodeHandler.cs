using System;
using System.Collections.Generic;
using LLVMSharp.Interop;
using BoomifyCS.Assembly.BifyObject;
using BoomifyCS.Ast;
using BoomifyCS.Exceptions;
using BoomifyCS.Lexer;

namespace BoomifyCS.Assembly.NodeHandlers
{
    class LoopNodeHandler : NodeHandler
    {
        public LoopNodeHandler(AssemblyCompiler compiler) : base(compiler) { }

        public override void HandleNode(AstNode node)
        {
            if (node is AstFor astFor)
                HandleFor(astFor);
            else if (node is AstWhile astWhile)
                HandleWhile(astWhile);
        }

        public  unsafe LLVMValueRef EvaluateBooleanCondition(AstNode conditionNode, string conditionName)
        {
            compiler.Visit(conditionNode);
            IValue conditionIValue = compiler.StackIValuePop();
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
            return conditionValue.GetLLVMValue();
        }

        private unsafe bool IsCurrentBlockTerminated()
        {
            var currentBlock = LLVM.GetInsertBlock(compiler.Builder);
            var lastInstruction = LLVM.GetLastInstruction(currentBlock);
            var terminator = LLVM.GetBasicBlockTerminator(currentBlock);
            return terminator != null;
        }

        private unsafe void PositionBuilderAt(LLVMBasicBlockRef block)
        {
            compiler.Builder.PositionAtEnd(block);
        }

        private unsafe void HandleWhile(AstWhile astWhile)
        {
            LLVMValueRef func = LLVM.GetBasicBlockParent(LLVM.GetInsertBlock(compiler.Builder));
            LLVMBasicBlockRef conditionBB = func.AppendBasicBlock("while.cond");
            LLVMBasicBlockRef bodyBB = func.AppendBasicBlock("while.body");
            LLVMBasicBlockRef mergeBB = func.AppendBasicBlock("while.end");
            LoopContext loopContext = new LoopContext(conditionBB, mergeBB, bodyBB);
            compiler.LoopManager.AddLoop(loopContext);
            compiler.Builder.BuildBr(conditionBB);
            PositionBuilderAt(bodyBB);

            compiler.VariableManager.EnterLocalScope();
            compiler.Visit(astWhile.BlockNode);
            compiler.VariableManager.ExitLocalScope();
            compiler.LoopManager.PopLoop();
            compiler.Builder.BuildBr(conditionBB);
            PositionBuilderAt(conditionBB);
            LLVMValueRef conditionLLVM = EvaluateBooleanCondition(astWhile.ConditionNode, "while_condition");
            compiler.Builder.BuildCondBr(conditionLLVM, bodyBB, mergeBB);
            PositionBuilderAt(mergeBB);
        }

        private unsafe (LLVMBasicBlockRef conditionBB, LLVMBasicBlockRef bodyBB, LLVMBasicBlockRef mergeBB, LLVMBasicBlockRef incrementBB)
            CreateForLoopBlocks(LLVMValueRef func)
        {
            var conditionBB = func.AppendBasicBlock("for.cond");
            var bodyBB = func.AppendBasicBlock("for.body");
            var incrementBB = func.AppendBasicBlock("for.inc");
            var mergeBB = func.AppendBasicBlock("for.end");
            return (conditionBB, bodyBB, mergeBB, incrementBB);
        }

        private unsafe void HandleFor(AstFor astFor)
        {
            LLVMValueRef func = LLVM.GetBasicBlockParent(LLVM.GetInsertBlock(compiler.Builder));
            var (conditionBB, bodyBB, mergeBB, incrementBB) = CreateForLoopBlocks(func);
            ((AstBlock)astFor.BlockNode).ChildNodes.Add(new AstContinue(new Token(TokenType.CONTINUE, "continue")));
            LoopContext loopContext = new LoopContext(conditionBB, mergeBB, incrementBB);
            compiler.LoopManager.AddLoop(loopContext);
            compiler.VariableManager.EnterLocalScope();
            compiler.Visit(astFor.InitNode);
            compiler.Builder.BuildBr(conditionBB);
            PositionBuilderAt(bodyBB);
            compiler.Visit(astFor.BlockNode);
            compiler.LoopManager.PopLoop();
            if (compiler.LoopManager.GetCurrentBranch() == 0)
                compiler.Builder.BuildBr(incrementBB);
            PositionBuilderAt(incrementBB);
            compiler.Visit(astFor.IncrementNode);
            compiler.Builder.BuildBr(conditionBB);
            PositionBuilderAt(conditionBB);
            LLVMValueRef conditionLLVM = EvaluateBooleanCondition(astFor.ConditionNode, "for_condition");
            compiler.Builder.BuildCondBr(conditionLLVM, bodyBB, mergeBB);
            PositionBuilderAt(mergeBB);
            compiler.VariableManager.ExitLocalScope();
        }
    }
}
