using System.Threading.Tasks;
using Discord;
using Qmmands;
using Volte.Commands.Checks;
using Volte.Core.Data.Models.Results;

namespace Volte.Commands.Modules
{
    public partial class BotOwnerModule : VolteModule
    {
        [Command("SetStatus")]
        [Description("Sets the bot's status.")]
        [Remarks("Usage: |prefix|setstatus {dnd|idle|invisible|online}")]
        [RequireBotOwner]
        public async Task<VolteCommandResult> SetStatusAsync([Remainder] string status)
        {
            switch (status.ToLower())
            {
                case "dnd":
                    await Context.Client.SetStatusAsync(UserStatus.DoNotDisturb);
                    return Ok("Set the status to Do Not Disturb.");

                case "idle":
                    await Context.Client.SetStatusAsync(UserStatus.Idle);
                    return Ok("Set the status to Idle.");

                case "invisible":
                    await Context.Client.SetStatusAsync(UserStatus.Invisible);
                    return Ok("Set the status to Invisible.");

                case "online":
                    await Context.Client.SetStatusAsync(UserStatus.Online);
                    return Ok("Set the status to Online.");

                default:
                    await Context.Client.SetStatusAsync(UserStatus.Online);
                    return BadRequest(
                        "Your option wasn't known, so I set the status to Online.\nAvailable options for this command are `dnd`, `idle`, `invisible`, or `online`.");
            }
        }
    }
}