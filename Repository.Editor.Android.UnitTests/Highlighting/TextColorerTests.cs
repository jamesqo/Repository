using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Repository.Common.Collections;
using Repository.Editor.Android.Highlighting;
using Repository.Editor.Android.UnitTests.TestInternal.Editor.Highlighting;
using Repository.Editor.Android.UnitTests.TestInternal.JavaInterop;
using Repository.Editor.Android.UnitTests.TestInternal.Threading;
using Repository.Editor.Highlighting;
using static Repository.Editor.Highlighting.SyntaxKind;

namespace Repository.Editor.Android.UnitTests.Highlighting
{
    [TestFixture]
    public class TextColorerTests
    {
        private static readonly string JavaSourceCode1 = @"
package com.mycompany;

class C {
    void m() {
        System.out.println(""Hello, world!"");
        System.out.println(""Scary to see Java in the middle of C# code, isn't it?"");
    }
}";

        private static readonly IEnumerable<SyntaxAssignment> JavaSourceCode1Assignments = new SyntaxAssignment[]
        {
            ("package", Keyword),
            ("com", Identifier),
            (".", Plaintext),
            ("mycompany", Identifier),
            (";", Plaintext),
            ("class", Keyword),
            ("C", TypeDeclaration),
            ("{", Plaintext),
            ("void", Keyword),
            ("m", MethodDeclaration),
            ("(", Plaintext),
            (")", Plaintext),
            ("{", Plaintext),
            ("System", Identifier),
            (".", Plaintext),
            ("out", Identifier),
            (".", Plaintext),
            ("println", MethodIdentifier),
            ("(", Plaintext),
            (@"""Hello, world!""", StringLiteral),
            (")", Plaintext),
            (";", Plaintext),
            ("System", Identifier),
            (".", Plaintext),
            ("out", Identifier),
            (".", Plaintext),
            ("println", MethodIdentifier),
            ("(", Plaintext),
            (@"""Scary to see Java in the middle of C# code, isn't it?""", StringLiteral),
            (")", Plaintext),
            (";", Plaintext),
            ("}", Plaintext),
            ("}", Plaintext)
        };

        [Test]
        public async void Edit_BeforeColorCursor_DoesNotAffectPendingColorings([Values] bool editIsDeletion)
        {
            async Task RunTest()
            {
                var sourceCode = JavaSourceCode1;
                var colorer = CreateTextColorer(sourceCode, out var yielder);
                yielder.SetCallback(0, Callback1, colorer);

                using (colorer.FlushEveryToken())
                {
                    await Highlighter.Java.Highlight(sourceCode, colorer);
                }

                AfterHighlight(colorer);
            }

            void Callback1(CallbackRunnerYielder yielder, TextColorer colorer)
            {
                var cursor = colorer.Text.GetStartCursor();
                if (editIsDeletion)
                {
                    cursor.SkipWhitespaceRight().DeleteRight("package");
                }
                else
                {
                    cursor.InsertLeft("Hello, galaxy!");
                }
            }

            void AfterHighlight(TextColorer colorer)
            {
                var expected = GetExpectedAssignments();
                var actual = colorer.GetSyntaxAssignments().RemoveWhitespaceTokens();
                Assert.AreEqual(expected, actual);
            }

            IEnumerable<SyntaxAssignment> GetExpectedAssignments()
            {
                return editIsDeletion
                    ? JavaSourceCode1Assignments.Skip(1)
                    : JavaSourceCode1Assignments;
            }

            await RunTest();
        }

        [Test]
        public async void Edit_AfterColorCursor_DoesNotAffectPendingColorings([Values] bool editIsDeletion)
        {
            async Task RunTest()
            {
                var sourceCode = JavaSourceCode1;
                var colorer = CreateTextColorer(sourceCode, out var yielder);
                yielder.SetCallback(0, Callback1, colorer);

                using (colorer.FlushEveryToken())
                {
                    await Highlighter.Java.Highlight(sourceCode, colorer);
                }

                AfterHighlight(colorer);
            }

            void Callback1(CallbackRunnerYielder yielder, TextColorer colorer)
            {
                var subtext = "System.out.print";
                var cursor = colorer.Text.GetStartCursor(subtext);
                if (editIsDeletion)
                {
                    cursor.DeleteRight(subtext);
                }
                else
                {
                    cursor.InsertLeft("SSSSSS");
                }
            }

            void AfterHighlight(TextColorer colorer)
            {
                var expected = GetExpectedAssignments();
                var actual = colorer.GetSyntaxAssignments().RemoveWhitespaceTokens();
                Assert.AreEqual(expected, actual);
            }

            IEnumerable<SyntaxAssignment> GetExpectedAssignments()
            {
                if (!editIsDeletion)
                {
                    return JavaSourceCode1Assignments;
                }

                return JavaSourceCode1Assignments.ReplaceConsecutiveTokens(
                    new[] { "System", ".", "out", ".", "println" },
                    new SyntaxAssignment[] { ("ln", MethodIdentifier) });
            }

            await RunTest();
        }

