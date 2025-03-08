using System;
using BoomifyCS.Assembly;
using BoomifyCS.Assembly.BifyObject;
using BoomifyCS.Ast;
using BoomifyCS.Exceptions;
using BoomifyCS.Lexer;
using LLVMSharp.Interop;
namespace BoomifyCS.Assembly.NodeHandlers
{
    class BinaryOpNodeHandler : NodeHandler
    {
        public BinaryOpNodeHandler(AssemblyCompiler compiler) : base(compiler) { }

        public override void HandleNode(AstNode node)
        {
            if (node is AstBinaryOp && node.Token.Type == TokenType.COMMA)
            {
                compiler.Visit(node.Left);
                compiler.Visit(node.Right);
                return;
            }
            
            compiler.Visit(node.Left);
            compiler.Visit(node.Right);
            BifyValue rhs = compiler.stack.Pop();
            BifyValue lhs = compiler.stack.Pop();
            BifyValue value = BinaryVal(lhs,rhs,node.Token.Type);
            compiler.stack.Push(value);
        }
        private BifyValue BinaryVal(BifyValue lhs,BifyValue rhs,TokenType type)
        {
            switch (type)
            {
                case TokenType.ADD:
                    return lhs.Add(rhs,compiler.builder);
                case TokenType.SUB:
                    return lhs.Sub(rhs, compiler.builder);
                case TokenType.MUL:
                    return lhs.Mul(rhs, compiler.builder);
                case TokenType.DIV:
                    return lhs.Div(rhs, compiler.builder);
                default:
                    throw new NotImplementedException("Not implemented binary operator.");
            }
        }
      
    }



}
