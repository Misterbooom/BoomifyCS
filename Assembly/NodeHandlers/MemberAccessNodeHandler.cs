using System;
using BoomifyCS.Assembly.BifyObject;
using BoomifyCS.Ast;
using BoomifyCS.Exceptions;

namespace BoomifyCS.Assembly.NodeHandlers
{
    internal class MemberAccessNodeHandler(AssemblyCompiler compiler) : NodeHandler(compiler)
    {
        public override void HandleNode(AstNode node)
        {
            if (node.Right is not AstIdentifier identifier)
            {
                Traceback.Instance.ThrowException(new BifyTypeError("Invalid member access. Expected identifier."));
                return;
            }
            bool loadMemberPointer = !Compiler.Flag.HasFlag(NodeVisitFlag.ASSIGNMENT_INDEX);
            Compiler.Flag &= ~NodeVisitFlag.ASSIGNMENT_INDEX;
            Compiler.Visit(node.Left);
            IValue iValue = Compiler.StackIValuePop();
            if (iValue is BifyType)
            {
                Traceback.Instance.ThrowException(new BifyTypeError("Invalid operand: type provided instead of value"));
                return;
            }
            BifyValue bifyValue = (BifyValue)iValue;
            string memberName = identifier.Name;
            BifyValue memberValue = bifyValue.GetAttribute(memberName,Compiler.CurrentClass, Compiler.Builder);
            if (!loadMemberPointer)
            {
                Compiler.StackPush(memberValue);

            }
            else
            {
                switch (memberValue)
                {
                    case PointerValue pointerValue:
                        Compiler.StackPush(pointerValue.Dereference(true));
                        break;
                    case BifyMethodRef:
                        Compiler.StackPush(memberValue);
                        break;
                    default:
                        throw new InvalidOperationException($"Unexpected type of class member: '{memberValue.GetType()}'");
                }
            }
        }
    }
}
