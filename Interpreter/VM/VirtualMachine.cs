using System;
using System.Collections.Generic;
using System.Linq;
using BoomifyCS.BuiltIn.Function;
using BoomifyCS.Objects;
using BoomifyCS.Exceptions;
namespace BoomifyCS.Interpreter.VM
{
    public class VirtualMachine
    {
        private VarManager _varManager = new VarManager();
        private StackManager _stackManager = new StackManager();

        private List<CallStackFrame> _callStack = new List<CallStackFrame>();
        private List<ByteInstruction> _instructions;
        private string[] SourceCode;
        private string _moduleName;
        private string _modulePath;
        private int _line;
        public VirtualMachine(string[] sourceCode) {
            this.SourceCode = sourceCode;
        }

        public void Run(List<ByteInstruction> instructions)
        {
            try
            {
                ProccesInstructions(instructions);
            }
            catch (BifyError e)
            {
                e.FileName = _modulePath;
                e.CallStack = _callStack;
                e.LineTokensString = SourceCode[_line - 1];
                e.CurrentLine = _line;
                e.PrintException();
                Environment.Exit(1);
            }
        }
        public void ProccesInstructions(List<ByteInstruction> instructions)
        {
            _instructions = instructions;
            int instructionIndex = 0;

            while (instructionIndex < _instructions.Count)
            {
                ByteInstruction instruction = _instructions[instructionIndex];
                _line = instruction.IndexOfInstruction;
                switch (instruction.Type)
                {
                    case ByteType.LOAD_CONST:
                        var constant = instruction.Value[0];
                        _stackManager.Push((BifyObject)constant);
                        break;

                    case ByteType.LOAD:
                        var varName = (string)instruction.Value[0];
                        try
                        {
                            BifyObject varValue = _varManager.GetVariable(varName);
                            _stackManager.Push(varValue);
                        }
                        catch (KeyNotFoundException)
                        {
                            throw new BifyUndefinedError(
                                $"Undefined variable - {varName}",
                                SourceCode[_line - 1],
                                varName,
                                _line
                            );
                        }
                        break;

                    case var _ when ByteCodeConfig.BinaryOperators.ContainsValue(instruction.Type):
                        _ProcessOperator(instruction);
                        break;

                    case ByteType.STORE:
                        if (_stackManager.Count() == 0)
                        {
                            throw new BifyInitializationError(
                                "Variable initialized incorrectly. Make sure to assign a value when declaring the variable.",
                                SourceCode[_line - 1],
                                SourceCode[_line - 1],
                                _line
                            );
                        }

                        var value = _stackManager.Pop();
                        var bifyVar = (BifyVar)instruction.Value[0];
                        _varManager.DefineVariable(bifyVar.Name, value);
                        break;

                    case ByteType.JUMP_IF_FALSE:
                        int jumpIndexFalse = (int)instruction.Value[0];
                        if (_stackManager.Peek().Bool().Value == false)
                        {
                            instructionIndex = jumpIndexFalse - 1;
                        }
                        _stackManager.Pop();
                        break;

                    case ByteType.JUMP_IF_TRUE:
                        int jumpIndexTrue = (int)instruction.Value[0];
                        if (_stackManager.Peek().Bool().Value == true)
                        {
                            instructionIndex = jumpIndexTrue;
                        }
                        _stackManager.Pop();
                        break;

                    case ByteType.JUMP:
                        int jumpIndex = (int)instruction.Value[0];
                        instructionIndex = jumpIndex;
                        break;

                    case ByteType.CALL:
                        List<BifyObject> arguments = new List<BifyObject>();
                        BifyFunction function = (BifyFunction)_varManager.GetVariable((string)instruction.Value[0]);
                        int expectedArgCount = (int)instruction.Value[1];
                        _callStack.Add(new CallStackFrame((string)instruction.Value[0], instructionIndex, _modulePath, SourceCode[_line - 1]));

                        if (expectedArgCount != function.ExpectedArgCount && function.ExpectedArgCount != -1)
                        {
                            if (arguments.Count < function.ExpectedArgCount)
                            {
                                throw new BifyArgumentError(
                                    $"Function '{function.Name}' called with too few arguments. Expected {function.ExpectedArgCount}, but got {arguments.Count}.",
                                    SourceCode[_line - 1],
                                    SourceCode[_line - 1],
                                    _line
                                );
                            }
                            else
                            {
                                throw new BifyArgumentError(
                                    $"Function '{function.Name}' called with too many arguments. Expected {function.ExpectedArgCount}, but got {arguments.Count}.",
                                    SourceCode[_line - 1],
                                    SourceCode[_line - 1],
                                    _line
                                );
                            }
                        }

                        for (int i = 0; i < expectedArgCount; i++)
                        {
                            arguments.Add(_stackManager.Pop());
                        }
                        arguments.Reverse();
                        BifyObject functionReturn = function.Call(arguments);
                        _stackManager.Push(functionReturn);
                        break;
                    case ByteType.MODULE:
                        _moduleName = (string)instruction.Value[0];
                        _modulePath = (string)instruction.Value[1];
                        break;

                    default:
                        throw new InvalidOperationException($"Unknown instruction type: {instruction.Type}");
                }

                instructionIndex++;
            }

            _stackManager.Print();
            _varManager.Print();
        }


