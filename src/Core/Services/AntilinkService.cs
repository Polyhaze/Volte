using System.Linq;
using System.Threading.Tasks;
using RestSharp;
using Volte.Core.Commands;
using Volte.Core.Discord;
using Volte.Core.Utils;

namespace Volte.Core.Services
{
    public sealed class AntilinkService : IService
    {
        internal async Task CheckMessageAsync(VolteContext ctx)
        {
            var config = VolteBot.GetRequiredService<DatabaseService>().GetConfig(ctx.Guild.Id);
            var m = ctx.Message.Content.Split(" ");
            if (m.Length < 1) m = new[] {ctx.Message.Content};

            if (!config.ModerationOptions.Antilink || UserUtil.IsAdmin(ctx)) return;

            foreach (var part in m)
            {
                if (!part.StartsWith("http://") && !part.StartsWith("https://")) continue;
                if (part.Contains("oauth2/authorize?client_id=")) continue;
                var resp = new RestClient(part).Execute(new RestRequest());

                var first = resp.Headers.FirstOrDefault(x => x.Name.Equals("Content-Security-Policy"));

                if (first != null && first.Value.ToString().Contains("discord.gg"))
                {
                    await ctx.Message.DeleteAsync();
                    var warnMsg = await Util.Send(ctx.Channel,
                        Util.CreateEmbed(ctx, "Don't send server invites here."));
                    await Task.Delay(3000).ContinueWith(_ => warnMsg.DeleteAsync());
                }
            }
        }
    }
}