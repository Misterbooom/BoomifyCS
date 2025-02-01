using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using BoomifyCS.BuiltIn.Function;
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
        public BifyObject BifyObject;
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
        public Variable(string name, Type type, BifyObject bifyValue)
        {
            Name = name;
            Type = type;
            BifyObject = bifyValue;
        }
       
        public override string ToString()
        {
            return $"{Name}(Type: {Type}, Value: {Value}, BifyObject: {BifyObject})";
        }
        public BifyValue ToBifyValue()
        {
            return new BifyValue(BifyObject);
        }
    }


    class AssemblyVariableManager
    {
        private Dictionary<string, Variable> table = new Dictionary<string, Variable>();

        private Dictionary<string, Variable> localTable = new Dictionary<string, Variable>();

        private AssemblyCompiler compiler;
        public AssemblyVariableManager(AssemblyCompiler compiler)
        {
            this.compiler = compiler;
            table["int"] = new Variable("int", typeof(BifyInteger));
            table["void"] = new Variable("void", typeof(BifyVoid));
            table["explode"] = new Variable("explode", typeof(Explode),new Explode(compiler));
            table["float"] = new Variable("float", typeof(BifyFloat));
        }
        public void IsExists(string name)
        {
            if (!GetCombinedTables().ContainsKey(name))
            {
                Traceback.Instance.ThrowException(new BifyUndefinedError($"Undefined variable - {name}", "", name));
            }

        }
        public Variable GetVariable(string name)
        {
            IsExists(name);
            return GetCombinedTables()[name];
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
        public void SetLocalBifyObject(string name,BifyObject bifyObject)
        {
            localTable[name].BifyObject = bifyObject;
        }
        public Dictionary<string, Variable> GetCombinedTables()
        {
            return localTable.Concat(table).ToDictionary();
        }
        public Variable AllocateLocal(string name, string type)
        {
            if (table.ContainsKey(type))
            {
                Type typeT = table[type].Type;
                localTable[name] = new Variable(name, typeT);

                return GetVariable(type);

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
