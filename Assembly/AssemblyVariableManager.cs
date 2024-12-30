using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BoomifyCS.Exceptions;
using BoomifyCS.Objects;
using NUnit.Framework;

namespace BoomifyCS.Assembly
{
    class Variable
    {
        public string Name { get; set; }
        public Type Type { get; set; } // Type is used to store type information
        public string Value { get; set; }
        public int Offset { get; set; }
        public int Size => (int)Type.GetProperty("Size").GetValue(null);

        public Variable(string name, Type type, int offset)
        {
            Name = name;
            Type = type;
            Offset = offset;
            Value = null; // Default value
        }

        public Variable(string name, Type type)
        {
            Name = name;
            Type = type;
            Value = null; // Default value
            Offset = 0; // Default offset
        }

        public override string ToString()
        {
            return $"Name: {Name}, Type: {Type}, Value: {Value}, Offset: {Offset}";
        }
    }


    class AssemblyVariableManager
    {
        Dictionary<string, Variable> table = new Dictionary<string, Variable>();

        Dictionary<string, Variable> localTable = new Dictionary<string, Variable>();
        private int offset = 0;
        public AssemblyVariableManager()
        {
            table["int"] = new Variable("int", typeof(BifyInteger), 0);
        }

        public int AllocateLocal(string name, string type)
        {
            if (table.ContainsKey(type))
            {
                localTable[name] = new Variable(name, table[type].Type, table[type].Offset);
                offset += (int)table[type].Size;
                localTable[name].Offset = offset;
                return offset;
            }
            else
            {
                Traceback.Instance.ThrowException(new BifyNameError($"Type '{type}' doesn't exist.","",type));
                return 0;
            }
        }
        public void ClearLocals()
        {
            localTable.Clear();
            offset = 0;
        }
        public Variable GetVariable(string name)
        {
            if (table.ContainsKey(name))
            {
                return table[name];
            }
            else
            {
                throw new KeyNotFoundException("Variable not found.");
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var entry in table)
            {
                sb.AppendLine(entry.Value.ToString());
            }
            return sb.ToString();
        }
    }
}
