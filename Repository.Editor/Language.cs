namespace Repository.Editor
{
    public enum Language
    {
        Begin,

        CSharp = Begin,
        Java,

        Unknown,
        End = Unknown
    }

    public static class LanguageExtensions
    {
        public static bool IsValid(this Language language)
        {
            return language >= Language.Begin && language <= Language.End;
        }
    }
}