using BoomifyCS.BuiltIn.Function;
using BoomifyCS.Objects;
using System.Collections.Generic;
using System;

public class VarManager
{
    private readonly Dictionary<string, Dictionary<string, BifyObject>> _variables = new()
    {
        { "locals", new Dictionary<string, BifyObject>() },
        { "globals", new Dictionary<string, BifyObject>
            {
                { "explode", new Explode() },
                { "ignite", new Ignite() },
                { "parse", new Parse() },
                { "null", new BifyNull()},
                { "false", new BifyBoolean(false) },
                { "true", new BifyBoolean(true) },
                { "len", new Len()}

            }
        }
    };

    private string _context = "globals";
    private Stack<Dictionary<string, BifyObject>> _stack = [];
    public VarManager() { }

    // Set the context for the variable manager
    public string GetContext() => _context;

    public void SetContext(string context)
    {
        if (_variables.ContainsKey(context))
        {
            _context = context;
        }
        else
        {
            throw new ArgumentException("Invalid context. Use 'locals' or 'globals'.");
        }
    }
    // Create a new local scope
    public void PushScope()
    {
        _stack.Push(_variables["locals"]);
        _variables["locals"]  = new();
    }

    public void PopScope() => _variables["locals"] = _stack.Pop();

    public void DefineVariable(string name, BifyObject value) => _variables[_context][name] = value;

    public void Store(string name, BifyObject value)
    {
        if (_variables[_context].ContainsKey(name))
        {
            _variables[_context][name] = value;
        }
        else
        {
            throw new KeyNotFoundException($"Variable '{name}' not found in context '{_context}'.");
        }
    }

    // Check if a variable exists in the current context
    public bool HasVariable(string name) => _variables[_context].ContainsKey(name);

    // Retrieve a variable from the current context or globals
    public BifyObject GetVariable(string name)
    {
        if (_variables["locals"].TryGetValue(name, out BifyObject value))
        {
            return value;
        }
        else if (_variables["globals"].TryGetValue(name, out value))
        {
            return value;
        }
        else
        {
            throw new KeyNotFoundException($"Variable '{name}' not found in context '{_context}'.");
        }
    }

    public void Print()
    {
        Console.WriteLine("--Variables--");
        
            foreach (var kvp in _variables["locals"])
            {
                Console.WriteLine($"{kvp.Key}: {kvp.Value.Repr()}");
            }
            foreach (var kvp in _variables["globals"])
            {
                Console.WriteLine($"{kvp.Key}: {kvp.Value.Repr()}");
            }
        Console.WriteLine("---End Of Variables---");
    }

    public Dictionary<string, BifyObject> CloneCurrentContext() => new Dictionary<string, BifyObject>(_variables[_context]);
}
