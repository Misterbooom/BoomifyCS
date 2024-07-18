using System;
using System.Collections.Generic;
using BoomifyCS.Objects;

namespace BoomifyCS.Interpreter.VM
{
    public class VirtualMachine
    {
        private VarManager _varManager = new VarManager();
        private StackManager _stackManager = new StackManager();
        private List<ByteInstruction> Instructions;

        public VirtualMachine() { }

        public void Run(List<ByteInstruction> instructions)
        {
            Instructions = instructions;
            foreach (ByteInstruction instruction in instructions)
            {
                if (instruction.Type == ByteType.LOAD_CONST)
                {
                    var constant = instruction.Value[0];
                    _stackManager.Push(constant);
                }
                else if (ByteCodeConfig.BinaryOperators.ContainsValue(instruction.Type))
                {
                    _ProcessOperator(instruction.Type);
                }
                else if (instruction.Type == ByteType.STORE) { 
                    var value = _stackManager.Pop();
                    var bifyVar = (BifyVar)instruction.Value[0];
                    _varManager.DefineVariable(bifyVar.Name, value);
                }

            }
            _stackManager.Print(); 
            _varManager.Print();

        }

        private void _ProcessOperator(ByteType operatorType)
        {
            if (ByteCodeConfig.BinaryOperators.ContainsValue(operatorType))
            {
                BifyObject right = _stackManager.Pop();
                BifyObject left = _stackManager.Pop();
                BifyObject result = PerformBinaryOperation(left, right,operatorType);
                //Console.WriteLine($"{left} {operatorType} {right} = {result}");
                _stackManager.Push(result);
            }

        }

        private BifyObject PerformBinaryOperation(BifyObject a, BifyObject b, ByteType operatorType)
        {
            if (operatorType == ByteType.ADD)
            {
                return a.Add(b);
            }
            else if (operatorType == ByteType.SUB)
            {
                return a.Sub(b);
            }
            else if (operatorType == ByteType.MUL)
            {
                return a.Mul(b);
            }
            else if (operatorType == ByteType.DIV)
            {
                return a.Div(b);
            }
            else if (operatorType == ByteType.MOD)
            {
                return a.Mod(b);
            }
            else if (operatorType == ByteType.FLOORDIV)
            {
                return a.FloorDiv(b);
            }

            throw new NotImplementedException($"{operatorType} is not Implemented in PerfomBinaryOperation");
        }
    }
}
