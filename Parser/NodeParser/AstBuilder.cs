using BoomifyCS.Ast;
using BoomifyCS.Lexer;
using System.Collections.Generic;

public static class AstBuilder
{
    public static AstNode TokenToAst(List<Token> tokens, string modulePath)
    {
        var ast = new AstTree(modulePath);
        return ast.ParseTokens(tokens);
    }

    public static AstNode TokenToAst(Token token, string modulePath)
    {
        var ast = new AstTree(modulePath);
        return ast.ParseTokens(new List<Token> { token });
    }

    public static AstNode BuildTokens(Token token, string modulePath)
    {
        var ast = new AstTree(modulePath);
        return ((AstModule)ast.ParseTokens(new List<Token> { token })).ChildNode;
    }

    public static AstNode BuildTokens(List<Token> tokens, string modulePath)
    {
        var ast = new AstTree(modulePath);
        return ((AstModule)ast.ParseTokens(tokens)).ChildNode;
    }
}
