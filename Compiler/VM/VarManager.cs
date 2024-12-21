﻿using BoomifyCS.BuiltIn.Function;
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

            }
        }
    };

    private string _context = "globals";

    public VarManager() { }

    // Set the context for the variable manager
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

    // Define a new variable in the current context
    public void DefineVariable(string name, BifyObject value)
    {
        _variables[_context][name] = value;
    }

    // Store a value for an existing variable in the current context
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
    public bool HasVariable(string name)
    {
        return _variables[_context].ContainsKey(name);
    }

    // Retrieve a variable from the current context or globals
    public BifyObject GetVariable(string name)
    {
        if (_variables[_context].TryGetValue(name, out BifyObject value))
        {
            return value;
        }
        else if (_context == "locals" && _variables["globals"].TryGetValue(name, out value))
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
        if (_variables.TryGetValue(_context, out Dictionary<string, BifyObject> variablesToPrint))
        {
            Console.WriteLine($"Contents of the {_context} context:");

            foreach (var kvp in variablesToPrint)
            {
                Console.WriteLine($"{kvp.Key}: {kvp.Value.Repr()}");
            }
        }
        else
        {
            Console.WriteLine($"Context '{_context}' does not exist.");
        }
    }

    public Dictionary<string, BifyObject> CloneCurrentContext()
    {
        return new Dictionary<string, BifyObject>(_variables[_context]);
    }
}
