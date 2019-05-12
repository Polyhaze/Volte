using System.Linq;
using System.Threading.Tasks;
using Discord;
using Qmmands;
using Volte.Commands.Preconditions;
using Volte.Extensions;

namespace Volte.Commands.Modules.Moderation
{
    public partial class ModerationModule : VolteModule
    {
        [Command("Bans")]
        [Description("Shows all bans in this server.")]
        [Remarks("Usage: |prefix|bans")]
        [RequireBotGuildPermission(GuildPermission.BanMembers)]
        [RequireGuildModerator]
        public async Task BansAsync()
        {
            var banList = await Context.Guild.GetBansAsync();
            if (!banList.Any())
            {
                await Context.CreateEmbed("This server doesn't have anyone banned.").SendToAsync(Context.Channel);
            }
            else
            {
                await Context.CreateEmbed(string.Join('\n',
                        banList.Select(b => $"**{b.User}**: {b.Reason ?? "No reason provided."}")))
                    .SendToAsync(Context.Channel);
            }
        }
    }
}