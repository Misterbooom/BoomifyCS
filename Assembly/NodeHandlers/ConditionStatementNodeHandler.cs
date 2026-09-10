using System;
using LLVMSharp.Interop;
using BoomifyCS.Assembly.BifyObject;
using BoomifyCS.Ast;

namespace BoomifyCS.Assembly.NodeHandlers
{
    /// <summary>
    /// Generates LLVM IR for if-else and elseif statements.
    /// </summary>
    internal class ConditionStatementNodeHandler(AssemblyCompiler compiler) : NodeHandler(compiler)
    {
        public override void HandleNode(AstNode node)
        {
            if (node is AstIf ifNode)
                EmitIfStatement(ifNode);
        }

        private unsafe void EmitIfStatement(AstIf ifNode)
        {
            var function = Compiler.Function;

            var thenBlock = function.AppendBasicBlock("if.then");
            LLVMBasicBlockRef elseBlock = default;
            if (ifNode.ElseNode != null)
                elseBlock = function.AppendBasicBlock("if.else");

            var elseifBlocks = new LLVMBasicBlockRef[ifNode.ElseIfNodes.Count];
            for (int i = 0; i < elseifBlocks.Length; i++)
                elseifBlocks[i] = function.AppendBasicBlock($"if.elseif.{i}");

            var mergeBlock = function.AppendBasicBlock("if.merge");

            EmitBranch(ifNode.ConditionNode, thenBlock, elseifBlocks.Length > 0 ? elseifBlocks[0] : (elseBlock.Handle != IntPtr.Zero ? elseBlock : mergeBlock));
            EmitBlock(thenBlock, ifNode.BlockNode, mergeBlock);

            for (int i = 0; i < ifNode.ElseIfNodes.Count; i++)
            {
                var current = ifNode.ElseIfNodes[i];
                var nextTarget = i < ifNode.ElseIfNodes.Count - 1
                    ? elseifBlocks[i + 1]
                    : (elseBlock.Handle != IntPtr.Zero ? elseBlock : mergeBlock);

                EmitBranch(current.ConditionNode, elseifBlocks[i], nextTarget);
                EmitBlock(elseifBlocks[i], current.BlockNode, mergeBlock);
            }

            if (ifNode.ElseNode != null)
                EmitBlock(elseBlock, ifNode.ElseNode.BlockNode, mergeBlock);

            Compiler.Builder.PositionAtEnd(mergeBlock);
        }

        private void EmitBranch(AstNode condition, LLVMBasicBlockRef trueBlock, LLVMBasicBlockRef falseBlock)
        {
            Compiler.Visit(condition);
            var boolVal = Compiler.StackPop<BoolValue>("Condition must be boolean.");
            Compiler.Builder.BuildCondBr(boolVal.GetLlvmValue(), trueBlock, falseBlock);
        }

        private void EmitBlock(LLVMBasicBlockRef block, AstNode body, LLVMBasicBlockRef mergeBlock)
        {
            Compiler.VariableManager.EnterLocalScope();
            Compiler.Builder.PositionAtEnd(block);
            Compiler.Visit(body);
            Compiler.Builder.BuildBr(mergeBlock);
            Compiler.VariableManager.ExitLocalScope();
        }
    }
}
