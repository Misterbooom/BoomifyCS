using BoomifyCS.Assembly.BifyObject;
using BoomifyCS.Assembly;
using BoomifyCS.Ast;
using BoomifyCS.Exceptions;
using System.ComponentModel.DataAnnotations;
using NUnit.Framework;

class FunctionPathChecker
{
    public bool AllPathsReturn = false;
    private BifyType _functionType;
    private AssemblyVariableManager _variableManager = AssemblyCompiler.Instance.variableManager;

    public FunctionPathChecker(BifyType functionType, AstNode node)
    {
        _functionType = functionType;
        AllPathsReturn = Check(node);
        if (!_functionType.CompareType(new VoidType()) && !AllPathsReturn)
        {
            Traceback.Instance.ThrowException(new BifyTypeError($"Not all execution paths return a value for {_functionType.Name}"));
        }
    }

    public bool Check(AstNode node)
    {
        Traceback.Instance.SetCurrentLine(node.LineNumber);
        if (node == null)
        {
            return false;
        }

        if (node is AstReturn astReturn)
        {
            if (astReturn.ArgumentsNode == null && !_functionType.CompareType(_variableManager.GetBifyType("void")))
            {
                Traceback.Instance.ThrowException(new BifyTypeError($"Expected {_functionType.Name} but got void"));
                return false;
            }
            return true;
        }
        else if (node is AstIf astIf)
        {
            bool ifReturns = Check(astIf.BlockNode);
            bool elseReturns = astIf.ElseNode?.BlockNode != null ? Check(astIf.ElseNode.BlockNode) : false;
            return ifReturns && elseReturns;
        }
        else if (node is AstBlock astBlock)
        {
            bool reachable = true;
            foreach (var child in astBlock.ChildNodes)
            {
                if (!reachable) continue;
                bool childReturns = Check(child);
                if (childReturns)
                {
                    reachable = false;
                }
            }
            return !reachable;
        }
        else
        {
            return false;
        }
    }
}