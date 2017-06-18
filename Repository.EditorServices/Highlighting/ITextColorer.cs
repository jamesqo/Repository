namespace Repository.EditorServices.Highlighting
{
    public interface ITextColorer
    {
        void Color(SyntaxKind kind, int count);
    }

    public interface ITextColorer<out TResult> : ITextColorer
    {
        TResult Result { get; }
    }
}
