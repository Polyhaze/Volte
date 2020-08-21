using System;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using Gommon;
using Volte.Commands.Checks;
using Volte.Core;
using Volte.Core.Models;
using Volte.Core.Models.Guild;
using Volte.Services;

namespace Volte.Commands.Modules
{
    [RequireGuildModerator]
    public sealed partial class ModerationModule : VolteModule
    {
        public static async Task WarnAsync(
            DiscordMember issuer, 
            GuildData data, 
            DiscordMember member, 
            DatabaseService db, 
            LoggingService logger, 
            string reason)
        {
            data.Extras.Warns.Add(new Warn
            {
                User = member.Id,
                Reason = reason,
                Issuer = issuer.Id,
                Date = DateTimeOffset.Now
            });
            db.UpdateData(data);
            var embed = new DiscordEmbedBuilder().WithColor(member.GetHighestRoleWithColor()?.Color ?? new DiscordColor(Config.SuccessColor)).WithAuthor(issuer)
                .WithDescription($"You've been warned in **{issuer.Guild.Name}** for **{reason}**.").Build();

            if (!await member.TrySendMessageAsync(
                embed: embed))
            {
                logger.Warn(LogSource.Volte,
                    $"encountered a 403 when trying to message {member}!");
            }
        }
    }
}