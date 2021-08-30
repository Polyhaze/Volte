using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Qmmands;
using Volte.Commands;
using Volte.Entities;
using Volte.Helpers;
using Volte.Services;

namespace Volte.Commands.Modules
{
    [Group("Welcome", "W")]
    [RequireGuildAdmin]
    public class WelcomeModule : VolteModule
    {
        public WelcomeService Service { get; set; }

        [Command, DummyCommand, Description("The command group for modifying the Welcome system.")]
        public async Task<ActionResult> BaseAsync() =>
            Ok(await CommandHelper.CreateCommandEmbedAsync(Context.Command, Context));

        [Command("Channel", "C")]
        [Description("Sets the channel used for welcoming new users for this guild.")]
        public Task<ActionResult> WelcomeChannelAsync(
            [Remainder, Description("The channel to use for welcoming messages.")]
            SocketTextChannel channel)
        {
            Context.Modify(data => data.Configuration.Welcome.WelcomeChannel = channel.Id);
            return Ok($"Set this guild's welcome channel to {channel.Mention}.");
        }

        [Command("Join")]
        [Description("Sets or shows the welcome message used to welcome new users for this guild.")]
        [ShowPlaceholdersInHelp]
        public Task<ActionResult> WelcomeMessageAsync([Remainder] string message = null)
        {
            if (message is null)
                return Ok($"The current welcome message for this guild is: {Format.Code(Context.GuildData.Configuration.Welcome.WelcomeMessage, string.Empty)}");

            Context.Modify(data => data.Configuration.Welcome.WelcomeMessage = message);
            var welcomeChannel = Context.Guild.GetTextChannel(Context.GuildData.Configuration.Welcome.WelcomeChannel);
            var sendingTest = welcomeChannel is null
                ? "Not sending a test message as you do not have a welcome channel set." +
                  "Set a welcome channel to fully complete the setup!"
                : $"Sending a test message to {welcomeChannel.Mention}.";

            return Ok(new StringBuilder()
                    .AppendLine($"Set this guild's welcome message to: {Format.Code(message, string.Empty)}")
                    .AppendLine()
                    .AppendLine($"{sendingTest}"),
                _ =>  Service.JoinAsync(new UserJoinedEventArgs(Context.User)));
        }

        [Command("Color", "Colour", "Cl")]
        [Description("Sets the color used for welcome embeds for this guild.")]
        public Task<ActionResult> WelcomeColorAsync([Remainder] Color color)
        {
            Context.GuildData.Configuration.Welcome.WelcomeColor = color.RawValue;
            Db.Save(Context.GuildData);
            return Ok("Successfully set this guild's welcome message embed color!");
        }

        [Command("Left")]
        [Description("Sets or shows the leaving message used to say bye for this guild.")]
        [ShowPlaceholdersInHelp]
        public Task<ActionResult> LeavingMessageAsync([Remainder] string message = null)
        {
            if (message is null)
                return Ok(new StringBuilder()
                    .AppendLine(
                        $"The current leaving message for this guild is: {Format.Code(Context.GuildData.Configuration.Welcome.LeavingMessage, string.Empty)}"));

            Context.GuildData.Configuration.Welcome.LeavingMessage = message;
            Db.Save(Context.GuildData);
            var welcomeChannel = Context.Guild.GetTextChannel(Context.GuildData.Configuration.Welcome.WelcomeChannel);
            var sendingTest = welcomeChannel is null
                ? "Not sending a test message, as you do not have a welcome channel set. " +
                  "Set a welcome channel to fully complete the setup!"
                : $"Sending a test message to {welcomeChannel.Mention}.";

            return Ok(new StringBuilder()
                    .AppendLine($"Set this server's leaving message to: {Format.Code(message, string.Empty)}")
                    .AppendLine()
                    .AppendLine($"{sendingTest}"),
                _ => Service.LeaveAsync(new UserLeftEventArgs(Context.User)));
        }

        [Command("Dm")]
        [Description("Sets or disables the message to be (attempted to) sent to members upon joining.")]
        [Remarks("Using this command without any arguments will __reset__ the DM message.")]
        [ShowPlaceholdersInHelp]
        public Task<ActionResult> WelcomeDmMessageAsync(
            [Remainder, Description("The message you want to be DM'd to users when they join.")]
            string message = null)
        {
            if (message is null)
                return Ok(
                    $"Unset the WelcomeDmMessage that was previously set to: {Format.Code(Context.GuildData.Configuration.Welcome.WelcomeDmMessage)}");

            Context.GuildData.Configuration.Welcome.WelcomeDmMessage = message;
            Db.Save(Context.GuildData);
            return Ok($"Set the WelcomeDmMessage to: {Format.Code(message, string.Empty)}");
        }
    }
}