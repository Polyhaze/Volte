using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using RestSharp;
using Volte.Core.Discord;
using Volte.Core.Modules;
using Volte.Helpers;

namespace Volte.Core.Services {
    internal class AntilinkService {
        internal async Task CheckMessage(VolteContext ctx) {
            var config = VolteBot.ServiceProvider.GetRequiredService<DatabaseService>().GetConfig(ctx.Guild.Id);
            var m = ctx.Message.Content.Split(" ");
            if (m.Length < 1) {
                m = new [] {ctx.Message.Content};
            }

            if (!config.Antilink || UserUtils.IsAdmin(ctx)) return;

            foreach (var part in m) {
                if (!part.StartsWith("http://") && !part.StartsWith("https://")) continue;
                if (part.Contains("oauth2/authorize?client_id=")) continue;
                var resp = new RestClient(part).Execute(new RestRequest());

                var first = resp.Headers.FirstOrDefault(x => x.Name.Equals("Content-Security-Policy"));

                if (first != null && first.Value.ToString().Contains("discord.gg")) {
                    await ctx.Message.DeleteAsync();
                    var warnMsg = await Utils.Send(ctx.Channel,
                        Utils.CreateEmbed(ctx, "Don't send server invites here."));
                    await Task.Delay(3000);
                    await warnMsg.DeleteAsync();
                }
            }
        }
    }
}