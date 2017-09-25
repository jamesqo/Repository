using Repository.Common.Validation;

namespace Repository.Editor
{
    public class DocumentInfo
    {
        public DocumentInfo(string path, string content)
        {
            Verify.NotNullOrEmpty(path, nameof(path));
            Verify.NotNull(content, nameof(content));

            Path = path;
            Content = content;
            Extension = System.IO.Path.GetExtension(path).TrimStart('.');
        }

        public string Content { get; }

        public string Extension { get; }

        public string Path { get; }
    }
}