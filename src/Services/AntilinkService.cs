using System.Linq;
using System.Threading.Tasks;
using RestSharp;
using Volte.Commands;
using Volte.Extensions;

namespace Volte.Services
{
    [Service("Antilink", "The main Service for checking links sent in chat.")]
    public sealed class AntilinkService
    {
        private readonly DatabaseService _db;

        public AntilinkService(DatabaseService databaseService)
        {
            _db = databaseService;
        }

        internal async Task CheckMessageAsync(VolteContext ctx)
        {
            var config = _db.GetConfig(ctx.Guild.Id);
            var m = ctx.Message.Content.Split(" ");
            if (m.Length < 1) m = new[] {ctx.Message.Content};

            if (!config.ModerationOptions.Antilink || ctx.User.IsAdmin()) return;

            foreach (var part in m)
            {
                if (!part.StartsWith("http://") && !part.StartsWith("https://")) continue;
                if (part.Contains("oauth2/authorize?client_id=")) continue;
                var resp = new RestClient(part).Execute(new RestRequest());

                var csp = resp.Headers.FirstOrDefault(x => x.Name.Equals("Content-Security-Policy"));

                if (csp != null && csp.Value.ToString().Contains("discord.gg"))
                {
                    await ctx.Message.DeleteAsync();
                    var warnMsg = await ctx.CreateEmbed("Don't send server invites here.").SendToAsync(ctx.Channel);
                    await Task.Delay(3000).ContinueWith(_ => warnMsg.DeleteAsync());
                }
            }
        }
    }
}