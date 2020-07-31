using System;
using System.Linq;
using System.Threading.Tasks;

using Discord.Commands;

namespace BrackeysBot.Core.Models
{
    [Name("Alias")]
    [Summary("Specifies one or multiple aliases for a command.")]
    public class AliasFeature : CustomCommandFeature
    {
        public string[] Aliases { get; set; }

        public override void FillArguments(string arguments)
            => Aliases = arguments.Split(' ');
        public override Task Execute(ICommandContext context)
            => Task.CompletedTask;

        public bool Matches(string name)
            => Aliases.Any(a => name.Equals(a, StringComparison.InvariantCultureIgnoreCase));

        public override string ToString()
            => $"Matches with \"{string.Join(", ", Aliases)}\".";
    }
}
