using System.Linq;
using System.Threading.Tasks;
using Discord;
using Qmmands;
using Volte.Core.Commands.Preconditions;
using Volte.Core.Extensions;

namespace Volte.Core.Commands.Modules.Moderation
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
            var bans = (await Context.Guild.GetBansAsync()).Select(b => $"**{b.User}**: {b.Reason ?? "No reason provided."}").ToList();
            if (bans.Count.Equals(0))
            {
                await Context.CreateEmbed("This server doesn't have anyone banned.").SendToAsync(Context.Channel);
            }
            else
            {
                await Context.CreateEmbed(string.Join('\n', bans)).SendToAsync(Context.Channel);
            }

        }

    }
}
