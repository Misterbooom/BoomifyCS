using BoomifyCS.Ast;
using BoomifyCS.Lexer;
using System.Collections.Generic;

public static class AstBuilder
{
    public static AstNode TokenToAst(List<Token> tokens, string modulePath,ref int lineCount)
    {
        var ast = new AstTree(modulePath);
        ast.lineCount = lineCount;
        var resultTokens = ast.ParseTokens(tokens);
        lineCount = ast.lineCount;
        return resultTokens;

    }

    public static AstNode TokenToAst(Token token, string modulePath)
    {
        var ast = new AstTree(modulePath);
        return ast.ParseTokens([token]);
    }

    public static AstNode BuildTokens(Token token, string modulePath)
    {
        var ast = new AstTree(modulePath);
        return ((AstModule)ast.ParseTokens([token])).ChildNode;
    }

    public static AstNode BuildTokens(List<Token> tokens, string modulePath)
    {
        var ast = new AstTree(modulePath);
        return ((AstModule)ast.ParseTokens(tokens)).ChildNode;
    }
}
