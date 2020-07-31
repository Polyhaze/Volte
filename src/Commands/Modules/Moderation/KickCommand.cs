using System.Threading.Tasks;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace BrackeysBot.Commands
{
    public partial class ModerationModule : BrackeysBotModule
    {
        [Command("kick")]
        [Summary("Kicks a user from the server.")]
        [Remarks("kick <user> <reason>")]
        [RequireModerator]
        [RequireBotPermission(GuildPermission.KickMembers)]
        public async Task KickUser(
            [Summary("The user to kick.")] SocketGuildUser user,
            [Summary("The reason to kick the user for."), Remainder] string reason = DefaultReason)
        {
            await user.KickAsync(reason);

            Moderation.AddInfraction(user, Infraction.Create(Moderation.RequestInfractionID())
                .WithType(InfractionType.Kick)
                .WithModerator(Context.User)
                .WithDescription(reason));

            await ModerationLog.CreateEntry(ModerationLogEntry.New
                .WithDefaultsFromContext(Context)
                .WithActionType(ModerationActionType.Kick)
                .WithReason(reason)
                .WithTarget(user), Context.Channel);
        }
    }
}
