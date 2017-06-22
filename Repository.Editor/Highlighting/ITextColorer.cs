namespace Repository.Editor.Highlighting
{
    public interface ITextColorer
    {
        void Color(SyntaxKind kind, int count);
    }
}
