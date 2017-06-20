namespace Repository.EditorServices.Highlighting
{
    public interface ITextColorer
    {
        void Color(SyntaxKind kind, int count);
    }
}
