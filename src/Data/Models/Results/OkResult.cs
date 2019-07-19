using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Volte.Commands;

namespace Volte.Data.Models.Results
{
    public class OkResult : BaseResult
    {
        public OkResult(string text, params FileAttachment[] attachments)
        {
            Message = text;
            Embed = null;
            Attachments = attachments;
        }

        public OkResult(EmbedBuilder embed, params FileAttachment[] attachments)
        {
            Message = null;
            Embed = embed;
            Attachments = attachments;
        }

        public override bool IsSuccessful => true;

        private string Message { get; }
        private EmbedBuilder Embed { get; }
        private FileAttachment[] Attachments { get; }

        public override async Task<ResultCompletionData> ExecuteResultAsync(VolteContext ctx)
        {
            var currentUser = await ctx.Guild.GetCurrentUserAsync();
            if (!currentUser.GetPermissions(ctx.Channel).SendMessages) return new ResultCompletionData();
            if (Attachments.Length > 0 && !currentUser.GetPermissions(ctx.Channel).AttachFiles)
                return new ResultCompletionData();

            var messages = new List<IUserMessage>();
            if (Attachments.Length == 1)
            {
                var attachment = Attachments.First();
                messages.Add(await ctx.Channel.SendFileAsync(attachment.Stream, attachment.Filename, Message, false,
                    Embed?.Build()));
            }
            else if (Attachments.Length > 0)
            {
                foreach (var attach in Attachments)
                {
                    messages.Add(await ctx.Channel.SendFileAsync(attach.Stream, attach.Filename));
                }

                if (Message != null || Embed != null)
                    messages.Add(await ctx.Channel.SendMessageAsync(Message, false, Embed?.Build()));
            }
            else
            {
                messages.Add(await ctx.Channel.SendMessageAsync(Message, false, Embed?.Build()));
            }

            return new ResultCompletionData(messages.ToArray());
        }
    }
}