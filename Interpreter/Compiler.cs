﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomifyCS.Ast;
using BoomifyCS.Objects;
using BoomifyCS.Interpreter.VM;
using BoomifyCS.Lexer;
using BoomifyCS.Parser;
namespace BoomifyCS.Interpreter
{
    public class MyCompiler(string[] sourcecode)
    {
        readonly List<ByteInstruction> instructions = [];
        private readonly VirtualMachine VM = new(sourcecode);
        private int _lineCount = 1;

        public void RunVM(AstNode root)
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

                Visit(astLine.Child);
                Visit(astLine.Left);
                Visit(astLine.Right);
            }
            if (node.LineNumber != 0)
            {
                _lineCount = node.LineNumber;
                Console.WriteLine(_lineCount);
            }


            if (node is AstAssignmentOperator astAssignmentOperator)
            {
                Visit(astAssignmentOperator.ValueNode);
                ByteType byteType;
                if (astAssignmentOperator.Token.Type == TokenType.ADDE)
                {
                    byteType = ByteType.ADDE;
                }
                else if (astAssignmentOperator.Token.Type == TokenType.SUBE)
                {
                    byteType = ByteType.SUBE;
                }
                else if (astAssignmentOperator.Token.Type == TokenType.MULE)
                {
                    byteType = ByteType.MULE;
                }
                else if (astAssignmentOperator.Token.Type == TokenType.DIVE)
                {
                    byteType = ByteType.DIVE;
                }
                else if (astAssignmentOperator.Token.Type == TokenType.FLOORDIVE)
                {
                    byteType = ByteType.FLOORDIVE;
                }
                else if (astAssignmentOperator.Token.Type == TokenType.POWE)
                {
                    byteType = ByteType.POWE;
                }
                else if (astAssignmentOperator.Token.Type == TokenType.ASSIGN)
                {

                    ByteInstruction instruction = new(ByteType.STORE,astAssignmentOperator.IdentifierNode.Token.Value,_lineCount);
                    instructions.Add(instruction);
                    return null;
                }
                else
                {
                    throw new InvalidOperationException($"Invalid assignment operator: {node.Token.Value}");
                }

                instructions.Add(new ByteInstruction(byteType, astAssignmentOperator.IdentifierNode.Token.Value, _lineCount));
            }
            else if (node is AstUnaryOperator astUnaryOperator)
            {
                instructions.Add(new ByteInstruction(ByteType.LOAD_CONST, new BifyInteger(1), _lineCount));

                if (node.Token.Type == TokenType.INCREMENT)
                {

                    ByteInstruction instruction = new(ByteType.ADDE, astUnaryOperator.value.Token.Value, _lineCount);
                    instructions.Add(instruction);

                }
                else if (node.Token.Type == TokenType.DECREMENT)
                {
                    ByteInstruction instruction = new(ByteType.SUBE, astUnaryOperator.value.Token.Value, _lineCount);
                    instructions.Add(instruction);
                }
            }
            else if (node is AstBinaryOp astBinaryOp)
            {
                Visit(astBinaryOp.Left);
                Visit(astBinaryOp.Right);

                if (ByteCodeConfig.BinaryOperators.TryGetValue(astBinaryOp.Token.Type, out ByteType byteType))
                {
                    ByteInstruction instruction = new(byteType, _lineCount);
                    instructions.Add(instruction);
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
                ByteInstruction instruction = new(ByteType.LOAD, astIdentifier.Token.Value, _lineCount);
                instructions.Add(instruction);
            }
            else if (node is AstAssignment astAssignment)
            {
                Visit(node.Right);

                if (astAssignment.Left is AstVar astVar)
                {
                    BifyVar bifyVar = (BifyVar)astVar.BifyValue;
                    instructions.Add(new ByteInstruction(ByteType.DEFINE, bifyVar, _lineCount));
                }

            }
            else if (node is AstIf astIf)
            {
                CompileIfStatement(astIf);
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
                int expectedArgCount = AstNodeConnector.CountCommaNode(astCall.ArgumentsNode) + 1;
                ByteInstruction instruction = new(ByteType.CALL, [astCall.CallableName.Token.Value, expectedArgCount], _lineCount);
                instructions.Add(instruction);
            }
            else if (node is AstModule astModule)
            {
                ByteInstruction instruction = new(ByteType.MODULE, [astModule.ModuleName, astModule.ModulePath], _lineCount);
                instructions.Add(instruction);
                Visit(astModule.ChildNode);
            }
            else if (node is AstFor astFor)
            {
                Visit(astFor.InitNode);
                int startLoopIndex = instructions.Count; 
                Visit(astFor.ConditionNode);

                ByteInstruction jumpIfFalse = new(ByteType.JUMP_IF_FALSE, 0, _lineCount);
                instructions.Add(jumpIfFalse);

                Visit(astFor.BlockNode);
                Visit(astFor.IncrementNode); 

                ByteInstruction jump = new(ByteType.JUMP, startLoopIndex, _lineCount);
                instructions.Add(jump);
                jumpIfFalse.Value[0] = instructions.Count;




            }
            else
            {
                Visit(node.Left);
                Visit(node.Right);
            }





            return null;
        }
        private void CompileIfStatement(AstIf ifNode)
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
