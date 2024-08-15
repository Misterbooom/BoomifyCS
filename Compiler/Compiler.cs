using System;
using System.Collections.Generic;
using BoomifyCS.Ast;
using BoomifyCS.Interpreter.VM;
using BoomifyCS.Lexer;
using BoomifyCS.Objects;
using BoomifyCS.Parser;

namespace BoomifyCS.Interpreter
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
        private void HandleIndexOperator(AstIndexOperator astIndexOperator)
        {
            Visit(astIndexOperator.OperandNode);

            Visit(astIndexOperator.IndexNode);

            _instructions.Add(new ByteInstruction(ByteType.LOAD_ARRAY, _lineCount));
        }

        private void HandleArray(AstArray array)
        {

            Visit(array.ArgumentsNode);
            int argCount = AstNodeConnector.CountCommaNode(array.ArgumentsNode) + 1;

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
            Visit(astVarDecl.AssignmentNode);
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
            int expectedArgCount = AstNodeConnector.CountCommaNode(astCall.ArgumentsNode) + 1;
            _instructions.Add(new ByteInstruction(ByteType.CALL, [astCall.CallableName.Token.Value, expectedArgCount], _lineCount));
        }

        private void HandleModule(AstModule astModule)
        {
            _instructions.Add(new ByteInstruction(ByteType.MODULE, [astModule.ModuleName, astModule.ModulePath], _lineCount));
            Visit(astModule.ChildNode);
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
            jumpIfFalse.Value[0] = _instructions.Count;
        }
    }
}
