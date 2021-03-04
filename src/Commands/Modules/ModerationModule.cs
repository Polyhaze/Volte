using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Gommon;
using Volte.Core.Attributes;
using Volte.Core.Models;
using Volte.Core.Models.Guild;
using Volte.Services;

namespace Volte.Commands.Modules
{
    [RequireGuildModerator]
    public sealed partial class ModerationModule : VolteModule
    {
        public static async Task WarnAsync(SocketGuildUser issuer, GuildData data, SocketGuildUser member, DatabaseService db, LoggingService logger, string reason)
        {
            data.Extras.Warns.Add(new Warn
            {
                User = member.Id,
                Reason = reason,
                Issuer = issuer.Id,
                Date = DateTimeOffset.Now
            });
            db.UpdateData(data);

            if (!await member.TrySendMessageAsync(
                embed: new EmbedBuilder().WithSuccessColor().WithAuthor(issuer)
                    .WithDescription($"You've been warned in **{issuer.Guild.Name}** for `{reason}`.").Build()))
            {
                logger.Warn(LogSource.Volte,
                    $"encountered a 403 when trying to message {member}!");
            }
        }
    }
}