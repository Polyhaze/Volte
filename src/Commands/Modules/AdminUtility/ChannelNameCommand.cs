using System.Threading.Tasks;
using Discord;
using Qmmands;
using Volte.Commands.Preconditions;

namespace Volte.Commands.Modules.AdminUtility
{
    public partial class AdminUtilityModule : VolteModule
    {
        [Command("ChannelName", "Cn")]
        [Description("Sets the name of the current channel.")]
        [Remarks("Usage: |prefix|channelname {name}")]
        [RequireBotChannelPermission(ChannelPermission.ManageChannels)]
        [RequireGuildAdmin]
        public async Task ChannelNameAsync([Remainder] string name)
        {
            await Context.Channel.ModifyAsync(c => c.Name = name);
            await Context.ReactSuccessAsync();
        }
    }
}