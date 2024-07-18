﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomifyCS.Objects;
namespace BoomifyCS.Interpreter.VM
{
    public class VarManager
    {
        private Dictionary<string, Dictionary<string, BifyObject>> variables = new Dictionary<string, Dictionary<string, BifyObject>>
    {
        { "locals", new Dictionary<string, BifyObject>() },
        { "globals", new Dictionary<string, BifyObject>() }
    };

        private string context = "globals";

        public VarManager() { }

        public void SetContext(string context)
        {
            if (variables.ContainsKey(context))
            {
                this.context = context;
            }
            else
            {
                throw new ArgumentException("Invalid context. Use 'locals' or 'globals'.");
            }
        }

        public void DefineVariable(string name, BifyObject value)
        {
            variables[context][name] = value;
        }

        public BifyObject GetVariable(string name)
        {
            if (variables[context].TryGetValue(name, out BifyObject value))
            {
                return value;
            }
            else if (context == "locals" && variables["globals"].TryGetValue(name, out value))
            {
                return value;
            }
            else
            {
                throw new KeyNotFoundException($"Variable '{name}' not found in context '{context}'.");
            }
        }
        public void Print()
        {
            if (!variables.ContainsKey(context))
            {
                Console.WriteLine($"Context '{context}' does not exist.");
                return;
            }

            var variablesToPrint = variables[context];

            Console.WriteLine($"Contents of the {context} context:");

            foreach (var kvp in variablesToPrint)
            {
                Console.WriteLine($"{kvp.Key}: {kvp.Value}");
            }
        }

    }
}
