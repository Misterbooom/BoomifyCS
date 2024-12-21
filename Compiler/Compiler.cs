﻿using System;
using System.Collections.Generic;
using BoomifyCS.Ast;
using BoomifyCS.Compiler.VM;
using BoomifyCS.Lexer;
using BoomifyCS.Objects;
using BoomifyCS.Parser;

namespace BoomifyCS.Compiler
{
    public class MyCompiler(string[] sourcecode)
    {
        private readonly List<ByteInstruction> _instructions = [];
        private readonly VirtualMachine _vm = new(sourcecode);
        private readonly Stack<ByteInstruction> _breakJumps = new();
        private readonly Stack<ByteInstruction> _continueJumps = new();
        private int _lineCount = 1;
        public void RunVM(AstNode root)
        {
            Visit(root);
            Console.WriteLine($"Instructions count - {_instructions.Count}");
            _instructions.WriteInstructions();
            _vm.Run(_instructions);
        }

        private void Visit(AstNode node)
        {
            if (node == null) return;
            if (node.LineNumber != 0)
            {
                _lineCount = node.LineNumber;
            }
            switch (node)
            {
                case AstLine astLine:
                    Visit(astLine.Child);
                    Visit(astLine.Left);
                    Visit(astLine.Right);
                    break;
                case AstAssignmentOperator astAssignmentOperator:
                    HandleAssignmentOperator(astAssignmentOperator);
                    break;
                case AstUnaryOperator astUnaryOperator:
                    HandleUnaryOperator(astUnaryOperator);
                    break;
                case AstBinaryOp astBinaryOp:
                    HandleBinaryOp(astBinaryOp);
                    break;

                case AstConstant astConstant:
                    HandleConstant(astConstant);
                    break;
                case AstVarDecl astVarDecl:
                    HandleVarDecl(astVarDecl);
                    break;
                case AstIdentifier astIdentifier:
                    HandleIdentifier(astIdentifier);
                    break;
                case AstAssignment astAssignment:
                    HandleAssignment(astAssignment);
                    break;
                case AstIf astIf:
                    CompileIfStatement(astIf);
                    break;
                case AstBlock astBlock:
                    Visit(astBlock.StatementsNode);
                    break;
                case AstElse astElse:
                    Visit(astElse.BlockNode);
                    break;
                case AstCall astCall:
                    HandleCall(astCall);
                    break;
                case AstModule astModule:
                    HandleModule(astModule);
                    break;
                case AstFor astFor:
                    HandleForLoop(astFor);
                    break;
                case AstWhile astWhile:
                    HandleWhileLoop(astWhile);
                    break;
                case AstBreak _:
                    HandleBreak();
                    break;
                case AstContinue _:
                    HandleContinue();
                    break;
                case AstArray astArray:
                    HandleArray(astArray);
                    break;
                case AstIndexOperator astIndexOperator:
                    HandleIndexOperator(astIndexOperator);
                    break;
                case AstFunctionDecl astFunctionDecl:
                    HandleFunctionDeclaration(astFunctionDecl);
                    break;
                default:
                    Visit(node.Left);
                    Visit(node.Right);
                    break;

            }
        }

        private void HandleAssignmentOperator(AstAssignmentOperator astAssignmentOperator)
        {
            Visit(astAssignmentOperator.ValueNode);

            ByteType byteType = astAssignmentOperator.Token.Type switch
            {
                TokenType.ADDE => ByteType.ADDE,
                TokenType.SUBE => ByteType.SUBE,
                TokenType.MULE => ByteType.MULE,
                TokenType.DIVE => ByteType.DIVE,
                TokenType.FLOORDIVE => ByteType.FLOORDIVE,
                TokenType.POWE => ByteType.POWE,
                TokenType.ASSIGN => ByteType.STORE,
                _ => throw new InvalidOperationException($"Invalid assignment operator: {astAssignmentOperator.Token.Value}")
            };

            if (byteType != ByteType.STORE)
            {
                _instructions.Add(new ByteInstruction(byteType, astAssignmentOperator.IdentifierNode.Token.Value, _lineCount));
            }
            else
            {
                _instructions.Add(new ByteInstruction(byteType, astAssignmentOperator.IdentifierNode.Token.Value, _lineCount));
            }
        }
        private void HandleFunctionDeclaration(AstFunctionDecl function)
        {
            ByteInstruction defFunc = new(ByteType.DEF_FUNC, function.functionNameNode.Name, _lineCount);
            _instructions.Add(defFunc);

            if (function.argumentsNode != null)
            {
                void VisitArgument(AstNode node)
                {
                    if (node == null)
                    {
                        return;
                    }
                    if (node is AstIdentifier identifier)
                    {
                        _instructions.Add(new ByteInstruction(ByteType.ADD_ARG, identifier.Token.Value, _lineCount));
                    }

                    VisitArgument(node.Left);
                    VisitArgument(node.Right);

                }
                VisitArgument(function.argumentsNode);
            }
            Visit(function.blockNode);

            ByteInstruction endFunc = new(ByteType.END_DEF_FUNC, _lineCount);
            _instructions.Add(endFunc);
        }