        private void _ProcessOperator(ByteInstruction instruction)
        {
            if (ByteCodeConfig.BinaryOperators.ContainsValue(instruction.Type))
            {
                BifyObject right = _stackManager.Pop();
                BifyObject left = _stackManager.Pop();
                try
                {
                    BifyObject result = PerformBinaryOperation(left, right, instruction.Type);
                    _stackManager.Push(result);

                }
                catch (BifyError e)
                {
                    ByteCodeConfig.byteToString.TryGetValue(instruction.Type,out string operatorChar);
                    e.CurrentLine = _line;
                    e.LineTokensString = SourceCode[_line - 1];
                    e.InvalidTokensString = operatorChar;
                    //Console.WriteLine(e.LineTokensString);
                    throw e;
                }
            }

        }

        private BifyObject PerformBinaryOperation(BifyObject a, BifyObject b, ByteType operatorType)
        {
            switch (operatorType)
            {
                case ByteType.ADD:
                    return a.Add(b);
                case ByteType.SUB:
                    return a.Sub(b);
                case ByteType.MUL:
                    return a.Mul(b);
                case ByteType.DIV:
                    return a.Div(b);
                case ByteType.MOD:
                    return a.Mod(b);
                case ByteType.POW:
                    return a.Pow(b);
                case ByteType.FLOORDIV:
                    return a.FloorDiv(b);
                case ByteType.EQ:
                    return a.Eq(b);
                case ByteType.LT:
                    return a.Lt(b);
                case ByteType.GT:
                    return a.Gt(b);
                case ByteType.LTE:
                    return a.Lte(b);
                case ByteType.GTE:
                    return a.Gte(b);
                case ByteType.AND:
                    return a.Bool().And(b.Bool());
                case ByteType.OR:
                    return a.Bool().Or(b.Bool());
                case ByteType.NEQ:
                    return a.Neq(b);
                case ByteType.NOT:
                    return a.Bool().Not();
                case ByteType.BITAND:
                    return a.BitAnd(b);
                case ByteType.BITOR:
                    return a.BitOr(b);
                case ByteType.BITXOR:
                    return a.BitXor(b);
                case ByteType.BITNOT:
                    return a.BitNot();
                case ByteType.LSHIFT:
                    return a.LeftShift(b);
                case ByteType.RSHIFT:
                    return a.RightShift(b);
                default:
                    throw new NotImplementedException($"{operatorType} is not implemented in PerformBinaryOperation");
            }
        }

    }
}
