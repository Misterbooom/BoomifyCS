using System;
using System.Collections.Generic;
using BoomifyCS.Lexer;
using BoomifyCS.Objects;

namespace BoomifyCS.Ast
{
    public class AstNode(Token token, AstNode left = null, AstNode right = null)
    {
        public Token Token { get; set; } = token;
        public AstNode Left { get; set; } = left;
        public AstNode Right { get; set; } = right;
        public int LineNumber;

        public override string ToString()
        {
            return StrHelper();
        }

        public virtual string StrHelper(int level = 0, string note = "", bool isLeft = true)
        {
            string indent = new(' ', 4 * level);
            string branch = isLeft ? "|- " : "|- ";
            string treeStr = $"{indent}{branch}{note}{GetType().Name}('{Token.Value}',line - {LineNumber})\n";

            if (Left != null)
            {
                treeStr += Left?.StrHelper(level + 1, "left:", true);
            }

            if (Right != null)
            {
                treeStr += Right?.StrHelper(level + 1, "right:", false);
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


    public class AstBinaryOp(Token token, AstNode left = null, AstNode right = null) : AstNode(token, left, right)
    {
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

    public class AstConstant(Token token) : AstNode(token)
    {
        public BifyObject BifyValue;

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

        public AstVar(Token token, BifyVar bifyVar) : base(token)
        {
            this.BifyValue = bifyVar;
        }
    }

    public class AstAssignment(Token token, AstNode left = null, AstNode right = null) : AstNode(token, left, right)
    {
        public override string StrHelper(int level = 0, string note = "", bool isLeft = true)
        {
            string baseStr = "";

            string leftStr = Left?.StrHelper(level, "Var name: ");
            string rightStr = Right?.StrHelper(level, "Var Value: ");


            return baseStr + $"{new String(' ', 4 * (level + 1))}\n{leftStr}\n{rightStr}";
        }
        public override string ToString()
        {
            return StrHelper();
        }
    }

    public class AstVarDecl(Token token, AstAssignment assignmentNode, AstNode left = null, AstNode right = null) : AstNode(token, left, right)
    {
        public AstAssignment AssignmentNode = assignmentNode;


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
    public class AstLine(AstNode child) : AstNode(new Token(TokenType.EOL, ";"))
    {
        public AstNode Child = child;

        public override string StrHelper(int level = 0, string note = "", bool isLeft = true)
        {
            string baseStr = base.StrHelper(level, note, isLeft);
            string childStr = Child?.StrHelper(level + 1, "Child: ");
            return baseStr + $"{new String(' ', 4 * (level + 1))}\n{childStr}";

        }

    }
    public class AstIdentifier(Token token, string name) : AstNode(token)
    {
        public string Name = name;
    }
    public class AstBracket(Token token) : AstNode(token)
    {
    }
    public class AstEOL(Token token) : AstNode(token)
    {
    }
    public class AstBlock(AstNode statementsNode) : AstNode(new Token(TokenType.BLOCK, "Block"))
    {

        public AstNode StatementsNode = statementsNode;

        public override string StrHelper(int level = 0, string note = "", bool isLeft = true)
        {
            string baseStr = base.StrHelper(level, note);
            string statementsStr = StatementsNode?.StrHelper(level + 1, "Statements: ") ?? "";
            return baseStr + $"{new String(' ', 4 * (level + 1))}\n{statementsStr}";
        }
    }
    public class AstIf(Token token, AstNode conditionNode, AstBlock blockNode, AstElse elseNode = null) : AstNode(token)
    {
        public AstNode ConditionNode = conditionNode;
        public AstBlock BlockNode = blockNode;
        public AstElse ElseNode = elseNode;
        public List<AstElseIf> ElseIfNodes = [];

        public override string StrHelper(int level = 0, string note = "", bool isLeft = true)
        {
            string baseStr = base.StrHelper(level, note);
            string conditionStr = ConditionNode?.StrHelper(level + 1, "condition: ");

            string blockStr = "";
            if (BlockNode != null)
            {
                blockStr = BlockNode?.StrHelper(level + 1, "block: ");
            }

            string elseStr = "";
            if (ElseNode != null)
            {
                elseStr = ElseNode?.StrHelper(level + 1, "else: ");
            }
            string elseIfNodes = "";
            foreach (AstElseIf astElseIf in ElseIfNodes)
            {
                elseIfNodes += astElseIf?.StrHelper(level + 1, "else if: ");
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
    public class AstElse(Token token, AstBlock blockNode) : AstNode(token, blockNode)
    {
        public AstBlock BlockNode = blockNode;

        public override string StrHelper(int level = 0, string note = "", bool isLeft = true)
        {
            string baseStr = base.StrHelper(level, note);
            string blockStr = BlockNode?.StrHelper(level + 1, "block: ");
            return baseStr + $"{new String(' ', 4 * (level + 1))}\n{blockStr}";
        }
    }
    public class AstElseIf(Token token, AstBlock blockNode, AstNode conditionNode) : AstNode(token)
    {
        public AstBlock BlockNode = blockNode;
        public AstNode ConditionNode = conditionNode;

        public override string StrHelper(int level = 0, string note = "", bool isLeft = true)
        {
            string baseStr = base.StrHelper(level, note);
            string conditionStr = ConditionNode?.StrHelper(level + 1, "Condition: ");
            string blockStr = BlockNode?.StrHelper(level + 1, "Block: ");
            return baseStr + $"{new String(' ', 4 * (level + 1))}\n{conditionStr}\n{blockStr}";

        }
    }
    public class AstWhile(Token token, AstBlock blockNode, AstNode conditionNode) : AstNode(token)
    {
        public AstBlock BlockNode = blockNode;
        public AstNode ConditionNode = conditionNode;

        public override string StrHelper(int level = 0, string note = "", bool isLeft = true)
        {
            string baseStr = base.StrHelper(level, note);
            string conditionStr = ConditionNode?.StrHelper(level + 1, "Condition: ");
            string blockStr = BlockNode?.StrHelper(level + 1, "Block: ");
            return baseStr + $"{new String(' ', 4 * (level + 1))}\n{conditionStr}\n{blockStr}";

        }
    }
    public class AstFor(Token token, AstBlock blockNode, AstNode conditionNode, AstNode incrementNode, AstNode initNode) : AstNode(token)
    {
        public AstBlock BlockNode = blockNode;
        public AstNode ConditionNode = conditionNode;
        public AstNode IncrementNode = incrementNode;
        public AstNode InitNode = initNode;

        public override string StrHelper(int level = 0, string note = "", bool isLeft = true)
        {
            string baseStr = base.StrHelper(level, note);
            string conditionStr = ConditionNode?.StrHelper(level + 1, "Condition: ");
            string incrementStr = IncrementNode?.StrHelper(level + 1, "Increment: ");
            string initNode = InitNode?.StrHelper(level + 1, "Init: ");
            string blockStr = BlockNode?.StrHelper(level + 1, "Block");
            return baseStr + $"{new String(' ', 4 * (level + 1))}\n{initNode}\n{conditionStr}\n{incrementStr}\n{blockStr}";
        }
    }
    public class AstCall(Token token, AstNode callableName, AstNode argumentsNode = null) : AstNode(token)
    {
        public AstNode CallableName = callableName;
        public AstNode ArgumentsNode = argumentsNode;

        public override string StrHelper(int level = 0, string note = "", bool isLeft = true)
        {
            string baseStr = base.StrHelper(level, note);
            string callableStr = CallableName?.StrHelper(level + 1, "Name: ");
            string argumentsStr = ArgumentsNode != null ? ArgumentsNode?.StrHelper(level + 2, "Arguments: ") : "";
            return baseStr + $"{new String(' ', 4 * (level + 1))}\n{callableStr}\n{argumentsStr}";

        }
    }
    public class AstUnaryOperator(Token token, AstNode value, int increment) : AstNode(token)
    {
        public AstNode value = value;
        public int increment = increment;

        public override string StrHelper(int level = 0, string note = "", bool isLeft = true)
        {
            string baseStr = base.StrHelper(level, note);
            string valueStr = value?.StrHelper(level + 1, "Value: ");
            return baseStr + $"{new String(' ', 4 * (level + 1))}\n{valueStr}";
        }
    }
    public class AstFunctionDecl(Token token, AstIdentifier functionNameNode, AstNode argumentsNode, AstBlock blockNode) : AstNode(token)
    {
        public AstNode argumentsNode = argumentsNode;
        public AstIdentifier functionNameNode = functionNameNode;
        public AstBlock blockNode = blockNode;

        public override string StrHelper(int level = 0, string note = "", bool isLeft = true)
        {
            string baseStr = base.StrHelper(level, note, false);
            string functionNameStr = functionNameNode?.StrHelper(level + 1, "Name: ");
            string argumentsStr = argumentsNode?.StrHelper(level + 1, "Arguments: ");
            string blockStr = blockNode?.StrHelper(level + 1, "Block: ");
            return baseStr + $"{new String(' ', 4 * (level + 1))}\n{functionNameStr}\n{argumentsStr}\n{blockStr}";
        }
    }
    public class AstModule(Token token, string moduleName, string modulePath, AstNode childNode = null) : AstNode(token)
    {
        public AstNode ChildNode = childNode;
        public string ModuleName = moduleName;
        public string ModulePath = modulePath;

        public override string StrHelper(int level = 0, string note = "", bool isLeft = true)
        {
            string baseStr = base.StrHelper(level, note, false);
            string childStr = ChildNode?.StrHelper(level + 1, "Child: ");
            return baseStr + $"{new String(' ', 4 * (level + 1))}\n{childStr}";

        }
    }
    public class AstAssignmentOperator(Token token, AstIdentifier identifierNode, AstNode valueNode) : AstNode(token)
    {
        public AstIdentifier IdentifierNode = identifierNode;
        public AstNode ValueNode = valueNode;

        public override string StrHelper(int level = 0, string note = "", bool isLeft = true)
        {
            string baseStr = base.StrHelper(level, note, false);

            string identifierStr = IdentifierNode?.StrHelper(level + 1, "Identifier: ");
            string valueStr = ValueNode?.StrHelper(level + 1, "Value: ");
            return baseStr + $"{new String(' ', 4 * (level + 1))}\n{identifierStr}\n{valueStr}";
        }

    }
    public class AstBreak(Token token) : AstNode(token) { }
    public class AstContinue(Token token) : AstNode(token) { }
    public class AstArray(Token token, AstNode argumentsNode) : AstNode(token)
    {
        public AstNode ArgumentsNode = argumentsNode;
        public override string StrHelper(int level = 0, string note = "", bool isLeft = true)
        {
            string baseStr = base.StrHelper(level, note, false);
            string argumentsStr = ArgumentsNode?.StrHelper(level + 1, "Arguments: ");
            return baseStr + $"{new String(' ', 4 * (level + 1))}\n{argumentsStr}";
        }
    }
}

