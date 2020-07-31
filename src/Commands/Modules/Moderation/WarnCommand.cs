using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace BrackeysBot.Commands
{
    public partial class ModerationModule : BrackeysBotModule
    {
        [Command("warn")]
        [Summary("Warns a user with a specified reason.")]
        [Remarks("warn <user> <reason>")]
        [RequireModerator]
        public async Task WarnUserAsync(
            [Summary("The user to warn.")] SocketGuildUser user,
            [Summary("The reason to warn the user."), Remainder] string reason)
        {
            UserData data = Data.UserData.GetUser(user.Id);

            EmbedBuilder builder = new EmbedBuilder()
                .WithColor(Color.Orange);

            bool printInfractions = data != null && data.Infractions?.Count > 0;
            string previousInfractions = null;
            
            // Collect the previous infractions before applying new ones, otherwise we will also collect this
            //  new infraction when printing them
            if (printInfractions)
            {
                previousInfractions = string.Join('\n', data.Infractions.OrderByDescending(i => i.Time).Select(i => i.ToString()));
            }

            Moderation.AddInfraction(user, Infraction.Create(Moderation.RequestInfractionID())
                .WithType(InfractionType.Warning)
                .WithModerator(Context.User)
                .WithDescription(reason));

            await ModerationLog.CreateEntry(ModerationLogEntry.New
                .WithDefaultsFromContext(Context)
                .WithActionType(ModerationActionType.Warn)
                .WithReason(reason)
                .WithTarget(user)
                .WithAdditionalInfo(previousInfractions), Context.Channel);
        }
    }
}
