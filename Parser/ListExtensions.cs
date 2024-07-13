using System;
using System.Collections.Generic;
using BoomifyCS.Lexer;
using BoomifyCS.Ast;
using System.Reflection.Emit;
using System.Text;
using System.Collections;
public static class ListExtensions
{
    public static T Pop<T>(this List<T> list)
    {
        if (list == null)
        {
            throw new ArgumentNullException(nameof(list));
        }

        if (list.Count == 0)
        {
            throw new InvalidOperationException("List is empty.");
        }

        T poppedItem = list[list.Count - 1];

        list.RemoveAt(list.Count - 1);

        return poppedItem;
    }

    public static void WriteTokens<T>(this List<T> list) where T : Token
    {
        string result = "";
        foreach (var token in list)
        {
            result += token.ToString() + "\n";
        }
        Console.WriteLine(result);
    }
    public static string TokensToString<T>(this List<T> list) where T : Token
    {
        string result = "";
        foreach (var token in list)
        {
            result += token.ToString() + "\n";
        }
        return result;
    }

    public static void WriteTokensWithoutWhiteSpace<T>(this List<T> list) where T : Token
    {
        string result = "";
        foreach (var token in list)
        {
            if (token.Type == TokenType.WHITESPACE)
            {
                continue;
            }
            result += token.ToString() + "\n";
        }
        Console.WriteLine(result);
    }
    public static void WriteNodes<T>(this List<T> list) where T : AstNode
    {
        Console.Write('[');
        foreach (var node in list)
        {
            if (node != null)
            {
                Console.WriteLine(node.ToString());

            }
        }
        Console.Write(']');

    }
    public static void WriteNodes<T>(this Stack<T> list) where T : AstNode
    {
        Console.Write('[');
        foreach (var node in list)
        {
            if (node != null)
            {
                Console.WriteLine(node.ToString());

            }
        }
        Console.Write(']');

    }
    public static string ToCustomString(this Hashtable table)
    {
        var sb = new StringBuilder();
        foreach (DictionaryEntry kvp in table)
        {
            sb.Append($"{kvp.Key}:{kvp.Value}, ");
        }
        if (sb.Length > 2)
        {
            sb.Length -= 2; 
        }
        return sb.ToString();
    }
}
