using System.Threading.Tasks;
using Gommon;
using Volte.Data.Models.EventArgs;
using Volte.Extensions;

namespace Volte.Services
{
    [Service("PingChecks", "The main Service used for checking if any given message contains mass mentions.")]
    public sealed class PingChecksService
    {
        public async Task CheckMessageAsync(MessageReceivedEventArgs args)
        {
            if (args.Data.Configuration.Moderation.MassPingChecks && !args.Context.User.IsAdmin())
            {
                var content = args.Message.Content;
                if (content.ContainsIgnoreCase("@everyone") ||
                    content.ContainsIgnoreCase("@here") ||
                    args.Message.MentionedUserIds.Count > 10)
                {
                    await args.Message.DeleteAsync();
                }
            }
        }
    }
}