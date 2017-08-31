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
        [Test]
        public async void Foo()
        {
            var yielder = NopYielder.Instance.CancelAfter(0);
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

            await highlighter.Highlight(sourceText, colorer).RunToCancellation();

            Assert.AreEqual(new[]
            {
                ("package", Keyword),
                ("com", Identifier),
                (".", Plaintext),
                ("mycompany", Identifier),
                ("class", Keyword),
                ("C", TypeDeclaration),
                ("{", Plaintext),
                ("void", Keyword),
                ("m", MethodDeclaration),
                ("(", Plaintext),
                (")", Plaintext),
                ("{", Plaintext)
            },
            colorer.GetSyntaxAssignments());
        }
    }
}