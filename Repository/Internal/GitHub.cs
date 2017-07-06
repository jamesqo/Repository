using Octokit;

namespace Repository.Internal
{
    internal static class GitHub
    {
        public static GitHubClient Client { get; } = new GitHubClient(new ProductHeaderValue(Strings.AppName));
    }
}