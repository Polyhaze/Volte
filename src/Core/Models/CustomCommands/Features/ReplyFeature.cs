using System.Threading.Tasks;

using Discord.Commands;

namespace BrackeysBot.Core.Models
{
    [Name("Reply")]
    [Summary("Replies to the command with a specified text.")]
    public class ReplyFeature : CustomCommandFeature
    {
        public string Content { get; set; }

        public override void FillArguments(string arguments)
            => Content = arguments;
        public override async Task Execute(ICommandContext context)
            => await context.Channel.SendMessageAsync(Content);

        public override string ToString()
            => $"Reply with \"{Content}\".";
    }
}
