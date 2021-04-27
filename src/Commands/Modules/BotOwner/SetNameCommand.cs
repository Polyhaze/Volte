using System.Threading.Tasks;
using Discord;
using Qmmands;

namespace Volte.Commands.Modules
{
    public sealed partial class BotOwnerModule
    {
        [Command("SetName")]
        [Description("Sets the bot's username.")]
        public Task<ActionResult> SetNameAsync([Remainder, Description("The username to use.")] string name) 
            => Ok($"Set my username to {Format.Bold(name)}.", _ => Context.Client.CurrentUser.ModifyAsync(u => u.Username = name));
    }
}