        private void HandleIndexOperator(AstIndexOperator astIndexOperator)
        {
            Visit(astIndexOperator.OperandNode);

            Visit(astIndexOperator.IndexNode);

            _instructions.Add(new ByteInstruction(ByteType.LOAD_ARRAY, _lineCount));
        }

        private void HandleArray(AstArray array)
        {

            Visit(array.ArgumentsNode);
            int argCount = NodeConventer.CountCommaNode(array.ArgumentsNode) + 1;

            _instructions.Add(new ByteInstruction(ByteType.NEW_ARRAY, argCount, _lineCount));

        }


        private void HandleUnaryOperator(AstUnaryOperator astUnaryOperator)
        {
            _instructions.Add(new ByteInstruction(ByteType.LOAD_CONST, new BifyInteger(1), _lineCount));

            ByteType byteType = astUnaryOperator.Token.Type switch
            {
                TokenType.INCREMENT => ByteType.ADDE,
                TokenType.DECREMENT => ByteType.SUBE,
                _ => throw new InvalidOperationException($"Invalid unary operator: {astUnaryOperator.Token.Value}")
            };

            _instructions.Add(new ByteInstruction(byteType, astUnaryOperator.value.Token.Value, _lineCount));
        }

        private void HandleBinaryOp(AstBinaryOp astBinaryOp)
        {
            Visit(astBinaryOp.Left);
            Visit(astBinaryOp.Right);
            if (astBinaryOp is AstRangeOperator)
            {
                _instructions.Add(new ByteInstruction(ByteType.LOAD_CONST, [new BifyRange()], _lineCount));
                _instructions.Add(new ByteInstruction(ByteType.INIT, _lineCount));
                return;
            }
            if (ByteCodeConfig.BinaryOperators.TryGetValue(astBinaryOp.Token.Type, out ByteType byteType))
            {
                _instructions.Add(new ByteInstruction(byteType, _lineCount));
            }
        }

        private void HandleConstant(AstConstant astConstant)
        {
            _instructions.Add(new ByteInstruction(ByteType.LOAD_CONST, astConstant.BifyValue, _lineCount));
        }

        private void HandleVarDecl(AstVarDecl astVarDecl)
        {
            Visit(astVarDecl.AssignmentNode.Right);
            _instructions.Add(new ByteInstruction(ByteType.DEFINE, [astVarDecl.AssignmentNode.Left.Token.Value], _lineCount));
            Visit(astVarDecl.Right);
        }

        private void HandleIdentifier(AstIdentifier astIdentifier)
        {
            _instructions.Add(new ByteInstruction(ByteType.LOAD, astIdentifier.Token.Value, _lineCount));
        }

        private void HandleAssignment(AstAssignment astAssignment)
        {
            Visit(astAssignment.Right);

            if (astAssignment.Left is AstVar astVar)
            {
                BifyVar bifyVar = (BifyVar)astVar.BifyValue;
                _instructions.Add(new ByteInstruction(ByteType.DEFINE, bifyVar, _lineCount));
            }
        }

        private void HandleCall(AstCall astCall)
        {
            Visit(astCall.ArgumentsNode);

            int expectedArgCount = -1;
            if (astCall.ArgumentsNode is AstBinaryOp astBinaryOp && astBinaryOp.Token.Type == TokenType.COMMA)
            {
                expectedArgCount = CountCommaNode(astCall.ArgumentsNode) + 1;

            }
            else if (astCall.ArgumentsNode == null)
            {
                expectedArgCount = 0;
            }
            else
            {
                expectedArgCount = 1;
            }
            Visit(astCall.CallableName);
            _instructions.Add(new ByteInstruction(ByteType.CALL, expectedArgCount, _lineCount));
        }

