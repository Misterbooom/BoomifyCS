﻿using System;
using System.Collections.Generic;
using System.Globalization;
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

        public override string ToString() => StrHelper();

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
        public override string ToString() => StrHelper();
    }

    public class AstConstant(Token token) : AstNode(token)
    {
        public BifyObject BifyValue;

        public override string StrHelper(int level = 0, string note = "", bool isLeft = true)
        {
            string baseStr = base.StrHelper(level, note);
            return baseStr + $"{new String(' ', 4 * (level + 1))}";
        }
        public override string ToString() => StrHelper();
    }

    public class AstNumber : AstConstant
    {
        public AstNumber(Token token, BifyObject bifyValue) : base(token)
        {
            this.BifyValue = bifyValue;
        }

        public override string StrHelper(int level = 0, string note = "", bool isLeft = true)
        {
            string baseStr = base.StrHelper(level, note);
            return baseStr + $"{new String(' ', 4 * (level + 1))}\n";
        }

        public override string ToString() => StrHelper();
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

        public override string ToString() => StrHelper();
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

        public override string ToString() => StrHelper();
    }
    public class AstNull : AstConstant
    {
        public AstNull(Token token) : base(token)
        {
            this.BifyValue = new BifyNull();
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
        public override string ToString() => StrHelper();
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
        public override string ToString() => StrHelper();
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
    public class AstBlock(List<AstNode> childsNodes) : AstNode(new Token(TokenType.BLOCK, "Block"))
    {

        public List<AstNode> ChildsNodes = childsNodes;

        public override string StrHelper(int level = 0, string note = "", bool isLeft = true)
        {
            string baseStr = base.StrHelper(level, note);
            string statementsStr = "";
            foreach (AstNode node in ChildsNodes)
            {
                statementsStr += node?.StrHelper(level + 1, "Statement: ") ?? "";

            }

            return baseStr + $"{new String(' ', 4 * (level + 1))}\n{statementsStr}";
        }
    }
    public class AstIf(Token token, AstNode conditionNode, AstNode blockNode, AstElse elseNode = null) : AstNode(token)
    {
        public AstNode ConditionNode = conditionNode;
        public AstNode BlockNode = blockNode;
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

        public override string ToString() => StrHelper();
        public void AddElseIfNode(AstElseIf astElseIf) => ElseIfNodes.Add(astElseIf);
    }
    public class AstElse(Token token, AstNode blockNode) : AstNode(token, blockNode)
    {
        public AstNode BlockNode = blockNode;

        public override string StrHelper(int level = 0, string note = "", bool isLeft = true)
        {
            string baseStr = base.StrHelper(level, note);
            string blockStr = BlockNode?.StrHelper(level + 1, "block: ");
            return baseStr + $"{new String(' ', 4 * (level + 1))}\n{blockStr}";
        }
    }
    public class AstElseIf(Token token, AstNode blockNode, AstNode conditionNode) : AstNode(token)
    {
        public AstNode BlockNode = blockNode;
        public AstNode ConditionNode = conditionNode;

        public override string StrHelper(int level = 0, string note = "", bool isLeft = true)
        {
            string baseStr = base.StrHelper(level, note);
            string conditionStr = ConditionNode?.StrHelper(level + 1, "Condition: ");
            string blockStr = BlockNode?.StrHelper(level + 1, "Block: ");
            return baseStr + $"{new String(' ', 4 * (level + 1))}\n{conditionStr}\n{blockStr}";

        }
    }
    public class AstWhile(Token token, AstNode blockNode, AstNode conditionNode) : AstNode(token)
    {
        public AstNode BlockNode = blockNode;
        public AstNode ConditionNode = conditionNode;

        public override string StrHelper(int level = 0, string note = "", bool isLeft = true)
        {
            string baseStr = base.StrHelper(level, note);
            string conditionStr = ConditionNode?.StrHelper(level + 1, "Condition: ");
            string blockStr = BlockNode?.StrHelper(level + 1, "Block: ");
            return baseStr + $"{new String(' ', 4 * (level + 1))}\n{conditionStr}\n{blockStr}";

        }
    }
    public class AstFor(Token token, AstNode blockNode, AstNode conditionNode, AstNode incrementNode, AstNode initNode) : AstNode(token)
    {
        public AstNode BlockNode = blockNode;
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
            string argumentsStr = ArgumentsNode != null ? ArgumentsNode?.StrHelper(level + 2, "Arguments: ") : "";
            return baseStr + $"{new String(' ', 4 * (level + 1))}\n{argumentsStr}";

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
    public class AstFunctionDecl(Token token, AstIdentifier functionNameNode, AstNode argumentsNode, AstNode blockNode) : AstNode(token)
    {
        public AstNode argumentsNode = argumentsNode;
        public AstIdentifier functionNameNode = functionNameNode;
        public AstNode blockNode = blockNode;

        public override string StrHelper(int level = 0, string note = "", bool isLeft = true)
        {
            string baseStr = base.StrHelper(level, note, false);
            string functionNameStr = functionNameNode?.StrHelper(level + 1, "Name: ");
            string argumentsStr = argumentsNode?.StrHelper(level + 1, "Arguments: ");
            string blockStr = blockNode?.StrHelper(level + 1, "Block: ");
            return baseStr + $"{new String(' ', 4 * (level + 1))}\n{functionNameStr}\n{argumentsStr}\n{blockStr}";
        }
    }
    public class AstModule(string moduleName, string modulePath, List<AstNode> nodes) : AstNode(new Token(TokenType.IDENTIFIER, moduleName))
    {
        public List<AstNode> ChildNodes = nodes;
        public string ModuleName = moduleName;
        public string ModulePath = modulePath;

        public override string StrHelper(int level = 0, string note = "", bool isLeft = true)
        {
            string baseStr = base.StrHelper(level, note, false);
            string childStr = "";
            foreach (AstNode node in ChildNodes)
            {
                childStr += node?.StrHelper(level + 1, "Child: ");

            }
            return baseStr + $"{new String(' ', 4 * (level + 1))}\n{childStr}";

        }
    }
    public class AstAssignmentOperator(Token token, AstNode identifierNode, AstNode valueNode) : AstNode(token)
    {
        public AstNode IdentifierNode = identifierNode;
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
    public class AstReturn(Token token, AstNode argumentsNode) : AstNode(token)
    {
        public AstNode ArgumentsNode = argumentsNode;
        public override string StrHelper(int level = 0, string note = "", bool isLeft = true)
        {
            string baseStr = base.StrHelper(level, note, false);
            string argumentsStr = ArgumentsNode?.StrHelper(level + 1, "Arguments: ");
            return baseStr + $"{new String(' ', 4 * (level + 1))}\n{argumentsStr}";
        }
    }
    public class AstArray(Token token, AstNode argumentsNode) : AstNode(token)
    {
        public AstNode ArgumentsNode = argumentsNode;
        public override string StrHelper(int level = 0, string note = "", bool isLeft = true)
        {
            string baseStr = base.StrHelper(level, note, false);
            List<AstNode> nodes = UnpackCommaNode(ArgumentsNode);
            string argumentsStr = "";
            foreach (var node in nodes)
            {
                argumentsStr += node?.StrHelper(level + 1, "Argument: ");
            }
            return baseStr + $"{new String(' ', 4 * (level + 1))}\n{argumentsStr}";
        }
        private List<AstNode> UnpackCommaNode(AstNode node)
        {
            List<AstNode> nodes = new List<AstNode>();
            if (node is AstBinaryOp binaryOp && binaryOp.Token.Type == TokenType.COMMA)
            {
                nodes.AddRange(UnpackCommaNode(binaryOp.Left));
                nodes.AddRange(UnpackCommaNode(binaryOp.Right));
            }
            else
            {
                nodes.Add(node);
            }
            return nodes;
        }
    }
    public class AstIndexOperator(AstNode nodeIndex, AstNode operandNode) : AstNode(new Token(TokenType.NUMBER, "UNKNOWN"))
    {
        public AstNode IndexNode = nodeIndex;
        public AstNode OperandNode = operandNode;
        public override string StrHelper(int level = 0, string note = "", bool isLeft = true)
        {
            string baseStr = base.StrHelper(level, note, isLeft);
            string nodeIndexStr = IndexNode?.StrHelper(level + 1, "IndexNode: ");
            string operandStr = OperandNode?.StrHelper(level + 1, "Operand: ");
            return baseStr + $"{new String(' ', 4 * (level + 1))}\n{nodeIndexStr}\n{operandStr}";
        }
    }
    public class AstConditionStatement(AstNode left = null, AstNode right = null) : AstNode(new Token(TokenType.IDENTIFIER, "Condition statement"), left, right)
    {

    }
    public class AstRangeOperator(Token token) : AstBinaryOp(token)
    {
    }
}

