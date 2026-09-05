using System;
using System.Collections.Generic;
using BoomifyCS.Lexer;

namespace BoomifyCS.Ast
{
    public class AstNode(Token token, AstNode left = null, AstNode right = null)
    {
        public Token Token { get; set; } = token;
        public AstNode Left { get; set; } = left;
        public AstNode Right { get; set; } = right;
        private int _lineNumber;
        public int LineNumber
        {

            get
            {
                if (_lineNumber == 0)
                {
                    return Token.Line;
                }
                return _lineNumber;
            }
            set
            {
                _lineNumber = value;
            }

        }

        public override string ToString() => StrHelper();

        public virtual string StrHelper(int level = 0, string note = "", bool isLeft = true)
        {
            string indent = new(' ', 4 * level);
            string branch = isLeft ? "|- " : "|- ";
            string treeStr = $"{indent}{branch}{note}{GetType().Name}('{Token.Value}',Line - {LineNumber})\n";

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
        public virtual int Len(int level = 1)
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

    public abstract class AstConstant(Token token, object value) : AstNode(token)
    {
        public object Value { get; protected set; } = value;


        public override string ToString() => StrHelper();
    }

    public class AstNumber(Token token, int value) : AstConstant(token, value);

    public class AstString(Token token, string value) : AstConstant(token, value);

    public class AstBoolean(Token token, bool value) : AstConstant(token, value);

    public class AstNull(Token token) : AstConstant(token, null);
    public class AstFloat(Token token, float value) : AstConstant(token, value);


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

    public class AstVarDecl(Token token, AstAssignment assignmentNode, AstNode type = null, AstFlag flagNode = null, AstNode left = null, AstNode right = null) : AstNode(token, left, right)
    {
        public readonly AstAssignment AssignmentNode = assignmentNode;
        public readonly AstNode Type = type;
        public readonly AstFlag Flag = flagNode;

        public override string StrHelper(int level = 0, string note = "", bool isLeft = true)
        {
            string baseStr = base.StrHelper(level, note);
            string assignmentStr = AssignmentNode?.StrHelper(level + 1, "Assignment:") ?? "";
            string type = this.Type?.StrHelper(level + 1, "Type: ") ?? "";
            string flag = this.Flag?.StrHelper(level + 1, "Flag: ") ?? "";
            return baseStr + $"{new String(' ', 4 * (level + 1))}\n{assignmentStr}{type}{flag}";
        }
        public override string ToString() => StrHelper();
    }
    public class AstLine(AstNode child) : AstNode(new Token(TokenType.EOL, ";"))
    {
        public readonly AstNode Child = child;

        public override string StrHelper(int level = 0, string note = "", bool isLeft = true)
        {
            string baseStr = base.StrHelper(level, note, isLeft);
            string childStr = Child?.StrHelper(level + 1, "Child: ");
            return baseStr + $"{new String(' ', 4 * (level + 1))}\n{childStr}";

        }

    }
    public class AstIdentifier(Token token, string name) : AstNode(token)
    {
        public readonly string Name = name;
    }
    public class AstBracket(Token token) : AstNode(token)
    {
    }
    public class AstEol(Token token) : AstNode(token)
    {
    }
    public class AstBlock(List<AstNode> childsNodes) : AstNode(new Token(TokenType.BLOCK, "Block"))
    {

        public readonly List<AstNode> ChildNodes = childsNodes;

        public override string StrHelper(int level = 0, string note = "", bool isLeft = true)
        {
            string baseStr = base.StrHelper(level, note);
            string statementsStr = "";
            foreach (AstNode node in ChildNodes)
            {
                statementsStr += node?.StrHelper(level + 1, "Statement: ") ?? "";

            }

            return baseStr + $"{new String(' ', 4 * (level + 1))}\n{statementsStr}";
        }
        public override int Len(int level = 1)
        {
            return ChildNodes.Count;
        }
    }
    public class AstIf(Token token, AstNode conditionNode, AstNode blockNode, AstElse elseNode = null) : AstNode(token)
    {
        public readonly AstNode ConditionNode = conditionNode;
        public readonly AstNode BlockNode = blockNode;
        public AstElse ElseNode = elseNode;
        public readonly List<AstElseIf> ElseIfNodes = [];

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
    public class AstElse(Token token, AstNode blockNode) : AstNode(token)
    {
        public readonly AstNode BlockNode = blockNode;

        public override string StrHelper(int level = 0, string note = "", bool isLeft = true)
        {
            string baseStr = base.StrHelper(level, note);
            string blockStr = BlockNode?.StrHelper(level + 1, "block: ");
            return baseStr + $"{new String(' ', 4 * (level + 1))}\n{blockStr}";
        }
    }
    public class AstElseIf(Token token, AstNode blockNode, AstNode conditionNode) : AstNode(token)
    {
        public readonly AstNode BlockNode = blockNode;
        public readonly AstNode ConditionNode = conditionNode;

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
        public readonly AstNode BlockNode = blockNode;
        public readonly AstNode ConditionNode = conditionNode;

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
        public readonly AstNode BlockNode = blockNode;
        public readonly AstNode ConditionNode = conditionNode;
        public readonly AstNode IncrementNode = incrementNode;
        public readonly AstNode InitNode = initNode;

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
        public readonly AstNode CallableName = callableName;
        public readonly AstNode ArgumentsNode = argumentsNode;

        public override string StrHelper(int level = 0, string note = "", bool isLeft = true)
        {
            string baseStr = base.StrHelper(level, note);
            string argumentsStr = ArgumentsNode != null ? ArgumentsNode?.StrHelper(level + 2, "Arguments: ") : "";
            string callableNameStr = CallableName?.StrHelper(level + 1, "Callable: ");
            return baseStr + $"{new String(' ', 4 * (level + 1))}\n{callableNameStr}{argumentsStr}";

        }
    }
    public class AstUnaryOperator(Token token, AstNode value, bool isPrefix = false) : AstNode(token)
    {
        public readonly AstNode Operand = value;
        public readonly bool IsPrefix = isPrefix;

        public override string StrHelper(int level = 0, string note = "", bool isLeft = true)
        {
            string baseStr = base.StrHelper(level, note);
            string valueStr = value?.StrHelper(level + 1, "Value: ");
            return baseStr + $"{new String(' ', 4 * (level + 1))}\n{valueStr}";
        }
        public AstUnaryOperator Update(AstNode newOperand)
        {
            return new AstUnaryOperator(this.Token, newOperand, isPrefix);
        }
    }
    public class AstFunctionDecl(Token token, AstNode typeNode, AstIdentifier functionNameNode, AstNode argumentsNode, AstBlock blockNode,AstFlag flagNode) : AstNode(token)
    {
        public readonly AstNode ArgumentsNode = argumentsNode;
        public readonly AstIdentifier FunctionNameNode = functionNameNode;
        public readonly AstBlock BlockNode = blockNode;
        public readonly AstNode TypeNode = typeNode;
        public readonly AstFlag FlagNode = flagNode;

        public override string StrHelper(int level = 0, string note = "", bool isLeft = true)
        {
            string baseStr = base.StrHelper(level, note, false);
            string functionNameStr = FunctionNameNode?.StrHelper(level + 1, "Name: ");
            string argumentsStr = ArgumentsNode?.StrHelper(level + 1, "Arguments: ");
            string blockStr = BlockNode?.StrHelper(level + 1, "Block: ");
            string typeStr = TypeNode?.StrHelper(level + 1, "Type: ");
            return baseStr + $"{new String(' ', 4 * (level + 1))}\n{typeStr}\n{functionNameStr}\n{argumentsStr}\n{blockStr}";
        }
    }
    public class AstModule(string moduleName, string modulePath, List<AstNode> nodes) : AstNode(new Token(TokenType.IDENTIFIER, moduleName))
    {
        public readonly List<AstNode> ChildNodes = nodes;
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
        public readonly AstNode IdentifierNode = identifierNode;
        public readonly AstNode ValueNode = valueNode;

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
        public readonly AstNode ArgumentsNode = argumentsNode;
        public override string StrHelper(int level = 0, string note = "", bool isLeft = true)
        {
            string baseStr = base.StrHelper(level, note, false);
            string argumentsStr = ArgumentsNode?.StrHelper(level + 1, "Arguments: ");
            return baseStr + $"{new String(' ', 4 * (level + 1))}\n{argumentsStr}";
        }
    }
    public class AstArray(Token token, AstNode argumentsNode) : AstNode(token)
    {
        public readonly AstNode ArgumentsNode = argumentsNode;
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
            List<AstNode> nodes = new();
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
        public readonly AstNode IndexNode = nodeIndex;
        public readonly AstNode TargetNode = operandNode;
        public override string StrHelper(int level = 0, string note = "", bool isLeft = true)
        {
            string baseStr = base.StrHelper(level, note, isLeft);
            string nodeIndexStr = IndexNode?.StrHelper(level + 1, "IndexNode: ");
            string operandStr = TargetNode?.StrHelper(level + 1, "Operand: ");
            return baseStr + $"{new String(' ', 4 * (level + 1))}\n{nodeIndexStr}\n{operandStr}";
        }
    }
    public class AstParam(AstNode type, AstNode name, AstNode flag) : AstNode(new Token(TokenType.AND, "Param"))
    {
        public readonly AstNode Type = type;
        public readonly AstNode Name = name;
        public readonly AstNode Flag = flag;
        public override string StrHelper(int level = 0, string note = "", bool isLeft = true)
        {
            string baseStr = base.StrHelper(level, note);
            string typeStr = Type?.StrHelper(level + 1, "Type: ");
            string nameStr = Name?.StrHelper(level + 1, "Name: ");
            string flagStr = Flag?.StrHelper(level + 1, "Flag: ");
            return baseStr + $"{new String(' ', 4 * (level + 1))}\n{typeStr}\n{nameStr}\n{flagStr}";
        }
    }
    public class AstConditionStatement(AstNode left = null, AstNode right = null) : AstNode(new Token(TokenType.IDENTIFIER, "Condition statement"), left, right)
    {

    }
    public class AstRangeOperator(Token token) : AstBinaryOp(token)
    {
    }
    public class AstClass(Token token, AstNode nameNode, AstNode bodyNode) : AstNode(token)
    {
        public readonly AstNode NameNode = nameNode;
        public readonly AstNode BodyNode = bodyNode;
        public override string StrHelper(int level = 0, string note = "", bool isLeft = true)
        {
            string nameStr = NameNode?.StrHelper(level + 1, "Name: ");
            string bodyStr = BodyNode?.StrHelper(level + 1, "Body: ");
            string baseStr = base.StrHelper(level, note);
            return baseStr + $"{new String(' ', 4 * (level + 1))}\n{nameStr}\n{bodyStr}";

        }
    }
    public class AstCast(Token token, AstNode typeNode, AstNode valueNode) : AstNode(token)
    {
        public readonly AstNode TypeNode = typeNode;
        public readonly AstNode ValueNode = valueNode;
        public override string StrHelper(int level = 0, string note = "", bool isLeft = true)
        {
            string typeStr = TypeNode?.StrHelper(level + 1, "Type: ");
            string valueStr = ValueNode?.StrHelper(level + 1, "Value: ");
            string baseStr = base.StrHelper(level, note);
            return baseStr + $"{new String(' ', 4 * (level + 1))}\n{typeStr}\n{valueStr}";


        }
    }
    public class AstNew(Token token, AstNode valueNode, AstNode arguments) : AstNode(token)
    {
        public readonly AstNode ValueNode = valueNode;
        public readonly AstNode ArgumentsNode = arguments;
        public override string StrHelper(int level = 0, string note = "", bool isLeft = true)
        {
            string baseStr = base.StrHelper(level, note);
            string valueStr = ValueNode?.StrHelper(level + 1, "Value: ");
            string argumentsStr = ArgumentsNode?.StrHelper(level + 1, "Arguments: ");
            return baseStr + $"{new String(' ', 4 * (level + 1))}\n{valueStr}\n{argumentsStr}";
        }
    }
    public class AstMemberAccess(Token token, AstNode left, AstNode right) : AstNode(token, left, right) { }
    public class AstFlag(Token token,List<AstNode> flags): AstNode(token)
    {
        public readonly List<AstNode> Flags = flags;
        public bool HasFlag(string name)
        {
            foreach (AstNode flag in Flags)
            {
                if (flag is AstIdentifier identifier && identifier.Name == name)
                {
                    return true;
                }
            }
            return false;
        }
        public override string StrHelper(int level = 0, string note = "", bool isLeft = true)
        {
            string baseStr = base.StrHelper(level, note);
            string flagsStr = "";
            foreach (var flag in Flags)
            {
                flagsStr += flag?.StrHelper(level + 1, "Flag: ");
            }
            return baseStr + $"{new String(' ', 4 * (level + 1))}\n{flagsStr}";
        }

    }

}

