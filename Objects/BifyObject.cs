using BoomifyCS.Lexer;
using System;

namespace BoomifyCS.Objects
{
    public class BifyObject
    {
        public Token Token;

        public BifyObject(Token token)
        {
            this.Token = token;
        }

        // Addition operator (+)
        public virtual BifyObject Add(BifyObject other)
        {
            throw new InvalidOperationException($"Addition operation not supported for {GetType().Name}.");
        }

        // Subtraction operator (-)
        public virtual BifyObject Sub(BifyObject other)
        {
            throw new InvalidOperationException($"Subtraction operation not supported for {GetType().Name}.");
        }

        // Multiplication operator (*)
        public virtual BifyObject Mul(BifyObject other)
        {
            throw new InvalidOperationException($"Multiplication operation not supported for {GetType().Name}.");
        }

        // Division operator (/)
        public virtual BifyObject Div(BifyObject other)
        {
            throw new InvalidOperationException($"Division operation not supported for {GetType().Name}.");
        }

        // Modulus operator (%)
        public virtual BifyObject Mod(BifyObject other)
        {
            throw new InvalidOperationException($"Modulus operation not supported for {GetType().Name}.");
        }

        // Power operator (^)
        public virtual BifyObject Pow(BifyObject other)
        {
            throw new InvalidOperationException($"Power operation not supported for {GetType().Name}.");
        }

        // Floor division operator (//)
        public virtual BifyObject FloorDiv(BifyObject other)
        {
            throw new InvalidOperationException($"Floor division operation not supported for {GetType().Name}.");
        }

        // Equality operator (==)
        public virtual BifyObject Eq(BifyObject other)
        {
            throw new InvalidOperationException($"Equality operation not supported for {GetType().Name}.");
        }

        // Inequality operator (!=)
        public virtual BifyObject Neq(BifyObject other)
        {
            throw new InvalidOperationException($"Inequality operation not supported for {GetType().Name}.");
        }

        // Less than operator (<)
        public virtual BifyObject Lt(BifyObject other)
        {
            throw new InvalidOperationException($"Less than operation not supported for {GetType().Name}.");
        }

        // Greater than operator (>)
        public virtual BifyObject Gt(BifyObject other)
        {
            throw new InvalidOperationException($"Greater than operation not supported for {GetType().Name}.");
        }

        // Less than or equal to operator (<=)
        public virtual BifyObject Lte(BifyObject other)
        {
            throw new InvalidOperationException($"Less than or equal to operation not supported for {GetType().Name}.");
        }

        // Greater than or equal to operator (>=)
        public virtual BifyObject Gte(BifyObject other)
        {
            throw new InvalidOperationException($"Greater than or equal to operation not supported for {GetType().Name}.");
        }

        // Logical AND operator (&&)
        public virtual BifyObject And(BifyObject other)
        {
            throw new InvalidOperationException($"Logical AND operation not supported for {GetType().Name}.");
        }

        // Logical OR operator (||)
        public virtual BifyObject Or(BifyObject other)
        {
            throw new InvalidOperationException($"Logical OR operation not supported for {GetType().Name}.");
        }

        // Bitwise AND operator (&)
        public virtual BifyObject BitAnd(BifyObject other)
        {
            throw new InvalidOperationException($"Bitwise AND operation not supported for {GetType().Name}.");
        }

        // Bitwise OR operator (|)
        public virtual BifyObject BitOr(BifyObject other)
        {
            throw new InvalidOperationException($"Bitwise OR operation not supported for {GetType().Name}.");
        }

        // Bitwise XOR operator (^)
        public virtual BifyObject BitXor(BifyObject other)
        {
            throw new InvalidOperationException($"Bitwise XOR operation not supported for {GetType().Name}.");
        }

        // Bitwise NOT operator (~)
        public virtual BifyObject BitNot()
        {
            throw new InvalidOperationException($"Bitwise NOT operation not supported for {GetType().Name}.");
        }

        // Left shift operator (<<)
        public virtual BifyObject LeftShift(BifyObject other)
        {
            throw new InvalidOperationException($"Left shift operation not supported for {GetType().Name}.");
        }

        // Right shift operator (>>)
        public virtual BifyObject RightShift(BifyObject other)
        {
            throw new InvalidOperationException($"Right shift operation not supported for {GetType().Name}.");
        }

        // Unary NOT operator (!)
        public virtual BifyObject Not()
        {
            throw new InvalidOperationException($"Unary NOT operation not supported for {GetType().Name}.");
        }
    }
}
