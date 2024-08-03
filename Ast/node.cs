using System;
using System.Collections.Generic;
using BoomifyCS.Lexer;
using BoomifyCS.Objects;
using BoomifyCS.Parser;

namespace BoomifyCS.Ast
{
    public class AstNode
    {
        public Token Token { get; set; }
        public AstNode Left { get; set; }
        public AstNode Right { get; set; }

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

        public virtual string StrHelper(int level = 0, string note = "", bool isLeft = true)
        {
            string indent = new String(' ', 4 * level);
            string branch = isLeft ? "|- " : "|- ";
            string treeStr = $"{indent}{branch}{note}{GetType().Name}({Token.Value})\n";

            if (Left != null)
            {
                treeStr += Left.StrHelper(level + 1, "left:", true);
            }

            if (Right != null)
            {
                treeStr += Right.StrHelper(level + 1, "right:", false);
            }

            return treeStr;
        }
        public int Len(int level = 1)
        {
            int leftLen = (Left != null) ? Left.Len(level + 1) : level;
            int rightLen = (Right != null) ? Right.Len(level + 1) : level;
            return Math.Max(leftLen, rightLen);
        }

    }


    public class AstBinaryOp : AstNode
    {
        public AstBinaryOp(Token token, AstNode left = null, AstNode right = null) : base(token, left, right)
        {
        }

