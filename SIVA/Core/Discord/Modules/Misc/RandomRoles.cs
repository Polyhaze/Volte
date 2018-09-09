using System;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using SIVA.Core.Files.Readers;
using SIVA.Helpers;

namespace SIVA.Core.Discord.Modules.Misc {
    public class RandomRoles : SIVACommand {
        [Command("RandomRoleMe")]
        public async Task PickARole() {
            var config = ServerConfig.Get(Context.Guild);
            if (config.RandomRoles.Count == 0) {
                await Context.Channel.SendMessageAsync("", false,
                    CreateEmbed(Context, "This server doesn't have random roles setup/enabled."));
            }

            var r = new Random().Next(0, config.RandomRoles.Count);
            var role = config.RandomRoles.ElementAt(r);
            var targetRole = Context.Guild.Roles.FirstOrDefault(ro => ro.Id == role);

            if (targetRole == null) {
                await Context.Channel.SendMessageAsync("", false,
                    CreateEmbed(Context, $"Something went wrong. Ping the developer of this bot to report."));
            }

            await ((SocketGuildUser) Context.User).AddRoleAsync(targetRole);

            await Context.Channel.SendMessageAsync("", false,
                CreateEmbed(Context, $"Chose a random role for you....your role is **{targetRole.Name}**."));
        }
    }
}