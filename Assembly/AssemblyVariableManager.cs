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
        private readonly Dictionary<string, IValue> _globalVariables = new()
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
            {"sizeof",new SizeOf() },
            {"exit",new Exit() },
            {"null", new NullValue()},
            {"malloc",StdC.DeclarFunction("malloc",[new IntegerType()],new AnyType()) },
            {"calloc",StdC.DeclarFunction("calloc",[new IntegerType(), new IntegerType()],new AnyType()) },
            {"realloc",StdC.DeclarFunction("realloc",[new AnyType(), new IntegerType()],new AnyType()) },
            {"free",StdC.DeclarFunction("free",[new AnyType()],new VoidType()) },
            { "any", new AnyType()},
            { "__log",new Log()},
        };

        private readonly Stack<Dictionary<string, IValue>> _localScopes = new();
        public AssemblyVariableManager()
        {

        }
        public Dictionary<string, IValue> GetLocals()
        {
            return _localScopes.Peek();
        }
        public void EnterLocalScope()
        {
            _localScopes.Push(new Dictionary<string, IValue>());
        }
        public void SetCurrentLocalScope(Dictionary<string,IValue> scope)
        {
            _localScopes.Pop();
            _localScopes.Push(new Dictionary<string, IValue>(scope));
        }



        public void ExitLocalScope()
        {
            if (_localScopes.Count > 0)
                _localScopes.Pop();
            else
                throw new InvalidOperationException("No local scope to exit.");
        }
        public LLVMTypeRef GetLlvmType(string type)
        {

            IValue value = GetVariable(type);
            if (value is BifyType bifyType)
            {
                return bifyType.LlvmType;
            }
            else
            {
                Traceback.Instance.ThrowException(new BifyTypeError($"{type} cannot be used as type."));
                return null;
            }
        }
        public void SetLocalVariable(string name, IValue value)
        {
            if (_localScopes.Count == 0)
                throw new InvalidOperationException("Local scope not created. Call EnterLocalScope before setting local variables.");
            if (!_localScopes.Peek().ContainsKey(name))
            {
                Traceback.Instance.ThrowException(new BifyNameError(ErrorMessage.UndefindedVariable(name)));
                return;
            }
            _localScopes.Peek()[name] = value;
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
        public BifyType TryGetBifyType(string type)
        {
            Traceback.Instance.Catch(typeof(BifyUndefinedError));
            IValue value = GetVariable(type);
            Traceback.Instance.TrackPop();

            if (value is BifyType bifyType)
            {
                return bifyType;
            }
            return null;
        }

        public void RegisterLocalVariable(string name, IValue variable)
        {
            if (_localScopes.Count == 0)
                throw new InvalidOperationException("Local scope not created. Call EnterLocalScope before registering local variables.");
            if (_localScopes.Peek().ContainsKey(name))
            {
                Traceback.Instance.ThrowException(new BifyNameError($"Variable redefinded: '{name}'"));
                return;
            }
            _localScopes.Peek()[name] = variable;
        }

        public void RegisterGlobalVariable(string name, IValue variable)
        {
            if (_globalVariables.ContainsKey(name))
            {
                Traceback.Instance.ThrowException(new BifyNameError($"Variable redefinded: '{name}'"));
                return;
            }
            _globalVariables[name] = variable;
        }

        public IValue? GetVariable(string name)
        {
            foreach (var scope in _localScopes)
                if (scope.TryGetValue(name, out IValue variable))
                    return variable;
            if (_globalVariables.TryGetValue(name, out IValue globalVar))
                return globalVar;

            Traceback.Instance.ThrowException(new BifyUndefinedError(ErrorMessage.UndefindedVariable(name)));
            return null;

        }
        public BifyValue? GetBifyValue(string name)
        {

            return (BifyValue)GetVariable(name);

        }

        public void ClearGlobalVariables() => _globalVariables.Clear();

        public void ClearLocalScopes() => _localScopes.Clear();
        public override string ToString()
        {
            var globalVars = string.Join(", ", _globalVariables.Keys);
            _localScopes.TryPeek(out var localScope);
            var localVars = string.Join(", ", localScope == null ? "" : localScope.Keys);
            return $"Global Variables: [{globalVars}], Local Variables: [{localVars}]";
        }
    }
}
