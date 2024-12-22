using BoomifyCS.Lexer;
using System;
using System.Collections.Generic;
using BoomifyCS.Exceptions;

namespace BoomifyCS.Objects
{
    public class BifyObject()
    {
        public int ExpectedArgCount;
        public string GetName()
        {
            return GetType().Name.Replace("Bify", "");
        }
        public virtual void Initialize(List<BifyObject> args)
        {
            Traceback.Instance.ThrowException(new BifyOperationError(ErrorMessage.OperationNotSupported("Initialize", GetName())));
        }
        public virtual int GetInitializerArgs()
        {
            Traceback.Instance.ThrowException(new BifyOperationError(ErrorMessage.OperationNotSupported("GetInitializerArgs", GetName())));
            return 0;
        }
        public virtual BifyString Repr()
        {
            Traceback.Instance.ThrowException(new BifyOperationError(ErrorMessage.OperationNotSupported("Repr", GetName())));
            return null;
        }

        public virtual BifyString ObjectToString()
        {
            Traceback.Instance.ThrowException(new BifyOperationError(ErrorMessage.OperationNotSupported("ToString", GetName())));
            return null;
        }

        // Addition operator (+)
        public virtual BifyObject Add(BifyObject other)
        {
            Traceback.Instance.ThrowException(new BifyOperationError(ErrorMessage.OperationNotSupported("Addition", GetName())));
            return null;
        }

        // Subtraction operator (-)
        public virtual BifyObject Sub(BifyObject other)
        {
            Traceback.Instance.ThrowException(new BifyOperationError(ErrorMessage.OperationNotSupported("Subtraction", GetName())));
            return null;
        }

        // Multiplication operator (*)
        public virtual BifyObject Mul(BifyObject other)
        {
            Traceback.Instance.ThrowException(new BifyOperationError(ErrorMessage.OperationNotSupported("Multiplication", GetName())));
            return null;
        }

        // Division operator (/)
        public virtual BifyObject Div(BifyObject other)
        {
            Traceback.Instance.ThrowException(new BifyOperationError(ErrorMessage.OperationNotSupported("Division", GetName())));
            return null;
        }

        // Modulus operator (%)
        public virtual BifyObject Mod(BifyObject other)
        {
            Traceback.Instance.ThrowException(new BifyOperationError(ErrorMessage.OperationNotSupported("Modulus", GetName())));
            return null;
        }

        // Power operator (^)
        public virtual BifyObject Pow(BifyObject other)
        {
            Traceback.Instance.ThrowException(new BifyOperationError(ErrorMessage.OperationNotSupported("Power", GetName())));
            return null;
        }

        // Floor division operator (//)
        public virtual BifyObject FloorDiv(BifyObject other)
        {
            Traceback.Instance.ThrowException(new BifyOperationError(ErrorMessage.OperationNotSupported("Floor division", GetName())));
            return null;
        }

        // Equality operator (==)
        public virtual BifyObject Eq(BifyObject other)
        {
            Traceback.Instance.ThrowException(new BifyOperationError(ErrorMessage.OperationNotSupported("Equality", GetName())));
            return null;
        }

        // Inequality operator (!=)
        public virtual BifyObject Neq(BifyObject other)
        {
            Traceback.Instance.ThrowException(new BifyOperationError(ErrorMessage.OperationNotSupported("Inequality", GetName())));
            return null;
        }

        // Less than operator (<)
        public virtual BifyObject Lt(BifyObject other)
        {
            Traceback.Instance.ThrowException(new BifyOperationError(ErrorMessage.OperationNotSupported("Less than", GetName())));
            return null;
        }

        // Greater than operator (>)
        public virtual BifyObject Gt(BifyObject other)
        {
            Traceback.Instance.ThrowException(new BifyOperationError(ErrorMessage.OperationNotSupported("Greater than", GetName())));
            return null;
        }

        // Less than or equal to operator (<=)
        public virtual BifyObject Lte(BifyObject other)
        {
            Traceback.Instance.ThrowException(new BifyOperationError(ErrorMessage.OperationNotSupported("Less than or equal to", GetName())));
            return null;
        }

        // Greater than or equal to operator (>=)
        public virtual BifyObject Gte(BifyObject other)
        {
            Traceback.Instance.ThrowException(new BifyOperationError(ErrorMessage.OperationNotSupported("Greater than or equal to", GetName())));
            return null;
        }

        // Logical AND operator (&&)
        public virtual BifyObject And(BifyObject other)
        {
            Traceback.Instance.ThrowException(new BifyOperationError(ErrorMessage.OperationNotSupported("Logical AND", GetName())));
            return null;
        }

        // Logical OR operator (||)
        public virtual BifyObject Or(BifyObject other)
        {
            Traceback.Instance.ThrowException(new BifyOperationError(ErrorMessage.OperationNotSupported("Logical OR", GetName())));
            return null;
        }

        // Bitwise AND operator (&)
        public virtual BifyObject BitAnd(BifyObject other)
        {
            Traceback.Instance.ThrowException(new BifyOperationError(ErrorMessage.OperationNotSupported("Bitwise AND", GetName())));
            return null;
        }

        // Bitwise OR operator (|)
        public virtual BifyObject BitOr(BifyObject other)
        {
            Traceback.Instance.ThrowException(new BifyOperationError(ErrorMessage.OperationNotSupported("Bitwise OR", GetName())));
            return null;
        }

        // Bitwise XOR operator (^)
        public virtual BifyObject BitXor(BifyObject other)
        {
            Traceback.Instance.ThrowException(new BifyOperationError(ErrorMessage.OperationNotSupported("Bitwise XOR", GetName())));
            return null;
        }

        // Bitwise NOT operator (~)
        public virtual BifyObject BitNot()
        {
            Traceback.Instance.ThrowException(new BifyOperationError(ErrorMessage.OperationNotSupported("Bitwise NOT", GetName())));
            return null;
        }

        // Left shift operator (<<)
        public virtual BifyObject LeftShift(BifyObject other)
        {
            Traceback.Instance.ThrowException(new BifyOperationError(ErrorMessage.OperationNotSupported("Left shift", GetName())));
            return null;
        }

        // Right shift operator (>>)
        public virtual BifyObject RightShift(BifyObject other)
        {
            Traceback.Instance.ThrowException(new BifyOperationError(ErrorMessage.OperationNotSupported("Right shift", GetName())));
            return null;
        }

        // Unary NOT operator (!)
        public virtual BifyObject Not()
        {
            Traceback.Instance.ThrowException(new BifyOperationError(ErrorMessage.OperationNotSupported("Unary NOT", GetName())));
            return null;
        }

        public virtual BifyBoolean Bool()
        {
            Traceback.Instance.ThrowException(new BifyCastError($"Boolean conversion not supported for {GetName()}."));
            return null;
        }

        public virtual BifyObject Call(List<BifyObject> bifyObjects)
        {
            Traceback.Instance.ThrowException(new BifyOperationError(ErrorMessage.OperationNotSupported("Call", GetName())));
            return null;
        }

        public virtual BifyInteger Int()
        {
            Traceback.Instance.ThrowException(new BifyCastError($"Integer conversion not supported for {GetName()}."));
            return null;
        }

        public virtual BifyObject Index(BifyObject other)
        {
            Traceback.Instance.ThrowException(new BifyOperationError(ErrorMessage.OperationNotSupported("Index", GetName())));
            return null;
        }
    }
}
