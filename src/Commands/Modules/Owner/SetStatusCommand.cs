using System.Threading.Tasks;
using DSharpPlus.Entities;
using Qmmands;
using Volte.Commands.Preconditions;
using Volte.Discord;
using Volte.Extensions;

namespace Volte.Commands.Modules.Owner
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
                    await Context.Client.UpdateStatusAsync(null, UserStatus.DoNotDisturb);
                    embed.WithDescription("Set the status to Do Not Disturb.");
                    break;
                case "idle":
                    await Context.Client.UpdateStatusAsync(null, UserStatus.Idle);
                    embed.WithDescription("Set the status to Idle.");
                    break;
                case "invisible":
                    await Context.Client.UpdateStatusAsync(null, UserStatus.Invisible);
                    embed.WithDescription("Set the status to Invisible.");
                    break;
                case "online":
                    await Context.Client.UpdateStatusAsync(null, UserStatus.Online);
                    embed.WithDescription("Set the status to Online.");
                    break;
                default:
                    await Context.Client.UpdateStatusAsync(null, UserStatus.Online);
                    embed.WithDescription(
                        "Your option wasn't known, so I set the status to Online.\nAvailable options for this command are `dnd`, `idle`, `invisible`, or `online`.");
                    break;
            }

            await embed.SendToAsync(Context.Channel);
        }
    }
}