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
        private readonly DatabaseService _db;
        private readonly IServiceProvider _provider;

        public BlacklistService(LoggingService loggingService, DatabaseService databaseService,
            IServiceProvider provider)
        {
            _logger = loggingService;
            _db = databaseService;
            _provider = provider;
        }

        public override Task DoAsync(EventArgs args)
            => CheckMessageAsync(args.Cast<MessageReceivedEventArgs>());

        private async Task CheckMessageAsync(MessageReceivedEventArgs args)
        {
            if (!args.Data.Configuration.Moderation.Blacklist.Any()) return;
            _logger.Debug(LogSource.Volte, "Checking a message for blacklisted words.");
            if (args.Context.User.IsAdmin(_provider))
            {
                _logger.Debug(LogSource.Volte, "Aborting check because the user is a guild admin.");
                return;
            }

            foreach (var word in args.Data.Configuration.Moderation.Blacklist)
                if (args.Message.Content.ContainsIgnoreCase(word))
                {
                    
                    await args.Message.TryDeleteAsync();
                    _logger.Debug(LogSource.Volte, $"Deleted a message for containing {word}.");

                    

                    var action = args.Data.Configuration.Moderation.BlacklistAction;
                    if (action is BlacklistAction.Nothing) return;

                    await PerformBlacklistAction(args.Context, args.Message.Author.Cast<SocketGuildUser>(), action,
                        word);
                }
        }

        private async Task PerformBlacklistAction(VolteContext ctx, SocketGuildUser member, BlacklistAction action,
            string word)
        {
            switch (action)
            {
                case BlacklistAction.Warn:
                    await WarnAsync(ctx, member, $"Used blacklisted word \"{word}\".");
                    break;
                case BlacklistAction.Kick:
                    await member.KickAsync($"Used blacklisted word \"{word}\".");
                    break;
                case BlacklistAction.Ban:
                    await member.BanAsync(7, $"Used blacklisted word \"{word}\".");
                    break;
            }
        }

        private async Task WarnAsync(VolteContext ctx, SocketGuildUser member, string reason)
        {
            await ModerationModule.WarnAsync(ctx.User, ctx.GuildData, member, _db, _logger, reason);
        }
    }
}