using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomifyCS.Assembly;
using BoomifyCS.Ast;
using BoomifyCS.Exceptions;
using BoomifyCS.Objects;
using LLVMSharp.Interop;
namespace BoomifyCS.Assembly.NodeHandlers
{
    class FunctionDeclarationNodeHandler : NodeHandler
    {
        public FunctionDeclarationNodeHandler(AssemblyCompiler compiler) : base(compiler) { }
        public override void HandleNode(AstNode node)
        {
            var astFunctionDecl = (AstFunctionDecl)node;
            string functionName = astFunctionDecl.functionNameNode.Token.Value;
            string functionTypeName = astFunctionDecl.typeNode.Token.Value;

            // Check if all paths return a value


            // Allocate and define the LLVM function
            var llvmFunctionType = compiler.variableManager.AllocateFunction(functionName, functionTypeName);
            var functionType = LLVMTypeRef.CreateFunction(llvmFunctionType, Array.Empty<LLVMTypeRef>());
            var function = compiler.module.AddFunction(functionName, functionType);

            // Create entry block and position the builder
            var entryBlock = function.AppendBasicBlock("entry");
            compiler.builder.PositionAtEnd(entryBlock);

            // Process the function body
            compiler.Visit(astFunctionDecl.blockNode);
            var pathChecker = new FunctionPathChecker(compiler.variableManager.GetVariable(functionTypeName).Type,compiler.variableManager).CheckAllPathsReturn(astFunctionDecl);
            if (!pathChecker.AllPathsReturn)
            {
                Traceback.Instance.SetCurrentLine(pathChecker.LastNode.LineNumber);
                Traceback.Instance.ThrowException(new BifyTypeError($"Function '{functionName}' does not return a value on all paths."));
                return;
            }

            // Ensure void functions have a return statement
            var blockNode = (AstBlock)astFunctionDecl.blockNode;
            if (blockNode.ChildNodes[^1] is not AstReturn && functionTypeName == "void")
            {
                compiler.builder.BuildRetVoid();
            }

            // Clear local variables
            compiler.variableManager.ClearLocals();
        }
    }
}
#nullable enable
class FunctionPathChecker(Type functionType, AssemblyVariableManager variableManager)
{
    private Type _functionType = functionType;
    private AssemblyVariableManager _variableManager = variableManager;
    private Type incorrectType = null;

    public (bool AllPathsReturn, AstNode? LastNode) CheckAllPathsReturn(AstFunctionDecl functionDecl)
    {
        return CheckBlock(functionDecl.blockNode);
    }

    private (bool AllPathsReturn, AstNode? LastNode) CheckBlock(AstNode? blockNode)
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

    private (bool AllPathsReturn, AstNode? LastNode) CheckStatement(AstNode statement)
    {
        switch (statement)
        {
            case AstReturn returnNode:
                if (returnNode.ArgumentsNode == null)
                {
                    // Check if a return value is required but missing
                    if (_functionType != typeof(BifyVoid))
                        return ThrowReturnTypeError(returnNode);
                }
                else if (!IsTypeCompatible(returnNode.ArgumentsNode))
                {
                    return ThrowReturnTypeError(returnNode);
                }
                return (true, null);

            case AstIf ifNode:
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

            // Add handling for loops or other constructs if needed
            // case AstWhile whileNode:
            // case AstFor forNode:

            default:
                return (true, statement);
        }
    }

    private (bool, AstNode?) ThrowReturnTypeError(AstReturn returnNode)
    {
        Traceback.Instance.SetCurrentLine(returnNode.LineNumber);
        Traceback.Instance.ThrowException(new BifyTypeError($"Invalid type of return value in function. Expected '{_functionType.Name.Replace("Bify", "")}', but got '{incorrectType.Name.Replace("Bify", "")}'."));
        return (false, returnNode);
    }

    private bool IsTypeCompatible(AstNode valueNode)
    {
        if (valueNode is AstConstant constant)
        {

            if (constant.BifyValue?.GetType() == _functionType)
            {
                return true;
            }
            incorrectType = constant.BifyValue?.GetType();
        }
        else if (valueNode is AstBinaryOp binaryOp)
        {
            var leftType = InferType(binaryOp.Left);
            var rightType = InferType(binaryOp.Right);

            if (leftType == rightType && leftType == _functionType)
            {
                return true;
            }
            incorrectType = leftType;
        }
        else if (valueNode is AstIdentifier identifier)
        {
            var variable = _variableManager.GetVariable(identifier.Name);
            if(variable.Type == _functionType)
            {
                return true;
            }
            incorrectType = variable.Type;
        }
        
        return false;
    }

    private Type? InferType(AstNode node)
    {
        return node switch
        {
            AstConstant constant => constant.BifyValue?.GetType(),
            AstIdentifier identifier => _variableManager.GetVariable(identifier.Name).Type,
            AstBinaryOp binaryOp => InferType(binaryOp.Left), // Assume left operand determines type
            _ => null
        };
    }
}
