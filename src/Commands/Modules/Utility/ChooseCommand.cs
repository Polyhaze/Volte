using System;
using System.Threading.Tasks;
using Discord;
using Gommon;
using Qmmands;

namespace Volte.Commands.Modules
{
    public sealed partial class UtilityModule
    {
        [Command("Choose")]
        [Description("Choose an item from a list separated by |.")]
        public Task<ActionResult> ChooseAsync(
            [Remainder, Description("The options you want to choose from; separated by `|`.")]
            string options)
            => Ok($"I choose {Format.Code(options.Split('|', StringSplitOptions.RemoveEmptyEntries).Random())}.");
    }
}