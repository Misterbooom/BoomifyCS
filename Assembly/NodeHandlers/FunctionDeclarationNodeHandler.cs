using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomifyCS.Ast;
using BoomifyCS.Exceptions;
using LLVMSharp.Interop;
namespace BoomifyCS.Assembly.NodeHandlers
{
    class FunctionDeclarationNodeHandler : NodeHandler
    {
        public FunctionDeclarationNodeHandler(AssemblyCompiler compiler) : base(compiler) { }
        public override void HandleNode(AstNode node)
        {
            AstFunctionDecl astFunctionDecl = (AstFunctionDecl)node;
            string functionName = astFunctionDecl.functionNameNode.Token.Value;
            string functionTypeName = astFunctionDecl.typeNode.Token.Value;
            var pathChecker = FunctionPathChecker.CheckAllPathsReturn(astFunctionDecl);
            if (pathChecker.AllPathsReturn == false && functionTypeName != "void")
            {
                Traceback.Instance.SetCurrentLine(pathChecker.LastNode.LineNumber);
                Traceback.Instance.ThrowException(new BifyTypeError("Not all paths return a value"));
                return;
            }
            var llvmFunctionType = compiler.variableManager.AllocateFunction(functionName,functionTypeName);
            var functionType = LLVMTypeRef.CreateFunction(llvmFunctionType, new LLVMTypeRef[] { });
            var function = compiler.module.AddFunction(functionName,functionType);
            var entryBlock = function.AppendBasicBlock("entry");
            compiler.builder.PositionAtEnd(entryBlock);
            compiler.Visit(astFunctionDecl.blockNode);

            var blockNode = (AstBlock)astFunctionDecl.blockNode;
            bool hasNoReturn = blockNode.ChildNodes[^1] is not AstReturn;

            if (hasNoReturn && functionTypeName == "void")
            {
                compiler.builder.BuildRetVoid();
            }
            compiler.variableManager.ClearLocals();


        }
        
    }
}
public class FunctionPathChecker
{
    public static (bool AllPathsReturn, AstNode? LastNode) CheckAllPathsReturn(AstFunctionDecl functionDecl)
    {
        // Start the recursive check from the function's block node
        return CheckBlock(functionDecl.blockNode);
    }

    private static (bool AllPathsReturn, AstNode? LastNode) CheckBlock(AstNode? blockNode)
    {
        if (blockNode is null)
            return (false, null);

        if (blockNode is AstBlock block)
        {
            foreach (var statement in block.ChildNodes)
            {
                var (allPathsReturn, lastNode) = CheckStatement(statement);
                if (!allPathsReturn)
                    return (false, lastNode);
            }
            return (true, null);
        }

        return CheckStatement(blockNode);
    }

    private static (bool AllPathsReturn, AstNode? LastNode) CheckStatement(AstNode statement)
    {
        switch (statement)
        {
            case AstReturn:
                return (true, null);

            case AstIf ifNode:
                {
                    var (blockReturns, blockLastNode) = CheckBlock(ifNode.BlockNode);

                    foreach (var elseIf in ifNode.ElseIfNodes)
                    {
                        var (elseIfReturns, elseIfLastNode) = CheckBlock(elseIf.BlockNode);
                        if (!elseIfReturns)
                            return (false, elseIfLastNode);
                    }

                    if (ifNode.ElseNode != null)
                    {
                        var (elseReturns, elseLastNode) = CheckBlock(ifNode.ElseNode.BlockNode);
                        if (!elseReturns)
                            return (false, elseLastNode);
                    }
                    else if (!blockReturns)
                    {
                        return (false, blockLastNode);
                    }

                    return (blockReturns, blockLastNode);
                }

            //case AstWhile whileNode:
            //    return CheckBlock(whileNode.BlockNode);

            //case AstFor forNode:
            //    return CheckBlock(forNode.BlockNode);

            default:
                return (true, statement);
        }
    }
}