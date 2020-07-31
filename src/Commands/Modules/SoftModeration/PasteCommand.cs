using System.Threading.Tasks;

using Discord;
using Discord.Commands;

using BrackeysBot.Services;

namespace BrackeysBot.Commands
{
    public partial class SoftModerationModule : BrackeysBotModule
    {
        public CodeblockService Codeblock { get; set; }

        [Command("paste")]
        [Summary("Uploads the message with the specified ID to the PasteMyst server.")]
        [Remarks("paste <id>")]
        [RequireModerator]
        public async Task PasteMessageAsync(
            [Summary("The ID of the message to paste.")] ulong id)
        {
            IMessage message = await Context.Channel.GetMessageAsync(id);
            string url = await Codeblock.PasteMessage(message);

            await GetDefaultBuilder()
                .WithAuthor("Pasted!", message.Author.EnsureAvatarUrl(), url)
                .WithDescription($"The message by {message.Author.Mention} has been pasted!\n[Click here to view it!]({url})")
                .WithColor(Color.Green)
                .Build()
                .SendToChannel(Context.Channel);

            await message.DeleteAsync();
        }
    }
}
