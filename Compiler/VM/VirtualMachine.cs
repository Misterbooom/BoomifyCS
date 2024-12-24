using System;
using System.Collections.Generic;
using System.Linq;
using BoomifyCS.BuiltIn.Function;
using BoomifyCS.Objects;
using BoomifyCS.Exceptions;
using BoomifyCS.Parser;
using BoomifyCS.Ast;

namespace BoomifyCS.Compiler.VM
{
    public class VirtualMachine
    {
        public VarManager varManager = new();
        public StackManager stackManager = new();
        public int startIndex = -1;
        private readonly List<CallStackFrame> _callStack = [];
        private List<ByteInstruction> _instructions;
        private readonly string[] _sourceCode;
        private string _moduleName;
        private string _modulePath;

        public VirtualMachine(string[] sourceCode) => _sourceCode = sourceCode;
        public void LoadInstructions(List<ByteInstruction> instructions)
        {
            _instructions = instructions;
        }

        public void RunWithVarManager(VarManager varManager)
        {
            this.varManager = varManager;
            ProcessInstructions(_instructions);
        }
        public BifyObject GetReturnValue()
        {
            return stackManager.Pop();
        }

        public void Run(List<ByteInstruction> instructions)
        {
            //Console.WriteLine("***START VIRTUAL MACHINE***");
            ProcessInstructions(instructions);
            //Console.WriteLine("***END VIRTUAL MACHINE***");
            //stackManager.Print();
        }

        private void ProcessInstructions(List<ByteInstruction> instructions)
        {
            _instructions = instructions;
            int instructionIndex = startIndex != -1 ? startIndex : 0;
            while (instructionIndex < _instructions.Count)
            {
                var instruction = _instructions[instructionIndex];
                Traceback.Instance.line = instruction.IndexOfInstruction;
                //Console.WriteLine($"Curr instr - {instructionIndex}: {instruction.ToString()}");

                switch (instruction.Type)
                {
                    case ByteType.LOAD_CONST:
                        stackManager.Push((BifyObject)instruction.Value[0]);
                        break;

                    case ByteType.LOAD:
                        ProcessLoad(instruction);
                        break;

                    case var _ when ByteCodeConfig.BinaryOperators.ContainsValue(instruction.Type):
                        ProcessOperator(instruction);
                        break;

                    case var _ when ByteCodeConfig.AssignmentOperators.ContainsValue(instruction.Type):
                        ProcessAssignment(instruction);
                        break;

                    case ByteType.DEFINE:
                        ProcessDefine(instruction);
                        break;

                    case ByteType.JUMP_IF_FALSE:
                    case ByteType.JUMP_IF_TRUE:
                    case ByteType.JUMP:
                        ProcessJump(instruction, ref instructionIndex);
                        continue;

                    case ByteType.CALL:
                        ProcessCall(instruction, ref instructionIndex);
                        break;

                    case ByteType.MODULE:
                        _moduleName = (string)instruction.Value[0];
                        _modulePath = (string)instruction.Value[1];
                        break;

                    case ByteType.STORE:
                        StoreVariable(stackManager.Pop(), (string)instruction.Value[0]);
                        break;

                    case ByteType.NEW_ARRAY:
                        ProcessNewArray(instruction);
                        break;

                    case ByteType.LOAD_ARRAY:
                        ProcessLoadArray();
                        break;

                    case ByteType.DEF_FUNC:
                        ProccesFunctionDefinition(instruction, ref instructionIndex);
                        break;
                    case ByteType.INIT:
                        ProccesInit(instruction);
                        break;
                    case ByteType.RETURN:
                        ProccesReturn(instruction);
                        return;
                    default:
                        throw new InvalidOperationException($"Unknown instruction type: {instruction.Type}");

                }

                instructionIndex++;
            }


        }
        private void ProccesReturn(ByteInstruction instruction)
        {
            BifyObject obj = stackManager.Pop();
            stackManager.Push(obj);
        }
        private void ProccesInit(ByteInstruction instruction)
        {
            BifyObject obj = stackManager.Pop();

            int numArgs = obj.GetInitializerArgs();

            List<BifyObject> arguments = new List<BifyObject>();

            for (int i = 0; i < numArgs; i++)
            {
                if (stackManager.Count() > 0)
                {
                    arguments.Add(stackManager.Pop());
                }
                else
                {
                    throw new InvalidOperationException("Not enough arguments in the stack for object initialization.");
                }
            }
            arguments.Reverse();
            obj.Initialize(arguments);
            stackManager.Push(obj);
        }


        private void ProccesFunctionDefinition(ByteInstruction instruction, ref int instructionIndex)
        {
            string name = (string)instruction.Value[0];
            BifyOwnFunction function = new(name);
            ProccesFunctionBody(function, ref instructionIndex);
            stackManager.Push(function);
            varManager.DefineVariable(name, function);




        }
        private void ProccesFunctionBody(BifyOwnFunction function, ref int instructionIndex)
        {
            int stack = 0;
            var currentInstruction = _instructions[0];
            for (int i = instructionIndex; i < _instructions.Count; i++)
            {
                currentInstruction = _instructions[instructionIndex];
                if (currentInstruction.Type == ByteType.DEF_FUNC)
                {
                    stack++;
                }
                else if (currentInstruction.Type == ByteType.END_DEF_FUNC)
                {
                    stack--;
                    if (stack <= 0)
                    {
                        instructionIndex = i;
                        break;
                    }
                }
                else if (currentInstruction.Type == ByteType.ADD_ARG)
                {
                    function.AddArgument((string)currentInstruction.Value[0]);
                }
                else
                {
                    if (function.index == -1)
                    {
                        function.index = instructionIndex;
                    }
                    function.AddInstruction(currentInstruction);
                }

                instructionIndex++;
            }

        }
        private void ProcessOperator(ByteInstruction instruction)
        {

            if (instruction.Type == ByteType.NOT)
            {
                BifyObject bifyObject = stackManager.Pop();
                stackManager.Push(new BifyBoolean(!bifyObject.Bool().Value));
                return;
            }
            BifyObject right = stackManager.Pop();
            BifyObject left = stackManager.Pop();
            try
            {
                BifyObject result = PerformBinaryOperation(left, right, instruction.Type);
                stackManager.Push(result);
            }
            catch (OverflowException)
            {
                Traceback.Instance.ThrowException(new BifyOverflowError("Overflow error", right.ToString() + left.ToString()));
            }



        }

