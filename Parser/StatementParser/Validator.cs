using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomifyCS.Lexer;
using BoomifyCS.Exceptions;
using BoomifyCS.Ast;
namespace BoomifyCS.Parser.StatementParser
{
    public class  ForLoopValidator
    {
        public static void ValidateForLoopHeader(List<List<Token>> bracketTokensSplit)
        {
            if (bracketTokensSplit.Count != 3)
            {
                throw new BifyInitializationError(ErrorMessage.InvalidInitializationStatement());
            }

            if (bracketTokensSplit[0].Count == 0)
            {
                throw new BifyInitializationError(ErrorMessage.ForLoopMustHaveInitialization());
            }

            if (bracketTokensSplit[1].Count == 0)
            {
                throw new BifyInitializationError(ErrorMessage.ForLoopMustHaveCondition());
            }

            if (bracketTokensSplit[2].Count == 0)
            {
                throw new BifyInitializationError(ErrorMessage.ForLoopMustHaveIncrement());
            }
        }

        public static void ValidateForLoop(AstNode initNode, AstNode conditionNode, AstNode incrementNode)
        {
            if (incrementNode is not AstAssignmentOperator && incrementNode is not AstUnaryOperator && incrementNode is not AstCall)
            {
                throw new BifyInitializationError(ErrorMessage.InvalidIncrementExpression());
            }

            if (initNode is not AstVarDecl)
            {
                throw new BifyInitializationError(ErrorMessage.InvalidVariableDeclaration());
            }

            if (conditionNode is not AstBinaryOp)
            {
                throw new BifyInitializationError(ErrorMessage.InvalidConditionExpression());
            }
        }
        
    }
}
