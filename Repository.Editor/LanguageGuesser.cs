using Repository.Common.Validation;

namespace Repository.Editor
{
    public static class LanguageGuesser
    {
        public static Language GuessFromDocument(DocumentInfo documentInfo)
        {
            Verify.NotNull(documentInfo, nameof(documentInfo));

            return
                GuessFromExtension(documentInfo.Extension) ??
                GuessFromContent(documentInfo.Content) ??
                Language.Unknown;
        }

        private static Language? GuessFromExtension(string extension)
        {
            Verify.NotNullOrEmpty(extension, nameof(extension));

            switch (extension)
            {
                case "cs":
                    return Language.CSharp;
                case "java":
                    return Language.Java;
            }

            return null;
        }

        private static Language? GuessFromContent(string content)
        {
            Verify.NotNull(content, nameof(content));

            return null;
        }
    }
}