        private void ProcessLoad(ByteInstruction instruction)
        {
            var varName = (string)instruction.Value[0];
            try
            {
                var value = varManager.GetVariable(varName);
                stackManager.Push(value);
            }
            catch (KeyNotFoundException)
            {
                Traceback.Instance.ThrowException(new BifyUndefinedError(
                    $"Undefined variable - {varName}",
                    "",
                    varName
                ));
            }
        }

        private void ProcessAssignment(ByteInstruction instruction)
        {
            var identifier = (string)instruction.Value[0];
            var varValue = varManager.GetVariable(identifier);

            ByteType byteType = instruction.Type switch
            {
                ByteType.ADDE => ByteType.ADD,
                ByteType.SUBE => ByteType.SUB,
                ByteType.MULE => ByteType.MUL,
                ByteType.DIVE => ByteType.DIV,
                ByteType.FLOORDIV => ByteType.FLOORDIV,
                ByteType.POWE => ByteType.POW,
                _ => throw new NotImplementedException($"Bytecode - {instruction.Type} not implemented in unary op"),
            };

            instruction.Type = byteType;
            ProcessOperator(instruction);
        }

        private void ProcessDefine(ByteInstruction instruction)
        {
            if (stackManager.Count() == 0)
            {
                Traceback.Instance.ThrowException(new BifyInitializationError(
                    "Variable initialized incorrectly. Make sure to assign a value when declaring the variable.",
                    "",
                    Traceback.Instance.source[Traceback.Instance.line - 1]

                ));
            }

            var value = stackManager.Pop();
            var bifyVar = (string)instruction.Value[0];
            varManager.DefineVariable(bifyVar, value);

        }

        private void ProcessJump(ByteInstruction instruction, ref int instructionIndex)
        {
            var jumpIndex = (int)instruction.Value[0];

            if (instruction.Type == ByteType.JUMP_IF_FALSE && !stackManager.Pop().Bool().Value ||
                instruction.Type == ByteType.JUMP_IF_TRUE && stackManager.Pop().Bool().Value ||
                instruction.Type == ByteType.JUMP)
            {
                instructionIndex = jumpIndex;

            }
            else
            {
                instructionIndex++;

            }
        }

        void ProcessCall(ByteInstruction instruction, ref int instructionsIndex)
        {
            var arguments = new List<BifyObject>();
            var function = (BifyFunction)stackManager.Pop();
            var expectedArgCount = (int)instruction.Value[0];

            _callStack.Add(new CallStackFrame(function.Name, Traceback.Instance.line, _modulePath, Traceback.Instance.source[Traceback.Instance.line - 1]));

            if (expectedArgCount != function.ExpectedArgCount && function.ExpectedArgCount != -1)
            {
                Traceback.Instance.ThrowException(new BifyArgumentError(
                    $"Function '{function.Name}' called with wrong number of arguments. Expected {function.ExpectedArgCount}, but got {expectedArgCount}.",
                    "",
                    Traceback.Instance.source[Traceback.Instance.line - 1]
                ));
            }
            for (int i = 0; i < expectedArgCount; i++)
            {
                arguments.Add(stackManager.Pop());
            }

            arguments.Reverse();

            if (function is BifyOwnFunction bifyOwnFunction)
            {
                int tempIndex = instructionsIndex;
                string context = varManager.GetContext();

                varManager.SetContext("locals");

                bifyOwnFunction.SetInstructions(_instructions);

                bifyOwnFunction.Call(arguments, this);

                var functionReturn = stackManager.Pop();

                _callStack.Pop();

                varManager.PopScope();
                stackManager.PopScope();
                stackManager.Push(functionReturn);

                varManager.SetContext(context);

                instructionsIndex = tempIndex;

                return;
            }

            stackManager.Push(function.Call(arguments));
            _callStack.Pop();
        }

        private void ProcessNewArray(ByteInstruction instruction)
        {
            var arrayLength = (int)instruction.Value[0];
            var array = new List<BifyObject>();

            for (int i = 0; i < arrayLength; i++)
            {
                array.Add(stackManager.Pop());
            }
            array.Reverse();
            stackManager.Push(new BifyArray(array));
        }

        private void ProcessLoadArray()
        {
            var index = stackManager.Pop();
            var bifyObject = stackManager.Pop();
            stackManager.Push(bifyObject.Index(index));
        }

        private void StoreVariable(BifyObject varValue, string varName)
        {
            if (varManager.HasVariable(varName))
            {
                varManager.Store(varName, varValue);
            }
            else
            {
                Traceback.Instance.ThrowException(new BifyUndefinedError(
                    $"Undefined variable - {varName}",
                    "",
                    varName
                ));
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
