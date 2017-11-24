using Repository.Common.Validation;
using Repository.Editor.Android;

namespace Repository.Internal.Editor
{
    public class EditorSettings
    {
        public EditorSettings(EditorTheme theme)
        {
            Verify.NotNull(theme, nameof(theme));

            Theme = theme;
        }

        public EditorTheme Theme { get; }
    }
}