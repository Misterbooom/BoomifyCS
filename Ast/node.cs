using System;
using BoomifyCS.Lexer;
using BoomifyCS.Objects;

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

        public override string StrHelper(int level = 0, string note = "")
        {
            string baseStr = base.StrHelper(level, note);
            return baseStr + $"{new String(' ', 4 * (level + 1))}Binary Operation\n";
        }
        public override string ToString()
        {
            return StrHelper();
        }
    }

    public class AstConstant : AstNode
    {
        public BifyObject BifyValue;
        public AstConstant(Token token) : base(token)
        {
        }

        public override string StrHelper(int level = 0, string note = "")
        {
            string baseStr = base.StrHelper(level, note);
            return baseStr + $"{new String(' ', 4 * (level + 1))}Constant Value\n";
        }
        public override string ToString()
        {
            return StrHelper();
        }
    }

    public class AstInt : AstConstant
    {
        public AstInt(Token token, BifyInteger bifyValue) : base(token)
        {
            this.BifyValue = bifyValue;
        }

        public override string StrHelper(int level = 0, string note = "")
        {
            string baseStr = base.StrHelper(level, note);
            return baseStr + $"{new String(' ', 4 * (level + 1))}Integer Value: {BifyValue}\n";
        }

        public override string ToString()
        {
            return StrHelper();
        }
    }

    public class AstString : AstConstant
    {
        public AstString(Token token, BifyString bifyValue) : base(token)
        {
            this.BifyValue = bifyValue;
        }

        public override string StrHelper(int level = 0, string note = "")
        {
            string baseStr = base.StrHelper(level, note);
            return baseStr + $"{new String(' ', 4 * (level + 1))}String Value: {BifyValue}\n";
        }

        public override string ToString()
        {
            return StrHelper();
        }
    }

    public class AstBoolean : AstConstant
    {
        public AstBoolean(Token token, BifyBoolean bifyValue) : base(token)
        {
            this.BifyValue = bifyValue;
        }

        public override string StrHelper(int level = 0, string note = "")
        {
            string baseStr = base.StrHelper(level, note);
            return baseStr + $"{new String(' ', 4 * (level + 1))}Boolean Value: {BifyValue}\n";
        }

        public override string ToString()
        {
            return StrHelper();
        }
    }
    public class AstNull : AstConstant
    {
        public AstNull(Token token) : base(token)
        {
            this.BifyValue = new BifyNull(token);
        }
    }

    public class AstAssignment : AstNode
    {
        public AstAssignment(Token token, AstNode left = null, AstNode right = null) : base(token, left, right)
        {
        }

        public override string StrHelper(int level = 0, string note = "")
        {
            string baseStr = "";
            try
            {
                baseStr += Left.StrHelper(level, "Var name: ");
                baseStr += Right.StrHelper(level, "Var Value: ");
            }
            catch (NullReferenceException)
            {
                throw new NullReferenceException("Left or right value in AstAssignment is null");
            }

            return baseStr + $"{new String(' ', 4 * (level + 1))}\n";
        }
        public override string ToString()
        {
            return StrHelper();
        }
    }

    public class AstVarDecl : AstNode
    {
        public AstAssignment AssignmentNode;
        public AstVarDecl(Token token, AstAssignment assignmentNode, AstNode left = null, AstNode right = null) : base(token, left, right)
        {
            AssignmentNode = assignmentNode;
        }

        public override string StrHelper(int level = 0, string note = "")
        {
            string baseStr = base.StrHelper(level, note);
            string assignmentStr = AssignmentNode?.StrHelper(level + 1, "assignment:") ?? "";
            return baseStr + $"{new String(' ', 4 * (level + 1))}\n{assignmentStr}";
        }
        public override string ToString()
        {
            return StrHelper();
        }
    }
    public class AstLine : AstNode
    {
        public AstLine(AstNode left = null, AstNode right = null) : base(new Token(TokenType.EOL, ";"), left, right)
        {
        }
    }
    public class AstIdentifier : AstNode
    {
        public string Name;
        public AstIdentifier(Token token, string name) : base(token)
        {
            this.Name = name;
        }
    }
    public class AstBracket : AstNode
    {
        public AstBracket(Token token) : base(token)
        {
        }
    }
    public class AstEOL : AstNode
    {
        public AstEOL(Token token) : base(token) { }
    }
    public class AstBlock : AstNode
    {

        public AstNode StatementsNode;
        public AstBlock(AstNode statementsNode) : base(new Token(TokenType.BLOCK, "Block"))
        {
            this.StatementsNode = statementsNode;
        }
        public override string StrHelper(int level = 0, string note = "")
        {
            string baseStr = base.StrHelper(level, note);
            string statementsStr = StatementsNode?.StrHelper(level + 1, "statements: ") ?? "";
            return baseStr + $"{new String(' ', 4 * (level + 1))}\n{statementsStr}";
        }
    }
    public class AstIf : AstNode
    {
        public AstNode ConditionNode;
        public AstBlock BlockNode;
        public AstElse ElseNode;
        public AstIf(Token token, AstNode conditionNode, AstBlock blockNode, AstElse elseNode = null) : base(token)
        {
            this.ElseNode = elseNode;
            this.ConditionNode = conditionNode;
            this.BlockNode = blockNode;
        }
        public override string StrHelper(int level = 0, string note = "")
        {
            string baseStr = base.StrHelper(level, note);
            string conditionStr = ConditionNode.StrHelper(level + 1, "condition: ");
            string blockStr = BlockNode.StrHelper(level + 1, "block: ");
            if (ElseNode != null)
            {
                string elseStr = ElseNode.StrHelper(level + 1, "else: ");
                return baseStr + $"{new String(' ', 4 * (level + 1))}\n{conditionStr}\n{blockStr}\n{elseStr}";

            }
            return baseStr + $"{new String(' ', 4 * (level + 1))}\n{conditionStr}\n{blockStr}";
        }
        public override string ToString()
        {
            return StrHelper();
        }
    }
    public class AstElse : AstNode
    {
        public AstBlock BlockNode;
        public AstElse(Token token, AstBlock blockNode) : base(token, blockNode)
        {
            this.BlockNode = blockNode;
        }
        public override string StrHelper(int level = 0, string note = "")
        {
            string baseStr = base.StrHelper(level, note);
            string blockStr = BlockNode.StrHelper(level + 1, "block: ");
            return baseStr + $"{new String(' ', 4 * (level + 1))}\n{blockStr}";
        }
    }
}

