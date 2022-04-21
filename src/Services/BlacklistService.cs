﻿using System.Linq;
using System.Threading.Tasks;
using Discord;
using Gommon;
using Volte.Entities;
using Volte.Helpers;

namespace Volte.Services
{
    public sealed class BlacklistService : IVolteService
    {
        public async Task CheckMessageAsync(MessageReceivedEventArgs args)
        {
            if (args.Data.Configuration.Moderation.Blacklist.IsEmpty()) return;
            Logger.Debug(LogSource.Volte, "Checking a message for blacklisted words.");
            if (!args.Context.IsAdmin(args.Context.User))
            {
                if (args.Data.Configuration.Moderation.Blacklist.AnyGet(x => args.Message.Content.ContainsIgnoreCase(x),
                    out var word))
                {
                    if (await args.Message.TryDeleteAsync())
                    {
                        Logger.Debug(LogSource.Volte, $"Deleted a message for containing {word}.");
                        await args.Data.Configuration.Moderation.BlacklistAction.PerformAsync(args.Context,
                            args.Context.User, word);
                    }
                }
            }
            else
                Logger.Debug(LogSource.Volte, "Aborting check because the user is a guild admin.");
        }
    }
}