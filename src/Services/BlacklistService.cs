using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Gommon;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Qmmands;
using Volte.Commands;
using Volte.Commands.Modules;
using Volte.Core.Models;
using Volte.Core.Models.EventArgs;
using Volte.Core.Models.Guild;

namespace Volte.Services
{
    public sealed class BlacklistService : VolteEventService
    {
        private readonly LoggingService _logger;

        public BlacklistService(LoggingService loggingService)
        {
            _logger = loggingService;
        }

        public override Task DoAsync(EventArgs args)
            => CheckMessageAsync(args.Cast<MessageReceivedEventArgs>() ?? throw new InvalidOperationException($"AutoRole was triggered with a null event. Expected: {nameof(MessageReceivedEventArgs)}, Received: {args.GetType().Name}"));

        private async Task CheckMessageAsync(MessageReceivedEventArgs args)
        {
            if (args.Data.Configuration.Moderation.Blacklist.IsEmpty()) return;
            _logger.Debug(LogSource.Volte, "Checking a message for blacklisted words.");
            if (!args.Context.User.IsAdmin(args.Context))
            {
                foreach (var word in args.Data.Configuration.Moderation.Blacklist.Where(word => args.Message.Content.ContainsIgnoreCase(word)))
                {
                    await args.Message.TryDeleteAsync();
                    _logger.Debug(LogSource.Volte, $"Deleted a message for containing {word}.");
                    if (args.Data.Configuration.Moderation.BlacklistAction.IsValid(out var action))
                        await action.PerformAsync(args.Context, args.Message.Author.Cast<SocketGuildUser>(), word);
                }
            }
        }
    }
}