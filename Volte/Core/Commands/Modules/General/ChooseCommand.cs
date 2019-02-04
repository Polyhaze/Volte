using System;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using Volte.Helpers;

namespace Volte.Core.Commands.Modules.General {
    public partial class GeneralModule : VolteModule {
        [Command("Choose")]
        [Summary("Choose an item from a | delimited list.")]
        [Remarks("Usage: |prefix|choose {option1|option2|option3|...}")]
        public async Task Choose([Remainder] string message) {
            var opt = message.Split('|', StringSplitOptions.RemoveEmptyEntries);

            await Reply(Context.Channel,
                CreateEmbed(Context,
                    $"I choose `{opt[new Random().Next(0, opt.Length)]}`."
                )
            );
        }
    }
}