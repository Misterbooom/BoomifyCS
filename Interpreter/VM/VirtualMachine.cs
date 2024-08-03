using System;
using System.Collections.Generic;
using System.Linq;
using BoomifyCS.BuiltIn.Function;
using BoomifyCS.Objects;

namespace BoomifyCS.Interpreter.VM
{
    public class VirtualMachine
    {
        private VarManager _varManager = new VarManager();
        private StackManager _stackManager = new StackManager();
        private List<ByteInstruction> Instructions;
        private string[] SourceCode;

        public VirtualMachine(string[] sourceCode ) {
            this.SourceCode = sourceCode;
        }

        public void Run(List<ByteInstruction> instructions)
        {
            Instructions = instructions;
            int instructionIndex = 0;
            while (instructionIndex < Instructions.Count)
            {
                ByteInstruction instruction = Instructions[instructionIndex];
                if (instruction.Type == ByteType.LOAD_CONST)
                {
                    var constant = instruction.Value[0];
                    _stackManager.Push((BifyObject)constant);
                }
                else if (instruction.Type == ByteType.LOAD)
                {
                    var varName = (string)instruction.Value[0];
                    try
                    {
                        BifyObject varValue = _varManager.GetVariable(varName);
                        _stackManager.Push(varValue);

                    }
                    catch (KeyNotFoundException )
                    {
                        throw new BifyUndefinedError($"Undefined variable - {varName}", SourceCode[instruction.IndexOfInstruction - 1],varName,instruction.IndexOfInstruction);
                    }
                }
                else if (ByteCodeConfig.BinaryOperators.ContainsValue(instruction.Type))
                {
                    _ProcessOperator(instruction);
                }
                else if (instruction.Type == ByteType.STORE) { 
                    if (_stackManager.Count() == 0)
                    {
                        throw new BifyInitializationError("Variable initialized incorrectly. Make sure to assign a value when declaring the variable.",
                            SourceCode[instruction.IndexOfInstruction - 1],
                            SourceCode[instruction.IndexOfInstruction - 1],
                            instruction.IndexOfInstruction);
                    }

                    var value = _stackManager.Pop();
                    var bifyVar = (BifyVar)instruction.Value[0];
                    _varManager.DefineVariable(bifyVar.Name, value);
                }
                else if (instruction.Type == ByteType.JUMP_IF_FALSE) {
                    int jumpIndex = (int)instruction.Value[0];
                    
                    if (_stackManager.Peek().Bool().Value == false )
                    {
                        instructionIndex = jumpIndex - 1;
                    }
                    _stackManager.Pop();

                }
                else if (instruction.Type == ByteType.JUMP_IF_TRUE)
                {
                    int jumpIndex = (int)instruction.Value[0];
                    
                    if (_stackManager.Peek().Bool().Value == true)
                    {
                        instructionIndex = jumpIndex;
                    }
                    _stackManager.Pop();
                }
                else if (instruction.Type == ByteType.JUMP)
                {
                    int jumpIndex = (int)instruction.Value[0];
                    instructionIndex = jumpIndex;

                }
                else if (instruction.Type== ByteType.CALL)
                {
                    List<BifyObject> arguments = new List<BifyObject>();
                   
                    BifyFunction function = (BifyFunction)_varManager.GetVariable((string)instruction.Value[0]);
                    int expectedArgCount = (int)instruction.Value[1];
                    if (expectedArgCount != function.ExpectedArgCount && function.ExpectedArgCount != -1) 
                    {
                        if (arguments.Count < function.ExpectedArgCount)
                        {
                            throw new BifyArgumentError($"Function '{function.Name}' called with too few arguments. Expected {function.ExpectedArgCount}, but got {arguments.Count}.",
                                SourceCode[instruction.IndexOfInstruction - 1],
                                SourceCode[instruction.IndexOfInstruction - 1],
                                instruction.IndexOfInstruction

                                );
                        }
                        else 
                        {
                            throw new BifyArgumentError($"Function '{function.Name}' called with too many arguments. Expected {function.ExpectedArgCount}, but got {arguments.Count}.",
                                SourceCode[instruction.IndexOfInstruction - 1],
                                SourceCode[instruction.IndexOfInstruction - 1],
                                instruction.IndexOfInstruction

                                );
                        }

                    }
                    for (int i = 0; i < expectedArgCount;i++)
                    {
                        arguments.Add(_stackManager.Pop());
                    }
                    arguments.Reverse();
                    BifyObject functionReturn = function.Call(arguments);
                    _stackManager.Push(functionReturn);
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
                catch (BifyException e)
                {
                    ByteCodeConfig.byteToString.TryGetValue(instruction.Type,out string operatorChar);
                    e.CurrentLine = instruction.IndexOfInstruction;
                    e.LineTokensString = SourceCode[instruction.IndexOfInstruction - 1];
                    e.InvalidTokensString = operatorChar;
                    Console.WriteLine(e.LineTokensString);
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
