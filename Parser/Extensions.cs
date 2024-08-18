using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BoomifyCS.Ast;
using BoomifyCS.Compiler;
using BoomifyCS.Lexer;

namespace BoomifyCS.Parser
{
    public static class Extensions
    {
        public static T Pop<T>(this List<T> list)
        {
            ArgumentNullException.ThrowIfNull(list);

            if (list.Count == 0)
            {
                throw new InvalidOperationException("List is empty.");
            }

            T poppedItem = list[^1];
            list.RemoveAt(list.Count - 1);

            return poppedItem;
        }

        public static void WriteTokens(this List<Token> list)
        {
            Console.WriteLine("[");
            foreach (var token in list)
            {
                Console.WriteLine(token.ToString());
            }
            Console.WriteLine("]");
        }

        public static string TokensToString(this List<Token> list)
        {
            var sb = new StringBuilder();
            foreach (var token in list)
            {
                sb.Append(token.Value);
            }
            return sb.ToString();
        }

        public static void WriteNodes(this List<AstNode> list)
        {
            Console.WriteLine("[");
            foreach (var node in list)
            {
                if (node != null)
                {
                    Console.WriteLine(node.ToString());
                }
            }
            Console.WriteLine("]");
        }

        public static void WriteNodes(this Stack<AstNode> stack)
        {
            Console.WriteLine("[");
            foreach (var node in stack)
            {
                if (node != null)
                {
                    Console.WriteLine(node.ToString());
                }
            }
            Console.WriteLine("]");
        }

        public static string HashtableToString(this Hashtable table)
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

        public static void WriteInstructions(this List<ByteInstruction> list)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < list.Count; i++)
            {
                var instruction = list[i];
                if (instruction != null)
                {
                    sb.AppendLine($"{i}:{instruction.ToString()}");
                }
            }
            Console.WriteLine(sb.ToString());
        }

        public static bool ContainsTokenType(this List<Token> list, TokenType tokenType)
        {
            return list.Any(token => token.Type == tokenType);
        }
        public static Token StringToToken(this  string str)
        {
            return new(TokenType.STRING,str);
        }
    }
}
