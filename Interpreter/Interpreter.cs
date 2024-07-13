using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomifyCS.Ast;
using BoomifyCS.Objects;
using BoomifyCS.Parser;
namespace BoomifyCS.Interpreter
{
    public class BetaInterpreter
    {
        public Hashtable hashTable = new Hashtable();
        public BetaInterpreter() 
        {
        
        }
        public void run(AstNode node)
        {
            Visit(node);
            Console.WriteLine(hashTable.ToCustomString());
        }
        public object Visit(AstNode node)
        {
            if (node == null) 
            {
                return null;
            }
            else if (node is AstBinaryOp)
            {
                object left = Visit(node.Left);
                object right = Visit(node.Right);
                BifyObject result = BifyObjectParser.CalculateBifyObjects((BifyObject)left, (BifyObject)right, node.Token.Type);
                return result;


            }
            else if (node is AstVarDecl varDeclNode)
            {
                return Visit(varDeclNode.AssignmentNode);
            }
            else if (node is AstAssignment)
            {
                BifyObject value = Visit(node.Right) as BifyObject;
                hashTable[node.Left.Token.Value] = value;
            }
            else if (node is AstConstant)
            {
                return (node as AstConstant).BifyValue;
            }
            return node;
        }
    }
}