        [Test]
        public async void Deletion_ContainsColorCursor_DoesNotAffectPendingColorings()
        {
            async Task RunTest()
            {
                var sourceCode = JavaSourceCode1;
                var colorer = CreateTextColorer(sourceCode, out var yielder);
                yielder.SetCallback(0, Callback1, colorer);

                using (colorer.FlushEveryToken())
                {
                    await Highlighter.Java.Highlight(sourceCode, colorer);
                }

                AfterHighlight(colorer);
            }

            void Callback1(CallbackRunnerYielder yielder, TextColorer colorer)
            {
                var subtext = "age com.my"; // package com.mycompany
                var cursor = colorer.Text.GetStartCursor(subtext);
                cursor.DeleteRight(subtext);
            }

            void AfterHighlight(TextColorer colorer)
            {
                var expected = GetExpectedAssignments();
                var actual = colorer.GetSyntaxAssignments().RemoveWhitespaceTokens();
                Assert.AreEqual(expected, actual);
            }

            IEnumerable<SyntaxAssignment> GetExpectedAssignments()
            {
                return JavaSourceCode1Assignments.ReplaceConsecutiveTokens(
                    new[] { "package", "com", ".", "mycompany" },
                    new SyntaxAssignment[] { ("pack", Keyword), ("company", Identifier) });
            }

            await RunTest();
        }

        [Test]
        public async void Deletion_EndCoincidesWithEndOfPendingColoring()
        {
            // This test should fail if the following check is omitted:
            // https://github.com/jamesqo/Repository/blob/102f610970d291eef2bf2c4177918cb355e292d4/java/repository/src/main/java/com/bluejay/repository/EditorText.java#L109-L111

            async Task RunTest()
            {
                var sourceCode = JavaSourceCode1;
                var colorer = CreateTextColorer(sourceCode, out _);
                Callback1(colorer);

                using (colorer.FlushEveryToken())
                {
                    await Highlighter.Java.Highlight(sourceCode, colorer);
                }

                AfterHighlight(colorer);
            }

            void Callback1(TextColorer colorer)
            {
                var subtext = "ackage"; // package
                var cursor = colorer.Text.GetStartCursor(subtext);
                cursor.DeleteRight(subtext);
            }

            void AfterHighlight(TextColorer colorer)
            {
                var expected = GetExpectedAssignments();
                var actual = colorer.GetSyntaxAssignments().RemoveWhitespaceTokens();
                Assert.AreEqual(expected, actual);
            }

            IEnumerable<SyntaxAssignment> GetExpectedAssignments()
            {
                return JavaSourceCode1Assignments.ReplaceToken("package", ("p", Keyword));
            }

            await RunTest();
        }

        [Test]
        public async void Deletion_EncompassesPreviousInsertion()
        {
            async Task RunTest()
            {
                var sourceCode = JavaSourceCode1;
                var colorer = CreateTextColorer(sourceCode, out _);
                Callback1(colorer);

                using (colorer.FlushEveryToken())
                {
                    await Highlighter.Java.Highlight(sourceCode, colorer);
                }

                AfterHighlight(colorer);
            }

            void Callback1(TextColorer colorer)
            {
                var subtext1 = "package";
                var message = "Hello, galaxy!";
                var cursor = colorer.Text.GetEndCursor(subtext1);
                cursor.InsertRight(message);

                var subtext2 = subtext1.Substring(1);
                cursor = colorer.Text.GetStartCursor(subtext2);
                // packageHello, galaxy! com.mycompany => p mpany
                cursor.DeleteRight($"{subtext2}{message} com.myco");
            }

            void AfterHighlight(TextColorer colorer)
            {
                var expected = GetExpectedAssignments();
                var actual = colorer.GetSyntaxAssignments().RemoveWhitespaceTokens();
                Assert.AreEqual(expected, actual);
            }

            IEnumerable<SyntaxAssignment> GetExpectedAssignments()
            {
                return JavaSourceCode1Assignments.ReplaceConsecutiveTokens(
                    new[] { "package", "com", ".", "mycompany" },
                    new SyntaxAssignment[] { ("p", Keyword), ("mpany", Identifier) });
            }

            await RunTest();
        }

        [Test]
        public async void Flushing(
            [Range(1, 6)] int numberOfFlushes,
            [Range(1, 6)] int flushSize)
        {
            async Task RunTest()
            {
                var sourceCode = JavaSourceCode1;
                var colorer = CreateTextColorer(sourceCode, out var yielder);
                yielder.SetCallback(numberOfFlushes - 1, Callback1, colorer);

                using (colorer.Setup(flushSize))
                {
                    await Highlighter.Java.Highlight(sourceCode, colorer);
                }
            }

            void Callback1(CallbackRunnerYielder yielder, TextColorer colorer)
            {
                var expected = GetExpectedAssignments();
                int maxTokenCount = numberOfFlushes * flushSize;
                var actual = colorer.GetSyntaxAssignments().ToArray();

                if (actual.Length < maxTokenCount)
                {
                    actual = actual.RemoveWhitespaceTokens().ToArray();
                    Assert.AreEqual(expected, actual);
                }
                else
                {
                    Assert.AreEqual(maxTokenCount, actual.Length);

                    actual = actual.RemoveWhitespaceTokens().ToArray();
                    Assert.IsTrue(expected.StartsWith(actual));
                }
            }

            IEnumerable<SyntaxAssignment> GetExpectedAssignments() => JavaSourceCode1Assignments;

            await RunTest();
        }

        private static TextColorer CreateTextColorer(string text, out CallbackRunnerYielder yielder)
        {
            yielder = new CallbackRunnerYielder(NopYielder.Instance);
            return new TextColorer(text, TestColorTheme.Instance, yielder);
        }
    }
}
