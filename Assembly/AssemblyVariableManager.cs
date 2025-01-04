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
using NUnit.Framework.Internal.Execution;

namespace BoomifyCS.Assembly
{
    class Variable
    {
        public string Name;
        public Type Type; // Type is used to store type information
        public LLVMValueRef Value;
        public int Size => (int)Type.GetProperty("Size").GetValue(null);
        public LLVMTypeRef LlvmType => (LLVMTypeRef)Type.GetProperty("LLVMType").GetValue(null);
        public Variable(string name, Type type, int offset)
        {
            Name = name;
            Type = type;
        }

        public Variable(string name, Type type)
        {
            Name = name;
            Type = type;
        }
        public Variable(string name, Type type, BifyFunction bifyFunction)
        {
            Name = name;
            Type = type;
        }

        public override string ToString()
        {
            return $"Name: {Name}, Type: {Type}, Value: {Value}";
        }
    }


    class AssemblyVariableManager
    {
        private Dictionary<string, Variable> table = new Dictionary<string, Variable>();

        private Dictionary<string, Variable> localTable = new Dictionary<string, Variable>();
        public AssemblyVariableManager()
        {
            table["int"] = new Variable("int", typeof(BifyInteger), 0);
            table["void"] = new Variable("void", typeof(BifyVoid));
        }
        public void IsExists(string name)
        {
            if (!localTable.Concat(table).ToDictionary().ContainsKey(name))
            {
                Traceback.Instance.ThrowException(new BifyUndefinedError($"Undefined variable - {name}", "", name));
            }

        }
        public Variable GetVariable(string name)
        {
            IsExists(name);
            return localTable.Concat(table).ToDictionary()[name];
        }
        public LLVMValueRef GetLocalValue(string name)
        {
            if (localTable[name].Value == null)
            {
                Traceback.Instance.ThrowException(new BifyUnknownError("Compiler Side: Get: Incorrect local value is null"));
            } 
            return localTable[name].Value;
        }
        public void SetLocalValue(string name,LLVMValueRef valueRef) {
            if (valueRef == null)
            {
                Traceback.Instance.ThrowException(new BifyUnknownError("Compiler Side: Set: Incorrect local value is null"));
            }
            localTable[name].Value = valueRef;
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
                table[name] = new Variable(name, typeof(BifyFunction), new BifyFunction(name, table[type].Type));
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