        public override string StrHelper(int level = 0, string note = "", bool isLeft = true)
        {
            string baseStr = base.StrHelper(level, note);
            return baseStr + $"{new String(' ', 4 * (level + 1))}\n";
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

        public override string StrHelper(int level = 0, string note = "", bool isLeft = true)
        {
            string baseStr = base.StrHelper(level, note);
            return baseStr + $"{new String(' ', 4 * (level + 1))}";
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

        public override string StrHelper(int level = 0, string note = "", bool isLeft = true)
        {
            string baseStr = base.StrHelper(level, note);
            return baseStr + $"{new String(' ', 4 * (level + 1))}\n";
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

        public override string StrHelper(int level = 0, string note = "", bool isLeft = true)
        {
            string baseStr = base.StrHelper(level, note);
            return baseStr + $"{new String(' ', 4 * (level + 1))}\n";
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

        public override string StrHelper(int level = 0, string note = "", bool isLeft = true)
        {
            string baseStr = base.StrHelper(level, note);
            return baseStr + $"{new String(' ', 4 * (level + 1))}\n";
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
    public class AstVar : AstConstant
    {

        public AstVar(Token token,BifyVar bifyVar):base(token) {
            this.BifyValue = bifyVar;
        }
    }

    public class AstAssignment : AstNode
    {
        public AstAssignment(Token token, AstNode left = null, AstNode right = null) : base(token, left, right)
        {
        }

        public override string StrHelper(int level = 0, string note = "", bool isLeft = true)
        {
            string baseStr = "";

            string leftStr = Left.StrHelper(level, "Var name: ");
            string rightStr = Right.StrHelper(level, "Var Value: ");
            

            return baseStr + $"{new String(' ', 4 * (level + 1))}\n{leftStr}\n{rightStr}";
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

        public override string StrHelper(int level = 0, string note = "", bool isLeft = true)
        {
            string baseStr = base.StrHelper(level, note);
            string assignmentStr = AssignmentNode?.StrHelper(level + 1, "Assignment:") ?? "";
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
        public override string StrHelper(int level = 0, string note = "", bool isLeft = true)
        {
            return base.StrHelper(level, note, isLeft);
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
        public override string StrHelper(int level = 0, string note = "", bool isLeft = true)
        {
            string baseStr = base.StrHelper(level, note);
            string statementsStr = StatementsNode?.StrHelper(level + 1, "Statements: ") ?? "";
            return baseStr + $"{new String(' ', 4 * (level + 1))}\n{statementsStr}";
        }
    }
    public class AstIf : AstNode
    {
        public AstNode ConditionNode;
        public AstBlock BlockNode;
        public AstElse ElseNode;
        public List<AstElseIf> ElseIfNodes = new List<AstElseIf>();
        public AstIf(Token token, AstNode conditionNode, AstBlock blockNode, AstElse elseNode = null) : base(token)
        {
            this.ElseNode = elseNode;
            this.ConditionNode = conditionNode;
            this.BlockNode = blockNode;
        }
        public override string StrHelper(int level = 0, string note = "", bool isLeft = true)
        {
            string baseStr = base.StrHelper(level, note);
            string conditionStr = ConditionNode.StrHelper(level + 1, "condition: ");

            string blockStr = "";
            if (BlockNode != null)
            {
                blockStr = BlockNode.StrHelper(level + 1, "block: ");
            }

            string elseStr = "";
            if (ElseNode != null)
            {
                elseStr = ElseNode.StrHelper(level + 1, "else: ");
            }
            string elseIfNodes = "";
            foreach(AstElseIf astElseIf in ElseIfNodes)
            {
                elseIfNodes += astElseIf.StrHelper(level + 1, "else if: ");
            }
            

            return baseStr + $"{new String(' ', 4 * (level + 1))}\n{conditionStr}\n{blockStr}\n{elseStr}\n{elseIfNodes}";
        }

        public override string ToString()
        {
            return StrHelper();
        }
        public void SetElseIfNode(AstElseIf astElseIf)
        {
            ElseIfNodes.Add(astElseIf);
        }
    }
    public class AstElse : AstNode
    {
        public AstBlock BlockNode;
        public AstElse(Token token, AstBlock blockNode) : base(token, blockNode)
        {
            this.BlockNode = blockNode;
        }
        public override string StrHelper(int level = 0, string note = "", bool isLeft = true)
        {
            string baseStr = base.StrHelper(level, note);
            string blockStr = BlockNode.StrHelper(level + 1, "block: ");
            return baseStr + $"{new String(' ', 4 * (level + 1))}\n{blockStr}";
        }
    }
    public class AstElseIf : AstNode
    {
        public AstBlock BlockNode;
        public AstNode ConditionNode;
        public AstElseIf(Token token, AstBlock blockNode, AstNode conditionNode) : base(token)
        {
            this.BlockNode = blockNode;
            this.ConditionNode = conditionNode;
        }
        public override string StrHelper(int level = 0, string note = "", bool isLeft = true)
        {
            string baseStr = base.StrHelper(level, note);
            string conditionStr = ConditionNode.StrHelper(level + 1, "Condition: ");
            string blockStr = BlockNode.StrHelper(level + 1, "Block: ");
            return baseStr + $"{new String(' ', 4 * (level + 1))}\n{conditionStr}\n{blockStr}";

        }
    }
    public class AstWhile : AstNode
    {
        public AstBlock BlockNode;
        public AstNode ConditionNode;
        public AstWhile(Token token, AstBlock blockNode, AstNode conditionNode) : base(token)
        {
            this.BlockNode = blockNode;
            this.ConditionNode = conditionNode;
        }
        public override string StrHelper(int level = 0, string note = "", bool isLeft = true)
        {
            string baseStr = base.StrHelper(level, note);
            string conditionStr = ConditionNode.StrHelper(level + 1, "Condition: ");
            string blockStr = BlockNode.StrHelper(level + 1, "Block: ");
            return baseStr + $"{new String(' ', 4 * (level + 1))}\n{conditionStr}\n{blockStr}";

        }
    }
    public class AstFor : AstNode
    {
        public AstBlock BlockNode;
        public AstNode ConditionNode;
        public AstNode IncrementNode;
        public AstNode InitNode;
        public AstFor(Token token, AstBlock blockNode, AstNode conditionNode, AstNode incrementNode, AstNode initNode) : base(token)
        {
            this.BlockNode = blockNode;
            this.ConditionNode = conditionNode;
            this.IncrementNode = incrementNode;
            this.InitNode = initNode;
        }
        public override string StrHelper(int level = 0, string note = "", bool isLeft = true)
        {
            string baseStr = base.StrHelper(level, note);
            string conditionStr = ConditionNode.StrHelper(level + 1, "Condition: ");
            string incrementStr = IncrementNode.StrHelper(level + 1, "Increment: ");
            string blockStr = BlockNode.StrHelper(level + 1, "Block");
            return baseStr + $"{new String(' ', 4 * (level + 1))}\n{conditionStr}\n{incrementStr}\n{blockStr}";
        }
    }
    public class AstCall : AstNode
    {
        public AstNode CallableName;
        public AstNode ArgumentsNode;
        public AstCall(Token token, AstNode callableName, AstNode argumentsNode = null, AstNode returnNode = null) : base(token)
        {
            this.CallableName = callableName;
            this.ArgumentsNode = argumentsNode;
        }
        public override string StrHelper(int level = 0, string note = "", bool isLeft = true)
        {
            string baseStr = base.StrHelper(level, note);
            string callableStr = CallableName.StrHelper(level + 1, "Name: ");
            string argumentsStr = ArgumentsNode.StrHelper(level + 2, "Arguments: ");
            return baseStr + $"{new String(' ', 4 * (level + 1))}\n{callableStr}\n{argumentsStr}";

        }
    }
    public class AstUnaryOperator : AstNode
    {
        public AstNode value;
        public int increment;
        public AstUnaryOperator(Token token, AstNode value, int increment) : base(token)
        {
            this.value = value;
            this.increment = increment;
        }
        public override string StrHelper(int level = 0, string note = "", bool isLeft = true)
        {
            string baseStr = base.StrHelper(level, note);
            string valueStr = value.StrHelper(level + 1, "Value: ");
            return baseStr + $"{new String(' ', 4 * (level + 1))}\n{valueStr}";
        }
    }
    public class AstFunctionDecl : AstNode
    {
        public AstNode argumentsNode;
        public AstIdentifier functionNameNode;
        public AstBlock blockNode;
        public AstFunctionDecl(Token token, AstIdentifier functionNameNode, AstNode argumentsNode, AstBlock blockNode) : base(token)
        {
            this.functionNameNode = functionNameNode;
            this.argumentsNode = argumentsNode;
            this.blockNode = blockNode;
        }
        public override string StrHelper(int level = 0, string note = "", bool isLeft = true)
        {
            string baseStr = base.StrHelper(level, note,false);
            string functionNameStr = functionNameNode.StrHelper(level + 1, "Name: ");
            string argumentsStr = argumentsNode.StrHelper(level + 1, "Arguments: ");
            string blockStr = blockNode.StrHelper(level + 1, "Block: ");
            return baseStr + $"{new String(' ', 4 * (level + 1))}\n{functionNameStr}\n{argumentsStr}\n{blockStr}";
        }
    }
    //public class AstComma : AstNode
    //{
    //    public AstComma(Token token, AstNode left = null, AstNode right = null) : base(token, left, right) { }

    //}

}

