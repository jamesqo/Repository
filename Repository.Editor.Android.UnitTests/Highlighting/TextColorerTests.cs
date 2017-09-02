using System;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Repository.Editor.Android.Highlighting;
using Repository.Editor.Android.UnitTests.TestInternal.Collections;
using Repository.Editor.Android.UnitTests.TestInternal.Editor.Highlighting;
using Repository.Editor.Android.UnitTests.TestInternal.Threading;
using Repository.Editor.Highlighting;
using static Repository.Editor.Highlighting.SyntaxKind;

namespace Repository.Editor.Android.UnitTests.Highlighting
{
    [TestFixture]
    public class TextColorerTests
    {
        [Test]
        public async void Flushing(
            [Range(1, 6)] int numberOfFlushes,
            [Range(1, 6)] int flushSize)
        {
            async Task RunTest()
            {
                var sourceText = @"
package com.mycompany;

class C {
    void m() {
        System.out.println(""Hello, world!"");
        System.out.println(""Scary to see Java in the middle of C# code, isn't it?"");
    }
}";
                var colorer = CreateTextColorer(sourceText, out var yielder);
                SetCallback(yielder, colorer, numberOfFlushes - 1, Callback1);

                using (colorer.Setup(flushSize))
                {
                    await Highlighter.Java.Highlight(sourceText, colorer);
                }
            }

            void Callback1(CallbackRunnerYielder yielder, TextColorer colorer)
            {
                var expected = new SyntaxAssignment[]
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