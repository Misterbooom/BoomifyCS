using System;
using System.Collections.Generic;
using LLVMSharp.Interop;
using BoomifyCS.Assembly.BifyObject;
using BoomifyCS.Ast;
using BoomifyCS.Exceptions;

namespace BoomifyCS.Assembly.NodeHandlers
{
    class ConditionStatementNodeHandler(AssemblyCompiler compiler) : NodeHandler(compiler)
    {
        public override void HandleNode(AstNode node)
        {
            if (node is AstIf astIf)
            {
                HandleIfStatement(astIf);
            }
        }
        private unsafe bool IsCurrentBlockTerminated()
        {  
            var currentBlock = LLVM.GetInsertBlock(compiler.Builder);
            var termiator = LLVM.GetBasicBlockTerminator(currentBlock);
            return termiator != null;
        }

        private void HandleIfStatement(AstIf node)
        {
            AstNode nextNode = compiler.NextNode;
            compiler.Visit(node.ConditionNode);
            var conditionValue = compiler.StackPop();
            if (conditionValue is not BoolValue)
            {
                Traceback.Instance.ThrowException(new BifyTypeError("Condition expression must evaluate to a boolean value."));
            }

            unsafe
            {
                LLVMValueRef func = LLVM.GetBasicBlockParent(LLVM.GetInsertBlock(compiler.Builder));

                LLVMBasicBlockRef mergeBB = nextNode != null ? func.AppendBasicBlock("if_merge") : default;
                LLVMBasicBlockRef thenBB = func.AppendBasicBlock("if_then");

                int numElseIf = node.ElseIfNodes.Count;
                var elseIfCondBlocks = new List<LLVMBasicBlockRef>();
                var elseIfBodyBlocks = new List<LLVMBasicBlockRef>();
                for (int i = 0; i < numElseIf; i++)
                {
                    elseIfCondBlocks.Add(func.AppendBasicBlock($"elseif{i}_cond"));
                    elseIfBodyBlocks.Add(func.AppendBasicBlock($"elseif{i}_body"));
                }

                LLVMBasicBlockRef elseBB = (node.ElseNode != null)
                    ? func.AppendBasicBlock("if_else")
                    : mergeBB;

                LLVMValueRef mainCondition = compiler.Builder.BuildICmp(
                    LLVMIntPredicate.LLVMIntEQ,
                    conditionValue.GetLLVMValue(),
                    conditionValue.GetBifyType().Create(1).GetLLVMValue(),
                    "if_condition"
                );

                if (numElseIf > 0)
                {
                    compiler.Builder.BuildCondBr(mainCondition, thenBB, elseIfCondBlocks[0]);
                }
                else
                {
                    compiler.Builder.BuildCondBr(mainCondition, thenBB, elseBB);
                }

                compiler.Builder.PositionAtEnd(thenBB);
                compiler.Visit(node.BlockNode);
                if (!IsCurrentBlockTerminated() && mergeBB.Handle != IntPtr.Zero)
                {
                    compiler.Builder.BuildBr(mergeBB);
                }

                for (int i = 0; i < numElseIf; i++)
                {
                    compiler.Builder.PositionAtEnd(elseIfCondBlocks[i]);
                    var elseIfNode = node.ElseIfNodes[i];
                    compiler.Visit(elseIfNode.ConditionNode);
                    var elseIfConditionValue = compiler.StackPop();
                    if (elseIfConditionValue is not BoolValue)
                    {
                        Traceback.Instance.ThrowException(new BifyTypeError("Else-if condition must evaluate to a boolean value."));
                    }
                    LLVMValueRef elseifCondition = compiler.Builder.BuildICmp(
                        LLVMIntPredicate.LLVMIntEQ,
                        elseIfConditionValue.GetLLVMValue(),
                        elseIfConditionValue.GetBifyType().Create(1).GetLLVMValue(),
                        $"elseif{i}_condition"
                    );
                    LLVMBasicBlockRef nextCondOrElse = (i < numElseIf - 1)
                        ? elseIfCondBlocks[i + 1]
                        : elseBB;
                    compiler.Builder.BuildCondBr(elseifCondition, elseIfBodyBlocks[i], nextCondOrElse);

                    compiler.Builder.PositionAtEnd(elseIfBodyBlocks[i]);
                    compiler.Visit(elseIfNode.BlockNode);
                    if (!IsCurrentBlockTerminated() && mergeBB.Handle != IntPtr.Zero)
                    {
                        compiler.Builder.BuildBr(mergeBB);
                    }
                }

                if (node.ElseNode != null)
                {
                    compiler.Builder.PositionAtEnd(elseBB);
                    compiler.Visit(node.ElseNode.BlockNode);
                    if (!IsCurrentBlockTerminated() && mergeBB.Handle != IntPtr.Zero)
                    {
                        compiler.Builder.BuildBr(mergeBB);
                    }
                }
                if (mergeBB.Handle != IntPtr.Zero)
                {
                    mergeBB.MoveAfter(LLVM.GetInsertBlock(compiler.Builder));
                    compiler.Builder.PositionAtEnd(mergeBB);
                }
            }
        }
    }
}
