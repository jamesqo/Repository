using System.Threading.Tasks;
using Android.Widget;
using Repository.Common.Validation;
using Repository.Editor;

namespace Repository.Internal.Editor
{
    internal abstract class EditorManager
    {
        private EditText _editor;

        public static EditorManager FromLanguage(Language language)
        {
            Verify.Argument(language.IsValid(), nameof(language));

            switch (language)
            {
                case Language.CSharp:
                    return new CSharpEditorManager();
                default:
                    return new CommonEditorManager(language);
            }
        }

        public abstract Task OnEditorCreated();

        public abstract Task OnEditorDestroyed();

        public virtual void SetEditor(EditText editor)
        {
            Verify.NotNull(editor, nameof(editor));
            Verify.ValidState(_editor == null, "This manager already has an editor.");

            _editor = editor;
        }
    }
}