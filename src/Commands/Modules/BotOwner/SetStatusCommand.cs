using System.Threading.Tasks;
using Discord;
using Qmmands;
using Volte.Commands.Checks;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public partial class BotOwnerModule : VolteModule
    {
        [Command("SetStatus")]
        [Description("Sets the bot's status.")]
        [Remarks("Usage: |prefix|setstatus {dnd|idle|invisible|online}")]
        [RequireBotOwner]
        public Task<VolteCommandResult> SetStatusAsync([Remainder] string status)
        {
            return status.ToLower() switch
                {
                "dnd" => Ok("Set the status to Do Not Disturb.",
                    _ => Context.Client.SetStatusAsync(UserStatus.DoNotDisturb)),
                "idle" => Ok("Set the status to Idle.", _ => Context.Client.SetStatusAsync(UserStatus.Idle)),
                "invisible" => Ok("Set the status to Invisible.",
                    _ => Context.Client.SetStatusAsync(UserStatus.Invisible)),
                "online" => Ok("Set the status to Online.",
                    _ => Context.Client.SetStatusAsync(UserStatus.Online)),
                _ => BadRequest(
                    "Your option wasn't known, so I didn't modify the status.\nAvailable options for this command are `dnd`, `idle`, `invisible`, or `online`.")
                };
        }
    }
}