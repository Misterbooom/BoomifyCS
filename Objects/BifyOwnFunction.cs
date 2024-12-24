using BoomifyCS.Compiler.VM;
using BoomifyCS.Compiler;
using BoomifyCS.Objects;
using System.Collections.Generic;

public class BifyOwnFunction : BifyFunction
{
    private List<ByteInstruction> _instructions = [];
    private readonly List<string> _arguments = [];
    public int index = -1;
    public BifyOwnFunction(string name) : base(name)
    {
        ExpectedArgCount = 0;
    }

    public void AddInstruction(ByteInstruction instruction)
    {
        _instructions.Add(instruction);
    }

    public void SetInstructions(List<ByteInstruction> byteInstruction)
    {
        _instructions = byteInstruction;
    }
    public void AddArgument(string argumentName)
    {
        _arguments.Add(argumentName);
        ExpectedArgCount++;
    }

    public List<ByteInstruction> GetInstructions() => _instructions;

    public void Call(List<BifyObject> arguments, VirtualMachine vm)
    {
        vm.varManager.PushScope();
        vm.stackManager.PushScope();

        for (int i = 0; i < arguments.Count; i++)
        {
            vm.varManager.DefineVariable(_arguments[i], arguments[i]);
        }
        vm.startIndex = index;
        vm.Run(_instructions);
        vm.startIndex = -1;

    }
}
