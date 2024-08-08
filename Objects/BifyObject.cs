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

        public virtual string Repr()
        {
            throw new BifyUnknownError($"Repr operation not supported for {GetType().Name}.");
        }
        // Addition operator (+)
        public virtual BifyObject Add(BifyObject other)
        {
            throw new BifyOperationError($"Addition operation not supported for {GetType().Name}.");
        }

        // Subtraction operator (-)
        public virtual BifyObject Sub(BifyObject other)
        {
            throw new BifyOperationError($"Subtraction operation not supported for {GetType().Name}.");
        }

        // Multiplication operator (*)
        public virtual BifyObject Mul(BifyObject other)
        {
            throw new BifyOperationError($"Multiplication operation not supported for {GetType().Name}.");
        }

        // Division operator (/)
        public virtual BifyObject Div(BifyObject other)
        {
            throw new BifyOperationError($"Division operation not supported for {GetType().Name}.");
        }

        // Modulus operator (%)
        public virtual BifyObject Mod(BifyObject other)
        {
            throw new BifyOperationError($"Modulus operation not supported for {GetType().Name}.");
        }

        // Power operator (^)
        public virtual BifyObject Pow(BifyObject other)
        {
            throw new BifyOperationError($"Power operation not supported for {GetType().Name}.");
        }

        // Floor division operator (//)
        public virtual BifyObject FloorDiv(BifyObject other)
        {
            throw new BifyOperationError($"Floor division operation not supported for {GetType().Name}.");
        }

        // Equality operator (==)
        public virtual BifyObject Eq(BifyObject other)
        {
            throw new BifyOperationError($"Equality operation not supported for {GetType().Name}.");
        }

        // Inequality operator (!=)
        public virtual BifyObject Neq(BifyObject other)
        {
            throw new BifyOperationError($"Inequality operation not supported for {GetType().Name}.");
        }

        // Less than operator (<)
        public virtual BifyObject Lt(BifyObject other)
        {
            throw new BifyOperationError($"Less than operation not supported for {GetType().Name}.");
        }

        // Greater than operator (>)
        public virtual BifyObject Gt(BifyObject other)
        {
            throw new BifyOperationError($"Greater than operation not supported for {GetType().Name}.");
        }

        // Less than or equal to operator (<=)
        public virtual BifyObject Lte(BifyObject other)
        {
            throw new BifyOperationError($"Less than or equal to operation not supported for {GetType().Name}.");
        }

        // Greater than or equal to operator (>=)
        public virtual BifyObject Gte(BifyObject other)
        {
            throw new BifyOperationError($"Greater than or equal to operation not supported for {GetType().Name}.");
        }

        // Logical AND operator (&&)
        public virtual BifyObject And(BifyObject other)
        {
            throw new BifyOperationError($"Logical AND operation not supported for {GetType().Name}.");
        }

        // Logical OR operator (||)
        public virtual BifyObject Or(BifyObject other)
        {
            throw new BifyOperationError($"Logical OR operation not supported for {GetType().Name}.");
        }

        // Bitwise AND operator (&)
        public virtual BifyObject BitAnd(BifyObject other)
        {
            throw new BifyOperationError($"Bitwise AND operation not supported for {GetType().Name}.");
        }

        // Bitwise OR operator (|)
        public virtual BifyObject BitOr(BifyObject other)
        {
            throw new BifyOperationError($"Bitwise OR operation not supported for {GetType().Name}.");
        }

        // Bitwise XOR operator (^)
        public virtual BifyObject BitXor(BifyObject other)
        {
            throw new BifyOperationError($"Bitwise XOR operation not supported for {GetType().Name}.");
        }

        // Bitwise NOT operator (~)
        public virtual BifyObject BitNot()
        {
            throw new BifyOperationError($"Bitwise NOT operation not supported for {GetType().Name}.");
        }

        // Left shift operator (<<)
        public virtual BifyObject LeftShift(BifyObject other)
        {
            throw new BifyOperationError($"Left shift operation not supported for {GetType().Name}.");
        }

        // Right shift operator (>>)
        public virtual BifyObject RightShift(BifyObject other)
        {
            throw new BifyOperationError($"Right shift operation not supported for {GetType().Name}.");
        }

        // Unary NOT operator (!)
        public virtual BifyObject Not()
        {
            throw new BifyOperationError($"Unary NOT operation not supported for {GetType().Name}.");
        }
        public virtual BifyBoolean Bool()
        {
            throw new BifyOperationError($"Boolean conversion not supported for {GetType().Name}.");
        }
        public virtual BifyObject Call(List<BifyObject> bifyObjects)
        {
            throw new BifyOperationError($"Call operation not supported for {GetType().Name}.");
        }
        public virtual BifyObject Int()
        {
            throw new BifyOperationError($"Integer conversion not supported for {GetType().Name}.");

        }


    }
}
