using System.Net.Http;
using System.Threading;
using Volte.Core.Entities;
using Volte.Services;
// ReSharper disable MemberCanBePrivate.Global

namespace Volte.Commands.Modules
{
    [RequireBotOwner]
    public sealed partial class BotOwnerModule : VolteModule
    {
        public HttpClient Http { get; set; }
        public CancellationTokenSource Cts { get; set; }
        public AddonService Addon { get; set; }
    }
}