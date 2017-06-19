using Octokit;

namespace Repository.Internal
{
    // TODO: Unify this + EditorContent into 1 static class where global app state can be accessed?
    internal static class GitHub
    {
        public static GitHubClient Client { get; } = new GitHubClient(new ProductHeaderValue(Strings.AppName));
    }
}