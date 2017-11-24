using System;
using System.Threading.Tasks;
using Android.Widget;
using Repository.Common.Validation;
using Repository.Editor;
using Repository.Editor.Highlighting;

namespace Repository.Internal.Editor
{
    internal class CommonEditorSetup : EditorSetup
    {
        private readonly IHighlighter _highlighter;

        internal CommonEditorSetup(Language language)
        {
            Verify.Argument(language.IsValid(), nameof(language));

            _highlighter = Highlighter.FromLanguage(language);
        }

        public override Task<IDisposable> Run(EditText editor, EditorSettings settings)
        {

        }
    }
}