using System;
using System.Collections.Generic;
using BoomifyCS.Lexer;

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
}
