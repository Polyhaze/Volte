using SIVA.Core.Bot.Services.Database.DbTypes;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System;

namespace SIVA.Core.Bot.Services.Database.Repositories.Impl
{
    public class GuildConfigRepository : Repository<GuildConfig>, IGuildConfigRepository
    {
        public GuildConfigRepository(DbContext context) : base(context)
        {
        }

        public IEnumerable<GuildConfig> GetAllGuildConfigs(List<long> availableGuilds) =>
            _set
                .Where(gc => availableGuilds.Contains(gc.Id))
                .Include(gc => gc.CanCloseOwnTicket)
                .Include(gc => gc.GuildOwnerId)
                .Include(gc => gc.SupportChannelName)
                .Include(gc => gc.SupportChannelId)
                .Include(gc => gc.SupportRole)
                .Include(gc => gc.Autorole)
                .Include(gc => gc.CommandPrefix)
                .Include(gc => gc.Leveling)
                .Include(gc => gc.WelcomeChannel)
                .Include(gc => gc.WelcomeMessage)
                .Include(gc => gc.LeavingMessage)
                .Include(gc => gc.WelcomeColour1).Include(gc => gc.WelcomeColour2).Include(gc => gc.WelcomeColour3)
                .Include(gc => gc.EmbedColour1).Include(gc => gc.EmbedColour2).Include(gc => gc.EmbedColour3)
                .Include(gc => gc.MassPengChecks)
                .Include(gc => gc.Antilink)
                .Include(gc => gc.VerifiedGuild)
                .Include(gc => gc.ModRole)
                .Include(gc => gc.AdminRole)
                .Include(gc => gc.IsTodEnabled)
                .Include(gc => gc.ServerLoggingChannel)
                .Include(gc => gc.IsServerLoggingEnabled)
                .Include(gc => gc.AntilinkIgnoredChannels)
                .Include(gc => gc.SelfRoles)
                .Include(gc => gc.Blacklist)
                .Include(gc => gc.CustomCommands)
                .ToList();

        public GuildConfig For(ulong guildId, Func<DbSet<GuildConfig>, IQueryable<GuildConfig>> includes = null)
        {
            GuildConfig config;

            if (includes == null)
            {
                config = _set
                    .Include(gc => gc.CanCloseOwnTicket)
                    .Include(gc => gc.GuildOwnerId)
                    .Include(gc => gc.SupportChannelName)
                    .Include(gc => gc.SupportChannelId)
                    .Include(gc => gc.SupportRole)
                    .Include(gc => gc.Autorole)
                    .Include(gc => gc.CommandPrefix)
                    .Include(gc => gc.Leveling)
                    .Include(gc => gc.WelcomeChannel)
                    .Include(gc => gc.WelcomeMessage)
                    .Include(gc => gc.LeavingMessage)
                    .Include(gc => gc.WelcomeColour1).Include(gc => gc.WelcomeColour2).Include(gc => gc.WelcomeColour3)
                    .Include(gc => gc.EmbedColour1).Include(gc => gc.EmbedColour2).Include(gc => gc.EmbedColour3)
                    .Include(gc => gc.MassPengChecks)
                    .Include(gc => gc.Antilink)
                    .Include(gc => gc.VerifiedGuild)
                    .Include(gc => gc.ModRole)
                    .Include(gc => gc.AdminRole)
                    .Include(gc => gc.IsTodEnabled)
                    .Include(gc => gc.ServerLoggingChannel)
                    .Include(gc => gc.IsServerLoggingEnabled)
                    .Include(gc => gc.AntilinkIgnoredChannels)
                    .Include(gc => gc.SelfRoles)
                    .Include(gc => gc.Blacklist)
                    .Include(gc => gc.CustomCommands)
                    .FirstOrDefault(c => c.ServerId == guildId);
            }
            else
            {
                var set = includes(_set);
                config = set.FirstOrDefault(c => c.ServerId == guildId);
            }

            if (config == null)
            {
                _set.Add((config = new GuildConfig
                {
                    ServerId = guildId,
                    GuildOwnerId = 0,
                    Autorole = "",
                    SupportChannelName = "",
                    SupportRole = "Support",
                    CanCloseOwnTicket = true,
                    SupportChannelId = 00000000000000,
                    CommandPrefix = "$",
                    WelcomeChannel = 0,
                    WelcomeColour1 = 112,
                    WelcomeColour2 = 0,
                    WelcomeColour3 = 251,
                    EmbedColour1 = 112,
                    EmbedColour2 = 0,
                    EmbedColour3 = 251,
                    Antilink = false,
                    VerifiedGuild = false
                }));
                _context.SaveChanges();
            }
            return config;
        }

        public GuildConfig LogSettingsFor(ulong guildId)
        {
            var config = _set.Include(gc => gc.ServerLoggingChannel)
               .FirstOrDefault(x => x.ServerId == guildId);

            if (config == null)
            {
                _set.Add((config = new GuildConfig
                {
                    ServerId = guildId,
                    GuildOwnerId = 0,
                    Autorole = "",
                    SupportChannelName = "",
                    SupportRole = "Support",
                    CanCloseOwnTicket = true,
                    SupportChannelId = 00000000000000,
                    CommandPrefix = "$",
                    WelcomeChannel = 0,
                    WelcomeColour1 = 112,
                    WelcomeColour2 = 0,
                    WelcomeColour3 = 251,
                    EmbedColour1 = 112,
                    EmbedColour2 = 0,
                    EmbedColour3 = 251,
                    Antilink = false,
                    VerifiedGuild = false
                }));
                _context.SaveChanges();
            }
            return config;
        }
    }
}