using BoomifyCS.Compiler.VM;
using BoomifyCS.Compiler;
using BoomifyCS.Objects;
using System.Collections.Generic;

public class BifyOwnFunction : BifyFunction
{
    private readonly List<ByteInstruction> _instructions = [];
    private readonly List<string> _arguments = [];

    public BifyOwnFunction(string name) : base(name)
    {
    }

    public void AddInstruction(ByteInstruction instruction)
    {
        _instructions.Add(instruction);
    }

    public void AddArgument(string argumentName)
    {
        _arguments.Add(argumentName);
    }

    public List<ByteInstruction> GetInstructions() => _instructions;

    public void Call(List<BifyObject> arguments, VirtualMachine vm)
    {
        // Save the current variable context
        var previousContext = vm.varManager.CloneCurrentContext();

        // Set the context to 'locals' for the function execution
        vm.varManager.SetContext("locals");

        // Define function arguments as local variables
        for (int i = 0; i < arguments.Count; i++)
        {
            vm.varManager.DefineVariable(_arguments[i], arguments[i]);
        }

        // Execute the function's instructions
        vm.Run(_instructions);

        // Restore the previous variable context
        vm.varManager.SetContext("globals");
        foreach (var kvp in previousContext)
        {
            vm.varManager.DefineVariable(kvp.Key, kvp.Value);
        }
    }
}
