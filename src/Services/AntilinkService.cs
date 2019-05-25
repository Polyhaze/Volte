using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord;
using Gommon;
using RestSharp;
using Volte.Data.Models.EventArgs;
using Volte.Extensions;

namespace Volte.Services
{
    [Service("Antilink", "The main Service for checking links sent in chat.")]
    public sealed class AntilinkService
    {
        private readonly Regex _invitePattern = new Regex(@"discord(?:\.gg|\.io|\.me|app\.com\/invite)\/([\w\-]+)", RegexOptions.Compiled);

        internal async Task CheckMessageAsync(MessageReceivedEventArgs args)
        {

            if (!args.Data.Configuration.Moderation.Antilink || args.Context.User.IsAdmin()) return;

            var matches = _invitePattern.Matches(args.Message.Content);
            if (!matches.Any()) return;

            await args.Message.DeleteAsync(new RequestOptions
                {AuditLogReason = "Deleted as it contained an invite link."});
            var m = await args.Context.CreateEmbed("Don't send invites here.").SendToAsync(args.Context.Channel);
            _ = Executor.ExecuteAfterDelayAsync(3000, async () => await m.DeleteAsync());

        }
    }
}