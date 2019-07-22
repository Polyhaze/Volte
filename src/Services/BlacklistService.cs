using System.Threading.Tasks;
using Gommon;
using Volte.Data.Models.EventArgs;

namespace Volte.Services
{
    [Service("Blacklist", "The main Service for checking messages for blacklisted words/phrases in user's messages.")]
    public sealed class BlacklistService
    {
        internal async Task CheckMessageAsync(MessageReceivedEventArgs args)
        {
            foreach (var word in args.Data.Configuration.Moderation.Blacklist)
                if (args.Message.Content.ContainsIgnoreCase(word))
                {
                    await args.Message.DeleteAsync();
                    return;
                }
        }
    }
}