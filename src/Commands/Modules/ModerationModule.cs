using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Gommon;
using Volte.Core;
using Volte.Core.Models;
using Volte.Core.Models.Guild;
using Volte.Services;

namespace Volte.Commands.Modules
{
    public sealed partial class ModerationModule : VolteModule
    {
        public static async Task WarnAsync(
            SocketGuildUser issuer, 
            GuildData data, 
            SocketGuildUser member, 
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
            var embed = new EmbedBuilder().WithColor(member.GetHighestRoleWithColor()?.Color ?? new Color(Config.SuccessColor)).WithAuthor(issuer)
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