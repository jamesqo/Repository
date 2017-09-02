using System;
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
        [Ignore("https://github.com/jamesqo/Repository/issues/103")]
        public async void DeletionBeforeColorCursor_DoesNotAffectPendingColorings()
        {
            async Task RunTest()
            {
                var sourceCode = JavaSourceCode1;
                var colorer = CreateTextColorer(sourceCode, out var yielder);
                SetCallback(yielder, colorer, 0, Callback1);

                using (colorer.FlushEveryToken())
                {
                    await Highlighter.Java.Highlight(sourceCode, colorer);
                }

                AfterHighlight(colorer);
            }

            void Callback1(CallbackRunnerYielder yielder, TextColorer colorer)
            {
                var cursor = colorer.Text.GetCursor(0);
                cursor.SkipWhitespace().Delete("package");
            }

            void AfterHighlight(TextColorer colorer)
            {
                var expected = JavaSourceCode1Assignments.Skip(1);
                var actual = colorer.GetSyntaxAssignments().RemoveWhitespaceTokens();
                Assert.AreEqual(expected, actual);
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
                SetCallback(yielder, colorer, numberOfFlushes - 1, Callback1);

                using (colorer.Setup(flushSize))
                {
                    await Highlighter.Java.Highlight(sourceCode, colorer);
                }
            }

            void Callback1(CallbackRunnerYielder yielder, TextColorer colorer)
            {
                var expected = JavaSourceCode1Assignments;
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

            await RunTest();
        }

        private static TextColorer CreateTextColorer(string text, out CallbackRunnerYielder yielder)
        {
            yielder = new CallbackRunnerYielder(NopYielder.Instance);
            return new TextColorer(text, TestColorTheme.Instance, yielder);
        }

        private static void SetCallback(CallbackRunnerYielder yielder, TextColorer colorer, int numberOfYields, Action<CallbackRunnerYielder, TextColorer> callback)
        {
            yielder.SetCallback(numberOfYields, () => callback(yielder, colorer));
        }
    }
}