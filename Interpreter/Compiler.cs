using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomifyCS.Ast;
using BoomifyCS.Objects;
using BoomifyCS.Interpreter.VM;
using BoomifyCS.Lexer;
namespace BoomifyCS.Interpreter
{
    public class MyCompiler
    {
        List<ByteInstruction> instructions = new List<ByteInstruction>();
        private VirtualMachine VM;
        private int _lineCount = 1;
        public MyCompiler(string[] sourcecode) {
            VM = new VirtualMachine(sourcecode);
        }
        public void runVM(AstNode root)
        {
            Visit(root);
            instructions.WriteBytes();
            VM.Run(instructions);

        }
        public object Visit(AstNode node)
        {
            if (node == null)
            {
                return null;
            }
            if (node is AstLine astLine)
            {
                Visit(astLine.Left);
                Visit(astLine.Right);
                _lineCount++;
            }
            if (node is AstBinaryOp astBinaryOp)
            {
                Visit(astBinaryOp.Left);
                Visit(astBinaryOp.Right);
                
                if (ByteCodeConfig.BinaryOperators.TryGetValue(astBinaryOp.Token.Type, out ByteType byteType))
                {
                    ByteInstruction instruction = new ByteInstruction(byteType, _lineCount);
                    instructions.Add(instruction);
                    return instruction;
                }

            }
            else if (node is AstConstant astConstant)
            {
                instructions.Add(new ByteInstruction(ByteType.LOAD_CONST, astConstant.BifyValue, _lineCount));

            }
            else if (node is AstVarDecl astVarDecl)
            {
                Visit(astVarDecl.AssignmentNode);
                Visit(node.Right);
            }
            else if (node is AstIdentifier astIdentifier)
            {
                ByteInstruction instruction = new ByteInstruction(ByteType.LOAD,astIdentifier.Token.Value,_lineCount);
                instructions.Add(instruction);
            }
            else if (node is AstAssignment astAssignment)
            {
                Visit(node.Right);

                if (astAssignment.Left is AstVar astVar)
                {
                    BifyVar bifyVar = (BifyVar)astVar.BifyValue;
                    instructions.Add(new ByteInstruction(ByteType.STORE, bifyVar, _lineCount));
                }

            }
            else if (node is AstIf astIf)
            {
                _CompileIfStatement(astIf);
            }
            else if (node is AstBlock astBlock)
            {
                Visit(astBlock.StatementsNode);

            }
            else if (node is AstElse astElse)
            {
                Visit(astElse.BlockNode);
            } 
            else if (node is AstCall astCall)
            {
                Visit(astCall.ArgumentsNode);
                int expectedArgCount = astCall.ArgumentsNode.Len();
                Console.WriteLine($"Excepected arg count - {expectedArgCount}");
                ByteInstruction instruction = new ByteInstruction(ByteType.CALL,new List<object> { astCall.CallableName.Token.Value, expectedArgCount },_lineCount);
                instructions.Add(instruction);
                

            }
            
            


            return null;
        }
        private void _CompileIfStatement(AstIf ifNode)
        {
            Visit(ifNode.ConditionNode);

            int jumpIfFalseIndex = instructions.Count;
            instructions.Add(new ByteInstruction(ByteType.JUMP_IF_FALSE, null, _lineCount));

            Visit(ifNode.BlockNode);

            int jumpToEndIndex = instructions.Count;
            instructions.Add(new ByteInstruction(ByteType.JUMP, null, _lineCount));

            instructions[jumpIfFalseIndex].SetValue(jumpToEndIndex);

            foreach (var elseIf in ifNode.ElseIfNodes)
            {
                Visit(elseIf.ConditionNode);

                int jumpElseIfFalseIndex = instructions.Count;
                instructions.Add(new ByteInstruction(ByteType.JUMP_IF_FALSE, null, _lineCount));

                Visit(elseIf.BlockNode);

                int jumpElseIfEndIndex = instructions.Count;
                instructions.Add(new ByteInstruction(ByteType.JUMP, null, _lineCount));

                instructions[jumpElseIfFalseIndex].SetValue(jumpElseIfEndIndex);
                instructions[jumpToEndIndex].SetValue(jumpElseIfEndIndex);

                jumpToEndIndex = jumpElseIfEndIndex;
            }

            if (ifNode.ElseNode != null)
            {
                Visit(ifNode.ElseNode);
            }

            instructions[jumpToEndIndex].SetValue(instructions.Count);
        }

    }}
