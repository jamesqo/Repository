using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Text;
using Android.Widget;
using Repository.Common.Validation;
using Repository.Editor;
using Repository.Editor.Android;
using Repository.Internal;
using Repository.Internal.Android;
using Repository.Internal.Editor;
using Repository.Internal.Threading;
using Repository.JavaInterop;

namespace Repository
{
    [Activity]
    public partial class EditFileActivity : Activity
    {
        private EditText _editor;

        private string _content;
        private string _path;

        private IEditorLifetimeManager _manager;

        public override async void OnBackPressed()
        {
            await _manager.TeardownEditor();
            base.OnBackPressed();
        }

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            void CacheViews()
            {
                _editor = FindViewById<EditText>(Resource.Id.EditFile_Editor);
            }

            void CacheParameters()
            {
                _content = EditorContent.Current;
                // The content can be arbitrarily large. Allow the GC to collect the string once we're done with it.
                EditorContent.Current = null;
                _path = Intent.Extras.GetString(Strings.Extra_EditFile_Path).NotNullOrEmpty();
            }

            base.OnCreate(savedInstanceState);

            this.HideActionBar();

            SetContentView(Resource.Layout.EditFile);
            CacheViews();
            CacheParameters();

            var theme = GetEditorTheme();
            if (await SetupEditor(theme).BecomesCanceled())
            {
                return;
            }
        }

        private EditorTheme GetEditorTheme()
        {
            return EditorTheme.GetDefault(TypefaceProvider.GetInstance(Assets));
        }

        private Language GuessLanguage()
        {
            var documentInfo = new DocumentInfo(path: _path, content: _content);
            return LanguageDetector.GuessLanguage(documentInfo);
        }

        private async Task SetupEditor(EditorTheme theme)
        {
            var language = GuessLanguage();
            var setup = EditorSetup.FromLanguage(language);
            _teardown = await setup.Run();
        }

        private void SetupEditorCore(EditorTheme theme, EditorText text)
        {
            _editor.InputType |= InputTypes.TextFlagNoSuggestions;
            _editor.SetEditableFactory(NoCopyEditableFactory.Instance);
            _editor.SetTypeface(theme.Typeface, TypefaceStyle.Normal);
            _editor.SetText(text, TextView.BufferType.Editable);
        }
    }
}