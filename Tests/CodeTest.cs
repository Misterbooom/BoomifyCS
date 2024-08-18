using System;
using BoomifyCS.Exceptions;
using BoomifyCS.Parser.NodeParser;
using NUnit.Framework;

namespace BoomifyCS.Tests
{
    public class CodeTest
    {
        [Test]
        public void SyntaxTest()
        {
            BracketTest();
            IfElseTest();
            LoopTest();
            ArrayIndexTest();
        }
        public void RealTest()
        {
            Calculator();
        }
        private void Calculator()
        {
            string calculatorCode = @"


}   

            ";
            RunLanguage(calculatorCode,expectError:false);
        }
        public static void BracketTest()
        {
            string codeWithMissingBracket = @"
                (1 + 2
                ";
            RunLanguage(codeWithMissingBracket, ErrorMessage.UnmatchedOpeningParenthesis());

            codeWithMissingBracket = @"
                1 + 2)
                ";
            RunLanguage(codeWithMissingBracket, ErrorMessage.UnmatchedClosingParenthesis());

            codeWithMissingBracket = @"
            {
            ";
            RunLanguage(codeWithMissingBracket, ErrorMessage.UnmatchedOpeningBrace());

            codeWithMissingBracket = @"
            }
            ";
            RunLanguage(codeWithMissingBracket, ErrorMessage.UnmatchedClosingBrace());
        }

        public static void IfElseTest()
        {
            string codeWithElseWithoutIf = @"
                else {
                    explode(""This should not work"");
                }
                ";
            RunLanguage(codeWithElseWithoutIf, ErrorMessage.ElseWithoutMatchingIf());

            string codeWithElseIfWithoutIf = @"
                else if (x > 0) {
                    explode(""This should not work"");
                }
                ";
            RunLanguage(codeWithElseIfWithoutIf, ErrorMessage.ElseIfWithoutMatchingIf());

            string codeWithElseFollowingElseIf = @"
                else if (x > 0) {
                    explode(""This should not work"");
                } else {
                    explode(""This should not work either"");
                }
                ";
            RunLanguage(codeWithElseFollowingElseIf, ErrorMessage.ElseIfWithoutMatchingIf());

            string codeWithElseIfAfterElse = @"
                if (x > 0) {
                    explode(""First block"");
                } else {
                    explode(""Else block"");
                } else if (x < 0) {
                    explode(""This should not work"");
                }
                ";
            RunLanguage(codeWithElseIfAfterElse, ErrorMessage.ElseIfCannotFollowElseDirectly());
        }

        public static void LoopTest()
        {
            // Test case 1: for loop without initialization, condition, or increment
            string codeWithLoopWithoutInitialization = @"
                for (; i < 10; i++) {
                    explode(i);
                }
                ";
            RunLanguage(codeWithLoopWithoutInitialization, ErrorMessage.ForLoopMustHaveInitialization());

            string codeWithLoopWithoutCondition = @"
                for (var i = 0; ; i++) {
                    explode(i);
                }
                ";
            RunLanguage(codeWithLoopWithoutCondition, ErrorMessage.ForLoopMustHaveCondition());

           

            string codeWithLoopWithInvalidCondition = @"
                for (var i = 0; i < 10; i > 10) {
                    explode(i);
                }
                ";
            RunLanguage(codeWithLoopWithInvalidCondition, ErrorMessage.InvalidIncrementExpression());

            // Test case 2: for loop with invalid increment expression
            string codeWithLoopWithInvalidIncrementExpression = @"
                for (var i = 0;NothingHere; i++) {i++;};
                ";
            RunLanguage(codeWithLoopWithInvalidIncrementExpression, ErrorMessage.InvalidConditionExpression());

            // Test case 3: for loop with invalid variable declaration
            string codeWithLoopWithInvalidVariableDeclaration = @"
                for (i < 10; i < 10; i++) {
                    explode(i);
                }
                ";
            RunLanguage(codeWithLoopWithInvalidVariableDeclaration, ErrorMessage.InvalidVariableDeclaration());

        }
        public static void ArrayIndexTest()
        {
            // Test case: Array index out of bounds
            string codeWithOutOfBoundsIndex = @"
                var arr = [1, 2, 3];
                explode(arr[5]);
                ";
            RunLanguage(codeWithOutOfBoundsIndex, ErrorMessage.InvalidIndex(5, 3));

            // Test case: Indexing a non-array type
            string codeWithNonArrayIndexing = @"
                var num = 5;
                explode(num[0]);
                ";
            RunLanguage(codeWithNonArrayIndexing, ErrorMessage.IndexOfArrayTypeError());
        }
        private static void RunLanguage(string code, string expectedMessage = "", bool expectError = true)
        {
          
            try
            {
                AstBuilder.BuildFromStringAndRunVM(code);
            }
            catch (BifyError e)
            {
                if (!expectError)
                {
                    e.PrintException();
                    Assert.Fail($"Test failed - {code}");
                    return;
                }
                string message = e.Message;
                
                Assert.That(message, Is.EqualTo(expectedMessage),
                    $"Expected error message: '{expectedMessage}', but got: '{e.Message}'");
                Console.WriteLine($"Test completed - `{code}`");
                return;
            }
            if (!expectError)
            {
                Console.WriteLine($"Test completed - `{code}`");
                return;
            }
            Assert.Fail($"Expected BifyError, but got nothing - `{code}`");
        }
    }
}
