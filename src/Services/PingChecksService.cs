using System.Threading.Tasks;
using Volte.Entities;
using Volte.Helpers;

namespace Volte.Services
{
    public sealed class PingChecksService : IVolteService
    {
        public async Task CheckMessageAsync(MessageReceivedEventArgs args)
        {
            if (args.Data.Configuration.Moderation.MassPingChecks &&
                !args.Context.IsAdmin(args.Context.User))
            {
                Logger.Debug(LogSource.Service,
                    "Received a message to check for ping threshold violations.");
                if (args.Message.MentionedEveryone || args.Message.MentionedUsers.Count > 10)
                {
                    await args.Message.DeleteAsync();
                    Logger.Debug(LogSource.Service,
                        "Deleted a message for violating the ping threshold.");
                }
            }
        }
    }
}