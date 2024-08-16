using BoomifyCS.Lexer;
using System;
using System.Collections.Generic;
using BoomifyCS.Exceptions;

namespace BoomifyCS.Objects
{
    public class BifyObject(Token token)
    {
        public Token Token = token;
        public int ExpectedArgCount;
        public string GetName()
        {
            return GetType().Name.Replace("Bify", "");
        }

        public virtual BifyString Repr()
        {
            throw new BifyOperationError(ErrorMessage.OperationNotSupported("Repr", GetName()));

        }

        public virtual BifyString ObjectToString()
        {
            throw new BifyOperationError(ErrorMessage.OperationNotSupported("ToString", GetName()));
        }

        // Addition operator (+)
        public virtual BifyObject Add(BifyObject other)
        {
            throw new BifyOperationError(ErrorMessage.OperationNotSupported("Addition", GetName()));
        }

        // Subtraction operator (-)
        public virtual BifyObject Sub(BifyObject other)
        {
            throw new BifyOperationError(ErrorMessage.OperationNotSupported("Subtraction", GetName()));
        }

        // Multiplication operator (*)
        public virtual BifyObject Mul(BifyObject other)
        {
            throw new BifyOperationError(ErrorMessage.OperationNotSupported("Multiplication", GetName()));
        }

        // Division operator (/)
        public virtual BifyObject Div(BifyObject other)
        {
            throw new BifyOperationError(ErrorMessage.OperationNotSupported("Division", GetName()));
        }

        // Modulus operator (%)
        public virtual BifyObject Mod(BifyObject other)
        {
            throw new BifyOperationError(ErrorMessage.OperationNotSupported("Modulus", GetName()));
        }

        // Power operator (^)
        public virtual BifyObject Pow(BifyObject other)
        {
            throw new BifyOperationError(ErrorMessage.OperationNotSupported("Power", GetName()));
        }

        // Floor division operator (//)
        public virtual BifyObject FloorDiv(BifyObject other)
        {
            throw new BifyOperationError(ErrorMessage.OperationNotSupported("Floor division", GetName()));
        }

        // Equality operator (==)
        public virtual BifyObject Eq(BifyObject other)
        {
            throw new BifyOperationError(ErrorMessage.OperationNotSupported("Equality", GetName()));
        }

        // Inequality operator (!=)
        public virtual BifyObject Neq(BifyObject other)
        {
            throw new BifyOperationError(ErrorMessage.OperationNotSupported("Inequality", GetName()));
        }

        // Less than operator (<)
        public virtual BifyObject Lt(BifyObject other)
        {
            throw new BifyOperationError(ErrorMessage.OperationNotSupported("Less than", GetName()));
        }

        // Greater than operator (>)
        public virtual BifyObject Gt(BifyObject other)
        {
            throw new BifyOperationError(ErrorMessage.OperationNotSupported("Greater than", GetName()));
        }

        // Less than or equal to operator (<=)
        public virtual BifyObject Lte(BifyObject other)
        {
            throw new BifyOperationError(ErrorMessage.OperationNotSupported("Less than or equal to", GetName()));
        }

        // Greater than or equal to operator (>=)
        public virtual BifyObject Gte(BifyObject other)
        {
            throw new BifyOperationError(ErrorMessage.OperationNotSupported("Greater than or equal to", GetName()));
        }

        // Logical AND operator (&&)
        public virtual BifyObject And(BifyObject other)
        {
            throw new BifyOperationError(ErrorMessage.OperationNotSupported("Logical AND", GetName()));
        }

        // Logical OR operator (||)
        public virtual BifyObject Or(BifyObject other)
        {
            throw new BifyOperationError(ErrorMessage.OperationNotSupported("Logical OR", GetName()));
        }

        // Bitwise AND operator (&)
        public virtual BifyObject BitAnd(BifyObject other)
        {
            throw new BifyOperationError(ErrorMessage.OperationNotSupported("Bitwise AND", GetName()));
        }

        // Bitwise OR operator (|)
        public virtual BifyObject BitOr(BifyObject other)
        {
            throw new BifyOperationError(ErrorMessage.OperationNotSupported("Bitwise OR", GetName()));
        }

        // Bitwise XOR operator (^)
        public virtual BifyObject BitXor(BifyObject other)
        {
            throw new BifyOperationError(ErrorMessage.OperationNotSupported("Bitwise XOR", GetName()));
        }

        // Bitwise NOT operator (~)
        public virtual BifyObject BitNot()
        {
            throw new BifyOperationError(ErrorMessage.OperationNotSupported("Bitwise NOT", GetName()));
        }

        // Left shift operator (<<)
        public virtual BifyObject LeftShift(BifyObject other)
        {
            throw new BifyOperationError(ErrorMessage.OperationNotSupported("Left shift", GetName()));
        }

        // Right shift operator (>>)
        public virtual BifyObject RightShift(BifyObject other)
        {
            throw new BifyOperationError(ErrorMessage.OperationNotSupported("Right shift", GetName()));
        }

        // Unary NOT operator (!)
        public virtual BifyObject Not()
        {
            throw new BifyOperationError(ErrorMessage.OperationNotSupported("Unary NOT", GetName()));
        }

        public virtual BifyBoolean Bool()
        {
            throw new BifyCastError($"Boolean conversion not supported for {GetName()}.");
        }

        public virtual BifyObject Call(List<BifyObject> bifyObjects)
        {
            throw new BifyOperationError(ErrorMessage.OperationNotSupported("Call", GetName()));
        }

        public virtual BifyInteger Int()
        {
            throw new BifyCastError($"Integer conversion not supported for {GetName()}.");
        }

        public virtual BifyObject Index(BifyInteger other)
        {
            throw new BifyOperationError(ErrorMessage.OperationNotSupported("Index", GetName()));
        }
    }
}