        private void HandleModule(AstModule astModule)
        {
            _instructions.Add(new ByteInstruction(ByteType.MODULE, [astModule.ModuleName, astModule.ModulePath], _lineCount));
            foreach (AstNode node in astModule.ChildNodes)
            {
                Visit(node);
            }
        }

        private void HandleForLoop(AstFor astFor)
        {
            Visit(astFor.InitNode);
            int startLoopIndex = _instructions.Count;
            Visit(astFor.ConditionNode);

            ByteInstruction jumpIfFalse = new(ByteType.JUMP_IF_FALSE, 0, _lineCount);
            _instructions.Add(jumpIfFalse);

            Visit(astFor.BlockNode);

            if (_continueJumps.Count > 0)
            {
                var continueJump = _continueJumps.Pop();
                continueJump.Value[0] = _instructions.Count - 2;
            }

            Visit(astFor.IncrementNode);

            ByteInstruction jump = new(ByteType.JUMP, startLoopIndex, _lineCount);
            _instructions.Add(jump);

            if (_breakJumps.Count > 0)
            {
                var breakJump = _breakJumps.Pop();
                breakJump.Value[0] = _instructions.Count;
            }

            jumpIfFalse.Value[0] = _instructions.Count;
        }

        private void HandleWhileLoop(AstWhile astWhile)
        {
            int startLoopIndex = _instructions.Count;
            Visit(astWhile.ConditionNode);

            ByteInstruction jumpIfFalse = new(ByteType.JUMP_IF_FALSE, 0, _lineCount);
            _instructions.Add(jumpIfFalse);

            Visit(astWhile.BlockNode);

            ByteInstruction jump = new(ByteType.JUMP, startLoopIndex, _lineCount);
            _instructions.Add(jump);

            if (_continueJumps.Count > 0)
            {
                var continueJump = _continueJumps.Pop();
                continueJump.Value[0] = _instructions.Count - 1;
            }

            if (_breakJumps.Count > 0)
            {
                var breakJump = _breakJumps.Pop();
                breakJump.Value[0] = _instructions.Count;
            }

            jumpIfFalse.Value[0] = _instructions.Count;
        }

        private void HandleBreak()
        {
            ByteInstruction jump = new(ByteType.JUMP, -1, _lineCount);
            _instructions.Add(jump);
            _breakJumps.Push(jump);
        }

        private void HandleContinue()
        {
            ByteInstruction jump = new(ByteType.JUMP, -1, _lineCount);
            _instructions.Add(jump);
            _continueJumps.Push(jump);
        }

        private void CompileIfStatement(AstIf ifNode)
        {
            Visit(ifNode.ConditionNode);
            ByteInstruction jumpIfFalse = new(ByteType.JUMP_IF_FALSE, -1, _lineCount);
            _instructions.Add(jumpIfFalse);

            Visit(ifNode.BlockNode);

            ByteInstruction jumpToEnd = new(ByteType.JUMP, -1, _lineCount);
            _instructions.Add(jumpToEnd);

            jumpIfFalse.Value[0] = _instructions.Count - 1;

            foreach (var elseIfNode in ifNode.ElseIfNodes)
            {
                Visit(elseIfNode.ConditionNode);
                ByteInstruction jumpIfFalseElseIf = new(ByteType.JUMP_IF_FALSE, -1, _lineCount);
                _instructions.Add(jumpIfFalseElseIf);

                Visit(elseIfNode.BlockNode);

                jumpToEnd.Value[0] = _instructions.Count;

                jumpToEnd = new(ByteType.JUMP, -1, _lineCount);
                _instructions.Add(jumpToEnd);

                jumpIfFalseElseIf.Value[0] = _instructions.Count;
            }

            if (ifNode.ElseNode != null)
            {
                Visit(ifNode.ElseNode);
            }

            jumpToEnd.Value[0] = _instructions.Count;
        }
        private static int CountCommaNode(AstNode node)
        {
            if (node == null)
            {
                return 0;
            }
            if (node is AstBinaryOp binaryOp && binaryOp.Token.Type == TokenType.COMMA)
            {
                return 1 + CountCommaNode(node.Left) + CountCommaNode(node.Right);
            }
            return CountCommaNode(node.Left) + CountCommaNode(node.Right);

        }


    }
}