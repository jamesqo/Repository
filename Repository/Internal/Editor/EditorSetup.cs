using System;
using System.Threading.Tasks;
using Android.Widget;
using Repository.Common.Validation;
using Repository.Editor;

namespace Repository.Internal.Editor
{
    internal abstract class EditorSetup
    {
        public abstract Task<IDisposable> Run(EditText editor, EditorSettings settings);

        public static EditorSetup FromLanguage(Language language)
        {
            Verify.Argument(language.IsValid(), nameof(language));

            switch (language)
            {
                case Language.CSharp:
                    return new CSharpEditorSetup();
                default:
                    return new CommonEditorSetup(language);
            }
        }
    }
}