using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomifyCS.Exceptions;
using BoomifyCS.Lexer;
using BoomifyCS.Objects;

namespace BoomifyCS.Ast
{
    class NodeConventer
    {
        public static AstNode TokenToNode(Token token)
        {
            switch (token.Type)
            {
                case TokenType.NUMBER:
                    if (token.Value.Contains('.'))
                    {
                        if (double.TryParse(token.Value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out double parsedFloatValue))
                        {
                            if (parsedFloatValue > double.MaxValue)
                            {
                                Traceback.Instance.ThrowException(new BifyOverflowError($"Token value '{token.Value}' exceeds the maximum allowable value for a double."));
                                return null;
                            }
                            return new AstNumber(token, new BifyFloat(parsedFloatValue));
                        }
                        else
                        {
                            Traceback.Instance.ThrowException(new BifyOverflowError($"Token value '{token.Value}' is not a valid float."));
                            return null;
                        }
                    }
                    else
                    {
                        if (int.TryParse(token.Value, out int parsedIntValue))
                        {
                            if (parsedIntValue > int.MaxValue)
                            {
                                Traceback.Instance.ThrowException(new BifyOverflowError($"Token value '{token.Value}' exceeds the maximum allowable value for a double."));
                                return null;
                            }
                            return new AstNumber(token, new BifyInteger(parsedIntValue));
                        }
                        else
                        {
                            Traceback.Instance.ThrowException(new BifyOverflowError($"Token value '{token.Value}' is not a valid integer."));
                            return null;
                        }
                    }


                case TokenType.IDENTIFIER:
                    return new AstIdentifier(token,token.Value);
                case TokenType.STRING:
                    return new AstString(token, new BifyString(token.Value));
                case TokenType.BREAK:
                    return new AstBreak(token);
                case TokenType.CONTINUE:
                    return new AstContinue(token);
                default: return null;
            }
        }
        public static int CountCommaNode(AstNode node)
        {
            if (node == null)
                return 0;

            int count = (node.Token != null && node.Token.Type == TokenType.COMMA) ? 1 : 0;

            count += CountCommaNode(node.Left);
            count += CountCommaNode(node.Right);

            return count;
        }


    }
}
