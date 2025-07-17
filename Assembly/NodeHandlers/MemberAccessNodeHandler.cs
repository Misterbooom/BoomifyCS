using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomifyCS.Assembly.BifyObject;
using BoomifyCS.Ast;
using BoomifyCS.Exceptions;

namespace BoomifyCS.Assembly.NodeHandlers
{
    class MemberAccessNodeHandler:NodeHandler
    {
        public MemberAccessNodeHandler(AssemblyCompiler compiler) : base(compiler) { }
        public override void HandleNode(AstNode node)
        {
            if (node.Right is not AstIdentifier)
            {
                Traceback.Instance.ThrowException(new BifyTypeError("Invalid member access. Expected identifier."));
                return;
            }
            bool loadMemberPointer = !compiler.Flag.HasFlag(NodeVisitFlag.ASSIGNMENT_INDEX);
            compiler.Flag &= ~NodeVisitFlag.ASSIGNMENT_INDEX;
            compiler.Visit(node.Left);
            IValue iValue = compiler.StackIValuePop();
            if (iValue is BifyType)
            {
                Traceback.Instance.ThrowException(new BifyTypeError("Invalid operand: type provided instead of value"));
                return;
            }
            BifyValue bifyValue = (BifyValue)iValue;
            string memberName = ((AstIdentifier)node.Right).Name;
            BifyValue memberValue = bifyValue.GetAttribute(memberName,compiler.CurrentClass, compiler.Builder);
            if (!loadMemberPointer)
            {
                compiler.StackPush(memberValue);

            }
            else {
                if (memberValue is PointerValue pointerValue)
                {
                    compiler.StackPush(pointerValue.Dereference());
                }
                else if (memberValue is BifyMethodRef)
                {
                    compiler.StackPush(memberValue);
                }
                else
                {
                    throw new InvalidOperationException($"Unexpected type of class member: '{memberValue.GetType()}'");
                }

            }
        }
    }
}
