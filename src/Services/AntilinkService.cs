using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Gommon;
using Humanizer;
using Volte.Core.Entities;
using Volte.Core.Helpers;

namespace Volte.Services
{
    public sealed class AntilinkService : IVolteService
    {
        private readonly Regex _invitePattern =
            new Regex(@"discord(?:\.gg|\.io|\.me|app\.com\/invite)\/([\w\-]+)", RegexOptions.Compiled);

        public async Task CheckMessageAsync(MessageReceivedEventArgs args)
        {
            if (!args.Data.Configuration.Moderation.Antilink ||
                args.Context.IsAdmin(args.Context.User)) return;

            Logger.Debug(LogSource.Volte,
                $"Checking a message in #{args.Context.Channel.Name} ({args.Context.Guild.Name}) for Discord invite URLs.");
            
            if (!_invitePattern.IsMatch(args.Message.Content, out _))
            {
                Logger.Debug(LogSource.Volte,
                    $"Message checked in #{args.Context.Channel.Name} ({args.Context.Guild.Name}) did not contain any detectable invites; aborted.");
                return;
            }

            _ = await args.Message.TryDeleteAsync("Deleted as it contained an invite link.");
            var m = await args.Context.CreateEmbed($"{args.Message.Author.Mention}, don't send invites here.").SendToAsync(args.Context.Channel);
            Logger.Debug(LogSource.Volte,
                $"Deleted a message in #{args.Context.Channel.Name} ({args.Context.Guild.Name}) for containing a Discord invite URL.");
            _ = Executor.ExecuteAfterDelayAsync(3.Seconds(), () => m.TryDeleteAsync());
        }
    }
}