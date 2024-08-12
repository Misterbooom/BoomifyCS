using System;
using NUnit.Framework;
using BoomifyCS.Exceptions;
using BoomifyCS.Parser.NodeParser;

namespace BoomifyCS.Tests
{
    public class CodeTest
    {
        [Test]
        public void SyntaxTest()
        {
            BracketTest();
            IfElseTest();
        }

        public static void BracketTest()
        {
            string codeWithMissingBracket = @"
                (1 + 2
                ";
            RunLanguage(codeWithMissingBracket, "Unmatched '(': missing ')'.");

            codeWithMissingBracket = @"
                1 + 2)
                ";
            RunLanguage(codeWithMissingBracket, "Unmatched ')': missing '('.");

            codeWithMissingBracket = @"
            {
            ";
            RunLanguage(codeWithMissingBracket, "Unmatched '{': missing '}'.");

            codeWithMissingBracket = @"
            }
            ";
            RunLanguage(codeWithMissingBracket, "Unmatched '}': missing '{'.");
        }

        public static void IfElseTest()
        {
            // Test case 1: else without matching if
            string codeWithElseWithoutIf = @"
                else {
                    ignite(""This should not work"");
                }
                ";
            RunLanguage(codeWithElseWithoutIf, "'else' without matching 'if'.");

            // Test case 2: else if without matching if
            string codeWithElseIfWithoutIf = @"
                else if (x > 0) {
                    ignite(""This should not work"");
                }
                ";
            RunLanguage(codeWithElseIfWithoutIf, "'else if' without matching 'if'.");

            // Test case 3: else following else if without if
            string codeWithElseFollowingElseIf = @"
                else if (x > 0) {
                    ignite(""This should not work"");
                } else {
                    ignite(""This should not work either"");
                }
                ";
            RunLanguage(codeWithElseFollowingElseIf, "'else if' without matching 'if'.");

            // Test case 4: else if following else
            string codeWithElseIfAfterElse = @"
                if (x > 0) {
                    ignite(""First block"");
                } else {
                    ignite(""Else block"");
                } else if (x < 0) {
                    ignite(""This should not work"");
                }
                ";
            RunLanguage(codeWithElseIfAfterElse, "'else if' cannot follow 'else' directly.");
        }

        private static void RunLanguage(string code, string expectedMessage)
        {
            try
            {
                AstBuilder.BuildString(code);
            }
            catch (BifyError e)
            {
                string message = e.Message;
                Assert.That(message, Is.EqualTo(expectedMessage),
                    $"Expected error message: '{expectedMessage}', but got: '{e.Message}'");
                Console.WriteLine($"Test completed - `{code}`");
                return;
            }

            Assert.Fail($"Expected BifyError, but got nothing - `{code}`");
        }
    }
}
