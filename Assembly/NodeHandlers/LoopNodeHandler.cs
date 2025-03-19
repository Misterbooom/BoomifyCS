using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomifyCS.Assembly.BifyObject;
using BoomifyCS.Ast;
using BoomifyCS.Exceptions;
using LLVMSharp.Interop;

namespace BoomifyCS.Assembly.NodeHandlers
{
    class LoopNodeHandler : NodeHandler
    {
        public LoopNodeHandler(AssemblyCompiler compiler) : base(compiler) { }

        public override void HandleNode(AstNode node)
        {
            if (node is AstFor astFor)
            {
                HandleFor(astFor);
            }
            else if (node is AstWhile astWhile)
            {
                HandleWhile(astWhile);
            }

        }
        private unsafe void HandleWhile(AstWhile astWhile)
        {
            LLVMValueRef func = LLVM.GetBasicBlockParent(LLVM.GetInsertBlock(compiler.builder));

            LLVMBasicBlockRef conditionBB = func.AppendBasicBlock("while.cond");
            LLVMBasicBlockRef bodyBB = func.AppendBasicBlock("while.body");
            LLVMBasicBlockRef mergeBB = func.AppendBasicBlock("while.end");

            compiler.builder.BuildBr(conditionBB);

            compiler.builder.PositionAtEnd(bodyBB);
            compiler.Visit(astWhile.BlockNode);
            compiler.builder.BuildBr(conditionBB);

            compiler.builder.PositionAtEnd(conditionBB);
            compiler.Visit(astWhile.ConditionNode);
            var conditionValue = compiler.StackPop();
            if (conditionValue.GetBifyType().GetType() != typeof(BoolType))
            {
                Traceback.Instance.ThrowException(new BifyTypeError("Condition expression must evaluate to a boolean value."));
                return;
            }
            compiler.builder.BuildCondBr(conditionValue.GetLLVMValue(), bodyBB, mergeBB);

            compiler.builder.PositionAtEnd(mergeBB);
        }

        private unsafe void HandleFor(AstFor astFor)
        {

            LLVMValueRef func = LLVM.GetBasicBlockParent(LLVM.GetInsertBlock(compiler.builder));

            LLVMBasicBlockRef conditionBB = func.AppendBasicBlock("for.cond");
            LLVMBasicBlockRef bodyBB = func.AppendBasicBlock("for.body");
            LLVMBasicBlockRef incrementBB = func.AppendBasicBlock("for.inc");
            LLVMBasicBlockRef mergeBB = func.AppendBasicBlock("for.end");

            compiler.Visit(astFor.InitNode);
            compiler.builder.BuildBr(conditionBB);



            compiler.builder.PositionAtEnd(bodyBB);
            compiler.Visit(astFor.BlockNode);
            compiler.builder.BuildBr(incrementBB);

            compiler.builder.PositionAtEnd(incrementBB);
            compiler.Visit(astFor.IncrementNode);
            compiler.builder.BuildBr(conditionBB);

            compiler.builder.PositionAtEnd(conditionBB);
            compiler.Visit(astFor.ConditionNode);
            var conditionValue = compiler.StackPop();
            if (conditionValue.GetBifyType().GetType() != typeof(BoolType))
            {
                Traceback.Instance.ThrowException(new BifyTypeError("Condition expression must evaluate to a boolean value."));
                return;
            }
            compiler.builder.BuildCondBr(conditionValue.GetLLVMValue(), bodyBB, mergeBB);
            compiler.builder.PositionAtEnd(mergeBB);
        }
    }



}
