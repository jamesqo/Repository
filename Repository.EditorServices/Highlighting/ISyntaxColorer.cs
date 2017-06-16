namespace Repository.EditorServices.Highlighting
{
    public interface ISyntaxColorer
    {
        void Color(SyntaxKind kind, int index, int count);
    }

    public interface ISyntaxColorer<out TResult> : ISyntaxColorer
    {
        TResult Result { get; }
    }
}
