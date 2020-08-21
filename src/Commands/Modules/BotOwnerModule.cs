using System.Net.Http;
using Volte.Commands.Checks;
using Volte.Services;
// ReSharper disable MemberCanBePrivate.Global

namespace Volte.Commands.Modules
{
    [RequireBotOwner]
    public sealed partial class BotOwnerModule : VolteModule
    {
        public EvalService Eval { get; set; }
        public HttpClient Http { get; set; }

    }
}