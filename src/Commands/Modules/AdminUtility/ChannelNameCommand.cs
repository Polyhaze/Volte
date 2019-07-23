using System.Threading.Tasks;
using Discord;
using Qmmands;
using Volte.Commands.Checks;
using Volte.Core.Data.Models.Results;

namespace Volte.Commands.Modules
{
    public partial class AdminUtilityModule : VolteModule
    {
        [Command("ChannelName", "Cn")]
        [Description("Sets the name of the current channel.")]
        [Remarks("Usage: |prefix|channelname {name}")]
        [RequireBotChannelPermission(ChannelPermission.ManageChannels)]
        [RequireGuildAdmin]
        public async Task<VolteCommandResult> ChannelNameAsync([Remainder] string name)
        {
            await Context.Channel.ModifyAsync(c => c.Name = name);
            return Ok($"Set this channel's name to **{name}**.");
        }
    }
}