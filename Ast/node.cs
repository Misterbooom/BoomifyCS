using System;
using BoomifyCS.Lexer;

namespace BoomifyCS.Ast
{
    public class AstNode
    {
        public Token Token;
        public AstNode Left;
        public AstNode Right;

        public AstNode(Token token, AstNode left = null, AstNode right = null)
        {
            Token = token;
            Left = left;
            Right = right;
        }

        public override string ToString()
        {
            return StrHelper();
        }

        public virtual string StrHelper(int level = 0, string note = "")
        {
            string indent = new String(' ', 4 * level);
            string treeStr = $"{indent}└──{note}{GetType().Name}({Token.Value})\n";

            if (Left != null)
            {
                treeStr += Left.StrHelper(level + 1, "left:");
            }

            if (Right != null)
            {
                treeStr += Right.StrHelper(level + 1, "right:");
            }

            return treeStr;
        }
    }

    public class AstBinaryOp : AstNode
    {
        public AstBinaryOp(Token token, AstNode left = null, AstNode right = null) : base(token, left, right)
        {

        }
    }
    public class AstConstant:AstNode
    {
        public AstConstant(Token token, AstNode left = null, AstNode right = null) : base(token, left, right)
        {

        }
    }





}
