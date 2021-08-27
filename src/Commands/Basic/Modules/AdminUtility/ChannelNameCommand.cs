using System.Threading.Tasks;
using Discord;
using Qmmands;
using Volte.Core.Entities;

namespace Volte.Commands.Modules
{
    public sealed partial class AdminUtilityModule
    {
        [Command("ChannelName", "Cn")]
        [Description("Sets the name of the current channel.")]
        public async Task<ActionResult> ChannelNameAsync(
            [Remainder, Description("The name to change the channel to. Automatically replaces spaces with -.")]
            string name)
        {
            await Context.Channel.ModifyAsync(c => c.Name = name.Replace(" ", "-"));
            return Ok($"Set this channel's name to **{name}**.");
        }
    }
}