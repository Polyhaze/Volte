using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Octokit;

namespace Volte.Core.Services
{
    public class GitHubService : IService
    {
        public readonly GitHubClient GitHub = new GitHubClient(new ProductHeaderValue("GreemDev"));

        public async Task<Commit> GetLastCommitAsync()
        {
            return (await GitHub.Repository.Commit.GetAll("GreemDev", "Volte"))
                .Select(x => x.Commit).Take(1).ElementAt(0);
        }

        public async Task<IEnumerable<Commit>> GetAllCommitsAsync()
        {
            return (await GitHub.Repository.Commit.GetAll("GreemDev", "Volte")).Select(x => x.Commit);
        }

        public async Task<int> GetOpenIssuesCountAsync()
        {
            return (await GitHub.Repository.Get("GreemDev", "Volte")).OpenIssuesCount;
        }

        public async Task<IReadOnlyList<PullRequest>> GetOpenPullRequestsAsync()
        {
            return await GitHub.PullRequest.GetAllForRepository("GreemDev", "Volte");
        }
    }
}