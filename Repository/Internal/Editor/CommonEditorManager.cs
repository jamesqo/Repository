using System;
using System.Threading.Tasks;
using Android.Widget;
using Repository.Common.Validation;
using Repository.Editor;
using Repository.Editor.Highlighting;

namespace Repository.Internal.Editor
{
    internal class CommonEditorManager : EditorManager
    {
        private readonly IHighlighter _highlighter;

        internal CommonEditorManager(Language language)
        {
            Verify.Argument(language.IsValid(), nameof(language));

            _highlighter = Highlighter.FromLanguage(language);
        }

        public override Task OnEditorCreated()
        {
            
        }

        public override Task OnEditorDestroyed()
        {
        }
    }
}