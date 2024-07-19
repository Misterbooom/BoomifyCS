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
                else if (ByteCodeConfig.BinaryOperators.ContainsValue(instruction.Type))
                {
                    _ProcessOperator(instruction);
                }
                else if (instruction.Type == ByteType.STORE) { 
                    var value = _stackManager.Pop();
                    var bifyVar = (BifyVar)instruction.Value[0];
                    _varManager.DefineVariable(bifyVar.Name, value);
                }
                else if (instruction.Type == ByteType.JUMP_IF_FALSE) {
                    int jumpIndex = (int)instruction.Value[0];
                    if (!(_stackManager.Peek() is BifyBoolean bifyBoolean))
                    {
                        throw new BifyTypeError("Condition must be a boolean", SourceCode[instruction.IndexOfInstruction], SourceCode[instruction.IndexOfInstruction],instruction.IndexOfInstruction + 1);
                    }
                    if (bifyBoolean.Value == false )
                    {
                        instructionIndex = jumpIndex;
                    }
                    _stackManager.Pop();
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
                catch (Exception e)
                {
                    ByteCodeConfig.byteToString.TryGetValue(instruction.Type,out string operatorChar);
                    throw new BifyOperationError(e.Message, SourceCode[instruction.IndexOfInstruction - 1],operatorChar,instruction.IndexOfInstruction);
                }
                //Console.WriteLine($"{left} {operatorType} {right} = {result}");
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
                    return a.And(b);
                case ByteType.OR:
                    return a.Or(b);
                case ByteType.NEQ:
                    return a.Neq(b);
                case ByteType.NOT:
                    return a.Not();
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
