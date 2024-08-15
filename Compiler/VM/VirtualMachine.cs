using System;
using System.Collections.Generic;
using System.Linq;
using BoomifyCS.BuiltIn.Function;
using BoomifyCS.Objects;
using BoomifyCS.Exceptions;
namespace BoomifyCS.Interpreter.VM
{
    public class VirtualMachine(string[] sourceCode)
    {
        private readonly VarManager _varManager = new();
        private readonly StackManager _stackManager = new();

        private readonly List<CallStackFrame> _callStack = [];
        private List<ByteInstruction> _instructions;
        private readonly string[] SourceCode = sourceCode;
        private string _moduleName;
        private string _modulePath;
        public int line;

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
                e.LineTokensString = SourceCode[line - 1];
                e.CurrentLine = line;
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
                //Console.WriteLine($"{instruction.IndexOfInstruction}:{instruction}");
                line = instruction.IndexOfInstruction;
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
                            BifyObject instrvalue = _varManager.GetVariable(varName);

                            _stackManager.Push(instrvalue);
                        }
                        catch (KeyNotFoundException)
                        {
                            throw new BifyUndefinedError(
                                $"Undefined variable - {varName}",
                                SourceCode[line - 1],
                                varName,
                                line
                            );
                        }
                        break;

                    case var _ when ByteCodeConfig.BinaryOperators.ContainsValue(instruction.Type):
                        ProcessOperator(instruction);
                        break;
                    case var _ when ByteCodeConfig.AssignmentOperators.ContainsValue(instruction.Type):
                        string identifier = (string)instruction.Value[0];
                        BifyObject varValue = _varManager.GetVariable(identifier);
                        varValue = instruction.Type switch
                        {
                            ByteType.ADDE => varValue.Add(_stackManager.Pop()),
                            ByteType.SUBE => varValue.Sub(_stackManager.Pop()),
                            ByteType.MULE => varValue.Mul(_stackManager.Pop()),
                            ByteType.DIVE => varValue.Div(_stackManager.Pop()),
                            ByteType.FLOORDIV => varValue.FloorDiv(_stackManager.Pop()),
                            ByteType.POWE => varValue.Pow(_stackManager.Pop()),
                            _ => throw new NotImplementedException($"Bytecode - {instruction.Type} not implemented in unary op"),
                        };
                        StoreVariable(varValue,identifier);



                        break;


                    case ByteType.DEFINE:
                        if (_stackManager.Count() == 0)
                        {
                            throw new BifyInitializationError(
                                "Variable initialized incorrectly. Make sure to assign a value when declaring the variable.",
                                SourceCode[line - 1],
                                SourceCode[line - 1],
                                line
                            );
                        }

                        var value = _stackManager.Pop();
                        var bifyVar = (BifyVar)instruction.Value[0];
                        _varManager.DefineVariable(bifyVar.Name, value);
                        break;

                    case ByteType.JUMP_IF_FALSE:

                        int jumpIndexFalse = (int)instruction.Value[0];
                        if (_stackManager.Pop().Bool().Value == false)
                        {
                            instructionIndex = jumpIndexFalse;

                            if (instructionIndex > instructions.Count)
                            {

                                break;
                            }
                            continue;
                        }
                        

                        break;


                    case ByteType.JUMP_IF_TRUE:
                        int jumpIndexTrue = (int)instruction.Value[0];
                        if (_stackManager.Pop().Bool().Value == true)
                        {
                            instructionIndex = jumpIndexTrue;

                            if (instructionIndex > instructions.Count)
                            {
                                break;
                            }
                            continue;

                        }
                        break;

                    case ByteType.JUMP:
                        int jumpIndex = (int)instruction.Value[0];
                        instructionIndex = jumpIndex;
                        continue;


                    case ByteType.CALL:
                        List<BifyObject> arguments = [];
                        
                        BifyFunction function = (BifyFunction)GetVariable((string)instruction.Value[0]);

                       
                        int expectedArgCount = (int)instruction.Value[1];
                        _callStack.Add(new CallStackFrame((string)instruction.Value[0], line, _modulePath, SourceCode[line - 1]));

