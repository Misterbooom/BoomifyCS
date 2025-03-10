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
    class ConditionStatementNodeHandler(AssemblyCompiler compiler) : NodeHandler(compiler)
    {
        public override void HandleNode(AstNode node)
        {
            if (node is AstIf astIf)
            {
                HandleIfStatement(astIf);
            }
        }
        private void HandleIfStatement(AstIf node)
        {
            compiler.Visit(node.ConditionNode);
            var conditionValue = compiler.StackPop();
            if (conditionValue is not BoolValue)
            {
                Traceback.Instance.ThrowException(new BifyTypeError("Condition expression must evaluate to a boolean value."));
            }

            unsafe
            {
                LLVMValueRef func = LLVM.GetBasicBlockParent(LLVM.GetInsertBlock(compiler.builder));

                LLVMBasicBlockRef mergeBB = func.AppendBasicBlock("if_merge");

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

                LLVMValueRef mainCondition = compiler.builder.BuildICmp(
                    LLVMIntPredicate.LLVMIntEQ,
                    conditionValue.GetLLVMValue(),
                    conditionValue.GetBifyType().Create(1).GetLLVMValue(),
                    "if_condition"
                );

                if (numElseIf > 0)
                {
                    compiler.builder.BuildCondBr(mainCondition, thenBB, elseIfCondBlocks[0]);
                }
                else
                {
                    compiler.builder.BuildCondBr(mainCondition, thenBB, elseBB);
                }

                compiler.builder.PositionAtEnd(thenBB);
                compiler.Visit(node.BlockNode);
                compiler.builder.BuildBr(mergeBB);

                for (int i = 0; i < numElseIf; i++)
                {
                    compiler.builder.PositionAtEnd(elseIfCondBlocks[i]);
                    var elseIfNode = node.ElseIfNodes[i];
                    compiler.Visit(elseIfNode.ConditionNode);
                    var elseIfConditionValue = compiler.StackPop();
                    if (elseIfConditionValue is not BoolValue)
                    {
                        Traceback.Instance.ThrowException(new BifyTypeError("Else-if condition must evaluate to a boolean value."));
                    }
                    LLVMValueRef elseifCondition = compiler.builder.BuildICmp(
                        LLVMIntPredicate.LLVMIntEQ,
                        elseIfConditionValue.GetLLVMValue(),
                        elseIfConditionValue.GetBifyType().Create(1).GetLLVMValue(),
                        $"elseif{i}_condition"
                    );
                    LLVMBasicBlockRef nextCondOrElse = (i < numElseIf - 1)
                        ? elseIfCondBlocks[i + 1]
                        : elseBB;
                    compiler.builder.BuildCondBr(elseifCondition, elseIfBodyBlocks[i], nextCondOrElse);

                    compiler.builder.PositionAtEnd(elseIfBodyBlocks[i]);
                    compiler.Visit(elseIfNode.BlockNode);
                    compiler.builder.BuildBr(mergeBB);
                }

                if (node.ElseNode != null)
                {
                    compiler.builder.PositionAtEnd(elseBB);
                    compiler.Visit(node.ElseNode.BlockNode);
                    compiler.builder.BuildBr(mergeBB);
                }

                compiler.builder.PositionAtEnd(mergeBB);
            }
        }

    }
}
