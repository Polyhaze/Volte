using System.Threading.Tasks;
using DSharpPlus;
using Qmmands;
using Volte.Commands.Preconditions;

namespace Volte.Commands.Modules.ServerAdmin
{
    public partial class ServerAdminModule : VolteModule
    {
        [Command("ChannelName", "Cn")]
        [Description("Sets the name of the current channel.")]
        [Remarks("Usage: |prefix|channelname {name}")]
        [RequireBotChannelPermission(Permissions.ManageChannels)]
        [RequireGuildAdmin]
        public async Task ChannelNameAsync([Remainder] string name)
        {
            await Context.Channel.ModifyAsync(x => x.Name = name);
            await Context.ReactSuccessAsync();
        }
    }
}