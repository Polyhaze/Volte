using System;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using Gommon;
using Volte.Core;
using Volte.Core.Entities;
using Volte.Services;

namespace Volte.Commands.Modules
{
    public sealed partial class ModerationModule
    {
        public static async Task WarnAsync(
            DiscordMember issuer,
            DiscordMember member, 
            DatabaseService db, 
            LoggingService logger, 
            string reason)
        {
            db.ModifyAndSaveData(issuer.Guild.Id, data =>
            {
                data.Extras.Warns.Add(new Warn
                {
                    User = member.Id,
                    Reason = reason,
                    Issuer = issuer.Id,
                    Date = DateTimeOffset.Now
                });
                return data;
            });
            
            var embed = new DiscordEmbedBuilder()
                .WithColor(member.GetHighestRoleWithColor()?.Color ?? new DiscordColor(Config.SuccessColor))
                .WithAuthor(issuer)
                .WithDescription($"You've been warned in **{issuer.Guild.Name}** for **{reason}**.")
                .Build();

            if (!await member.TrySendMessageAsync(
                embed: embed))
            {
                logger.Warn(LogSource.Volte,
                    $"encountered a 403 when trying to message {member}!");
            }
        }
    }
}