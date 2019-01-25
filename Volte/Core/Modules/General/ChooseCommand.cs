using System;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using Volte.Helpers;

namespace Volte.Core.Modules.General {
    public partial class GeneralModule : VolteModule {
        [Command("Choose")]
        [Remarks("Usage: choose {option1|option2|option3|...}")]
        public async Task Choose([Remainder] string message) {
            var opt = message.Split('|', StringSplitOptions.RemoveEmptyEntries);

            await Context.Channel.SendMessageAsync(
                "",
                false,
                CreateEmbed(Context,
                    $"I choose `{opt[new Random().Next(0, opt.Length)]}`."
                )
            );
        }
    }
}