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
        private readonly Dictionary<string, IValue> globalVariables = new()
        {
            {"int", new IntegerType()},
            {"float", new FloatType()},
            {"void",new VoidType()},
            {"explode", new Explode() },
            {"char", new CharType()},
            {"bool", new BoolType()},
            {"false", new BoolType().Create(0)},
            {"true", new BoolType().Create(1)},
            {"input",new Input() },

        };

        private readonly Stack<Dictionary<string, IValue>> localScopes = new();
        public AssemblyVariableManager()
        {

        }

        public void EnterLocalScope()
        {
            localScopes.Push(new Dictionary<string, IValue>());
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

            IValue value = GetVariable(type);
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
        public void SetLocalVariable(string name, IValue value)
        {
            if (localScopes.Count == 0)
                throw new InvalidOperationException("Local scope not created. Call EnterLocalScope before setting local variables.");
            if (!localScopes.Peek().ContainsKey(name))
            {
                Traceback.Instance.ThrowException(new BifyNameError(ErrorMessage.UndefindedVariable(name)));
                return;
            }
            localScopes.Peek()[name] = value;
        }
        public BifyType GetBifyType(string type)
        {

            IValue value = GetVariable(type);
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

        public void RegisterLocalVariable(string name, IValue variable)
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

        public void RegisterGlobalVariable(string name, IValue variable)
        {
            if (globalVariables.ContainsKey(name))
            {
                Traceback.Instance.ThrowException(new BifyNameError($"Variable redefinded: '{name}'"));
                return;
            }
            globalVariables[name] = variable;
        }

        public IValue? GetVariable(string name)
        {
            foreach (var scope in localScopes)
                if (scope.TryGetValue(name, out IValue variable))
                    return variable;
            if (globalVariables.TryGetValue(name, out IValue globalVar))
                return globalVar;

            Traceback.Instance.ThrowException(new BifyUndefinedError(ErrorMessage.UndefindedVariable(name)));
            return null;

        }
        public BifyValue? GetBifyValue(string name)
        {

            return (BifyValue)GetVariable(name);

        }

        public void ClearGlobalVariables() => globalVariables.Clear();

        public void ClearLocalScopes() => localScopes.Clear();
    }
}
