using System;
using BoomifyCS.Ast;
using BoomifyCS.Lexer;
using BoomifyCS.Assembly.NodeHandlers;

using System.Data;
using BoomifyCS.Exceptions;

namespace BoomifyCS.Assembly
{
    class NodeHandlerFactory
    {
        public static NodeHandler CreateHandler(AstNode node, AssemblyCompiler compiler)
        {
            Traceback.Instance.SetCurrentLine(node.LineNumber);
            return node switch
            {
                AstModule astModule => new ModuleHandler(compiler), 
                AstFunctionDecl astFunctionDecl => new FunctionDeclarationNodeHandler(compiler),
                AstVarDecl astVarDecl => new VariableDeclarationNodeHandler(compiler),
                AstBinaryOp astBinaryOp => new BinaryOpNodeHandler(compiler),
                AstBlock astBlock => new BlockNodeHandler(compiler),
                AstConstant astConstant => new ConstantNodeHandler(compiler),
                AstReturn astReturn => new ReturnNodeHandler(compiler),
                AstIdentifier astIdentifier => new IdentifierNodeHandler(compiler),
                _ => throw new SyntaxErrorException($"Unhandled node - {node.GetType().Name}") 
            };
        }
    }
}
