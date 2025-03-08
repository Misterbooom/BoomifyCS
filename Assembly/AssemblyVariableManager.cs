using System;
using System.Collections.Generic;
using BoomifyCS.Assembly.BifyObject;
using BoomifyCS.Assembly.Builtin;
using BoomifyCS.Exceptions;
using LLVMSharp.Interop;
namespace BoomifyCS.Assembly
{

    public class AssemblyVariableManager
    {
        private readonly Dictionary<string, BifyValue> globalVariables = new()
        {
            {"int", new IntegerType()},
            {"float", new FloatType()},
            {"void",new VoidType()},
            {"explode", new Explode() },
            {"string", new StringType()}

        };

        private readonly Stack<Dictionary<string, BifyValue>> localScopes = new();
        public AssemblyVariableManager()
        {

        }

        public void EnterLocalScope()
        {
            localScopes.Push(new Dictionary<string, BifyValue>());
        }

        public void ExitLocalScope()
        {
            if (localScopes.Count > 0)
                localScopes.Pop();
            else
                throw new InvalidOperationException("No local scope to exit.");
        }
        public LLVMTypeRef GetLLVMType(string type)
        {

            BifyValue value = GetVariable(type);
            if (value is BifyType bifyType)
            {
                return bifyType.LLVMType;
            }
            else
            {
                Traceback.Instance.ThrowException(new BifyTypeError($"{type} cannot be used as type."));
                return null;
            }
        }
        public BifyType GetBifyType(string type)
        {

            BifyValue value = GetVariable(type);
            if (value is BifyType bifyType)
            {
                return bifyType;
            }
            else
            {
                Traceback.Instance.ThrowException(new BifyTypeError($"{type} cannot be used as type."));
                return null;
            }
        }

        public void RegisterLocalVariable(string name, BifyValue variable)
        {
            if (localScopes.Count == 0)
                throw new InvalidOperationException("Local scope not created. Call EnterLocalScope before registering local variables.");
            if (localScopes.Peek().ContainsKey(name))
            {
                Traceback.Instance.ThrowException(new BifyNameError($"Variable redefinded: '{name}'"));
                return;
            }
            localScopes.Peek()[name] = variable;
        }

        public void RegisterGlobalVariable(string name, BifyValue variable)
        {
            if (globalVariables.ContainsKey(name))
            {
                Traceback.Instance.ThrowException(new BifyNameError($"Variable redefinded: '{name}'"));
                return;
            }
            globalVariables[name] = variable;
        }

        public BifyValue? GetVariable(string name)
        {
            foreach (var scope in localScopes)
                if (scope.TryGetValue(name, out BifyValue variable))
                    return variable;
            if (globalVariables.TryGetValue(name, out BifyValue globalVar))
                return globalVar;

            Traceback.Instance.ThrowException(new BifyUndefinedError(ErrorMessage.UndefindedVariable(name)));
            return null;

        }

        public void ClearGlobalVariables() => globalVariables.Clear();

        public void ClearLocalScopes() => localScopes.Clear();
    }
}
