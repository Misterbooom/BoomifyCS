using BoomifyCS.Exceptions;
using BoomifyCS.Lexer;

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
                        if (float.TryParse(token.Value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float parsedFloatValue))
                        {
                            if (parsedFloatValue > float.MaxValue)
                            {
                                Traceback.Instance.ThrowException(new BifyOverflowError($"Token value '{token.Value}' exceeds the maximum allowable value for a double."));
                                return null;
                            }
                            return new AstFloat(token, parsedFloatValue);
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
                            return new AstNumber(token, parsedIntValue);
                        }
                        else
                        {
                            Traceback.Instance.ThrowException(new BifyOverflowError($"Token value '{token.Value}' is not a valid integer."));
                            return null;
                        }
                    }


                case TokenType.IDENTIFIER or TokenType.CONST:
                    return new AstIdentifier(token,token.Value);
                case TokenType.STRING or TokenType.CHAR:
                    return new AstString(token, token.Value);
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
