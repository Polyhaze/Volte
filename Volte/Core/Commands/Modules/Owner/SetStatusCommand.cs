using System.Threading.Tasks;
using Discord;
using Qmmands;
using Volte.Core.Commands.Preconditions;
using Volte.Core.Data;
using Volte.Core.Discord;
using Volte.Core.Extensions;

namespace Volte.Core.Commands.Modules.Owner
{
    public partial class OwnerModule : VolteModule
    {
        [Command("SetStatus")]
        [Description("Sets the bot's status.")]
        [Remarks("Usage: |prefix|setstatus {dnd|idle|invisible|online}")]
        [RequireBotOwner]
        public async Task SetStatusAsync([Remainder] string status)
        {
            var embed = Context.CreateEmbedBuilder();
            switch (status.ToLower())
            {
                case "dnd":
                    await VolteBot.Client.SetStatusAsync(UserStatus.DoNotDisturb);
                    embed.WithDescription("Set the status to Do Not Disturb.");
                    break;
                case "idle":
                    await VolteBot.Client.SetStatusAsync(UserStatus.Idle);
                    embed.WithDescription("Set the status to Idle.");
                    break;
                case "invisible":
                    await VolteBot.Client.SetStatusAsync(UserStatus.Invisible);
                    embed.WithDescription("Set the status to Invisible.");
                    break;
                case "online":
                    await VolteBot.Client.SetStatusAsync(UserStatus.Online);
                    embed.WithDescription("Set the status to Online.");
                    break;
                default:
                    await VolteBot.Client.SetStatusAsync(UserStatus.Online);
                    embed.WithDescription(
                        "Your option wasn't known, so I set the status to Online.\nAvailable options for this command are `dnd`, `idle`, `invisible`, or `online`.");
                    break;
            }

            await embed.SendTo(Context.Channel);
        }
    }
}