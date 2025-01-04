using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomifyCS.Exceptions;

namespace BoomifyCS.Ast.Validators
{
    class ReturnValidator
    {
        public static void Validate(AstReturn node)
        {
            if (node.ArgumentsNode == null)
            {
                return;
            }   
            if (!CanBeReturned(node.ArgumentsNode))
            {
                Traceback.Instance.ThrowException(new BifyTypeError($"'{node.GetType().Name.Replace("Ast", "")}' Cannot be returned",
                    "", node.Token.Value));
            }



        }
        private static bool CanBeReturned(AstNode node)
        {
            return node is AstConstant || node is AstIdentifier ||
                node is AstBinaryOp binaryOp &&
                binaryOp.Token.Type != Lexer.TokenType.COMMA ||
                node is AstCall;
        }

    }
}