                        if (expectedArgCount != function.ExpectedArgCount && function.ExpectedArgCount != -1)
                        {
                            if (arguments.Count < function.ExpectedArgCount)
                            {
                                throw new BifyArgumentError(
                                    $"Function '{function.Name}' called with too few arguments. Expected {function.ExpectedArgCount}, but got {arguments.Count}.",
                                    SourceCode[line - 1],
                                    SourceCode[line - 1],
                                    line
                                );
                            }
                            else
                            {
                                throw new BifyArgumentError(
                                    $"Function '{function.Name}' called with too many arguments. Expected {function.ExpectedArgCount}, but got {arguments.Count}.",
                                    SourceCode[line - 1],
                                    SourceCode[line - 1],
                                    line
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
                    case ByteType.STORE:
                        StoreVariable(_stackManager.Pop(), (string)instruction.Value[0]);
                        break;
                    case ByteType.NEW_ARRAY:
                        int arrayLength = (int)instruction.Value[0];
                        List<BifyObject> array = [];
                        for (int i = 0; i < arrayLength; i++)
                        {
                            array.Add(_stackManager.Pop());
                        }
                        array.Reverse();
                        _stackManager.Push(new BifyArray(array));
                        break;
                    case ByteType.LOAD_ARRAY:
                        BifyInteger index = _stackManager.Pop().Int();
                        BifyObject bifyObject = _stackManager.Pop();
                        GetIndexAndPush(index,bifyObject);
                        break;
                    default:
                        throw new InvalidOperationException($"Unknown instruction type: {instruction.Type}");


                }

                instructionIndex++;
            }

            _stackManager.Print();
            _varManager.Print();
        }


        private void ProcessOperator(ByteInstruction instruction)
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
                    e.CurrentLine = line;
                    e.LineTokensString = SourceCode[line - 1];
                    e.InvalidTokensString = operatorChar;
                    //Console.WriteLine(e.LineTokensString);
                    throw;
                }
            }

        }
        private void StoreVariable(BifyObject varValue,string varName)
        {
            if (_varManager.HasVariable(varName))
            {
                _varManager.Store(varName, varValue);

            }
            else
            {
                throw new BifyUndefinedError(
                    $"Undefined variable - {varName}",
                    SourceCode[line - 1],
                    varName,
                    line
                );
            }

        }
        private void GetIndexAndPush(BifyInteger index,BifyObject bifyObject)
        {
            try
            {
                _stackManager.Push(bifyObject.Index(index));

            }
            catch (BifyError e)
            {
                e.CurrentLine = line;
                e.LineTokensString = SourceCode[line - 1];
                throw;
            }
        }
        private BifyObject GetVariable(string name)
        {
            try
            {
                return _varManager.GetVariable(name);
            }
            catch (KeyNotFoundException)
            {
                throw new BifyUndefinedError(
                    $"Undefined variable - {name}",
                    SourceCode[line - 1],
                    name,
                    line
                );
            }
        }
        private static BifyObject PerformBinaryOperation(BifyObject a, BifyObject b, ByteType operatorType)
        {
            return operatorType switch
            {
                ByteType.ADD => a.Add(b),
                ByteType.SUB => a.Sub(b),
                ByteType.MUL => a.Mul(b),
                ByteType.DIV => a.Div(b),
                ByteType.MOD => a.Mod(b),
                ByteType.POW => a.Pow(b),
                ByteType.FLOORDIV => a.FloorDiv(b),
                ByteType.EQ => a.Eq(b),
                ByteType.LT => a.Lt(b),
                ByteType.GT => a.Gt(b),
                ByteType.LTE => a.Lte(b),
                ByteType.GTE => a.Gte(b),
                ByteType.AND => a.Bool().And(b.Bool()),
                ByteType.OR => a.Bool().Or(b.Bool()),
                ByteType.NEQ => a.Neq(b),
                ByteType.NOT => a.Bool().Not(),
                ByteType.BITAND => a.BitAnd(b),
                ByteType.BITOR => a.BitOr(b),
                ByteType.BITXOR => a.BitXor(b),
                ByteType.BITNOT => a.BitNot(),
                ByteType.LSHIFT => a.LeftShift(b),
                ByteType.RSHIFT => a.RightShift(b),
                _ => throw new NotImplementedException($"{operatorType} is not implemented in PerformBinaryOperation"),
            };
        }

    }
}
