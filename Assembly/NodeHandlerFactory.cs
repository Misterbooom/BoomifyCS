using BoomifyCS.Ast;
using BoomifyCS.Assembly.NodeHandlers;

using System.Data;
using BoomifyCS.Exceptions;
using BoomifyCS.Assembly.NodeHandlers.ClassHandler;

namespace BoomifyCS.Assembly
{
    class NodeHandlerFactory
    {
        public static NodeHandler CreateHandler(AstNode node, AssemblyCompiler compiler)
        {
        
            var conditionStatementHanlder = new ConditionStatementNodeHandler(compiler);
            var loopNodeHandler = new LoopNodeHandler(compiler);
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
                AstCall astCall => new CallNodeHandler(compiler),
                AstAssignmentOperator astAssignmentOperator => new AssignmentOperatorNodeHandler(compiler),
                AstIf => conditionStatementHanlder,
                AstElse => conditionStatementHanlder,
                AstElseIf => conditionStatementHanlder,
                AstFor => loopNodeHandler,
                AstWhile => loopNodeHandler,
                AstUnaryOperator => new UnaryOperatorNodeHandler(compiler),
                AstIndexOperator => new IndexOperatorNodeHandler(compiler),
                AstArray => new ArrayNodeHandler(compiler),
                AstBreak or AstContinue => new BreakContinueNodeHandler(compiler),
                AstClass => new ClassNodeHandler(compiler),
                AstCast => new CastNodeHandler(compiler),
                AstNew => new NewNodeHandler(compiler),
                AstMemberAccess => new MemberAccessNodeHandler(compiler),
                _ => throw new SyntaxErrorException($"Unhandled node - {node.GetType().Name}")
            };
        }
    }
}
