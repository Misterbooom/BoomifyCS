using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using BoomifyCS.Exceptions;
using BoomifyCS.Objects;
using LLVMSharp.Interop;
using NUnit.Framework;

namespace BoomifyCS.Assembly
{
    class Variable
    {
        public string Name { get; set; }
        public Type Type { get; set; } // Type is used to store type information
        public string Value { get; set; }
        public int Size => (int)Type.GetProperty("Size").GetValue(null);
        public LLVMTypeRef LlvmType => (LLVMTypeRef)Type.GetProperty("LLVMType").GetValue(null);
        public Variable(string name, Type type, int offset)
        {
            Name = name;
            Type = type;
            Value = null; // Default value
        }

        public Variable(string name, Type type)
        {
            Name = name;
            Type = type;
            Value = null; // Default value
        }
        public Variable(string name, Type type,BifyFunction bifyFunction)
        {
            Name = name;
            Type = type;
            Value = null; // Default value
        }

        public override string ToString()
        {
            return $"Name: {Name}, Type: {Type}, Value: {Value}";
        }
    }


    class AssemblyVariableManager
    {
        Dictionary<string, Variable> table = new Dictionary<string, Variable>();

        Dictionary<string, Variable> localTable = new Dictionary<string, Variable>();
        public AssemblyVariableManager()
        {
            table["int"] = new Variable("int", typeof(BifyInteger), 0);
            table["void"] = new Variable("void", typeof(BifyVoid));
        }

        public LLVMTypeRef AllocateLocal(string name, string type)
        {
            if (table.ContainsKey(type))
            {
                Type typeT = table[type].Type;
                localTable[name] = new Variable(name, typeT);

                return GetType(type);

            }
            else
            {
                Traceback.Instance.ThrowException(new BifyNameError($"Type '{type}' doesn't exist.", "", type));
                return null;
            }
        }

        public LLVMTypeRef AllocateFunction(string name, string type)
        {
            if (table.ContainsKey(type))
            {
                LLVMTypeRef lLVMType = GetType(type);
                table[name] = new Variable(name, typeof(BifyFunction),new BifyFunction(name, table[type].Type));
                return lLVMType;

            }
            else
            {
                Traceback.Instance.ThrowException(new BifyNameError($"Type '{type}' doesn't exist.", "", type));
                return null;
            }
        }
        private LLVMTypeRef GetType(string type)
        {
            BifyDebug.Log($"Type - {type}");
            LLVMTypeRef llvmType = table[type].LlvmType;
            if (llvmType != null)
            {
                return llvmType;
            }
            Traceback.Instance.ThrowException(new BifyNameError($"Type '{type}' doesn't exist.", "", type));
            return null;
        }
        public void ClearLocals()
        {
            localTable.Clear();
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
