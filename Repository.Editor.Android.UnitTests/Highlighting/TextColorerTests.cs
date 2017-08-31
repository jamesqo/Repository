using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NUnit.Framework;
using Repository.Editor.Android.Highlighting;
using Repository.Editor.Android.UnitTests.TestInternal.Editor.Highlighting;
using Repository.Editor.Android.UnitTests.TestInternal.Threading;
using Repository.Editor.Highlighting;
using static Repository.Editor.Highlighting.SyntaxKind;

namespace Repository.Editor.Android.UnitTests.Highlighting
{
    [TestFixture]
    public class TextColorerTests
    {
        [TestCaseSource(nameof(Flushing_Data))]
        public async void Flushing(int numberOfFlushes, int flushSize)
        {
            Debug.Assert(numberOfFlushes > 0);
            Debug.Assert(flushSize > 0);

            var yielder = NopYielder.Instance.CancelAfter(numberOfFlushes - 1);
            var sourceText = @"
package com.mycompany;

class C {
    void m() {
        System.out.println(""Hello, world!"");
        System.out.println(""Scary to see Java in the middle of C# code, isn't it?"");
    }
}";
            var theme = TestColorTheme.Instance;
            var colorer = new TextColorer(sourceText, theme, yielder);
            var highlighter = Highlighter.Java;

            using (colorer.Setup(flushSize))
            {
                await highlighter.Highlight(sourceText, colorer).RunToCancellation();
            }

            var expected = new[]
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
                ("out", Identifier),
                ("println", MethodIdentifier),
                ("(", Plaintext),
                (@"""Hello, world!""", StringLiteral),
                (")", Plaintext),
                (";", Plaintext),
                ("System", Identifier),
                ("out", Identifier),
                ("println", MethodIdentifier),
                ("(", Plaintext),
                (@"""Scary to see Java in the middle of C# code, isn't it?""", StringLiteral),
                (")", Plaintext),
                (";", Plaintext),
                ("}", Plaintext),
                ("}", Plaintext)
            };

            int tokenCount = numberOfFlushes * flushSize;
            Assert.AreEqual(
                expected.Take(tokenCount),
                colorer.GetSyntaxAssignments().Take(tokenCount).RemoveWhitespace());
        }

        public static IEnumerable<object[]> Flushing_Data()
        {
            foreach (int numberOfFlushes in Enumerable.Range(1, 8))
            {
                foreach (int flushSize in Enumerable.Range(1, 8))
                {
                    yield return new object[] { numberOfFlushes, flushSize };
                }
            }
        }
    }
}