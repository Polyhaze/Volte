using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Qmmands;
using Volte.Commands.Checks;
using Volte.Core.Models.EventArgs;
using Volte.Commands.Results;
using Volte.Services;

namespace Volte.Commands.Modules
{
    public sealed partial class AdminModule : VolteModule
    {
        public WelcomeService WelcomeService { get; set; }

        [Command("WelcomeChannel", "Wc")]
        [Description("Sets the channel used for welcoming new users for this guild.")]
        [Remarks("Usage: |prefix|welcomechannel {#channel}")]
        [RequireGuildAdmin]
        public Task<ActionResult> WelcomeChannelAsync([Remainder] SocketTextChannel channel)
        {
            Context.GuildData.Configuration.Welcome.WelcomeChannel = channel.Id;
            Db.UpdateData(Context.GuildData);
            return Ok($"Set this server's welcome channel to {channel.Mention}.");
        }

        [Command("WelcomeMessage", "Wmsg")]
        [Description(
            "Sets or shows the welcome message used to welcome new users for this guild. Only in effect when the bot isn't using the welcome image generating API.")]
        [Remarks("Usage: |prefix|welcomemessage [message]")]
        [RequireGuildAdmin]
        public Task<ActionResult> WelcomeMessageAsync([Remainder] string message = null)
        {
            if (message is null)
            {
                return Ok(
                    $"The current welcome message for this server is ```\n{Context.GuildData.Configuration.Welcome.WelcomeMessage}```");
            }

            Context.GuildData.Configuration.Welcome.WelcomeMessage = message;
            Db.UpdateData(Context.GuildData);
            var welcomeChannel = Context.Guild.GetTextChannel(Context.GuildData.Configuration.Welcome.WelcomeChannel);
            var sendingTest = Context.GuildData.Configuration.Welcome.WelcomeChannel is 0 || welcomeChannel is null
                ? "Not sending a test message as you do not have a welcome channel set." +
                  "Set a welcome channel to fully complete the setup!"
                : $"Sending a test message to {welcomeChannel.Mention}.";
            if (welcomeChannel is null || Context.GuildData.Configuration.Welcome.WelcomeChannel is 0) return None();

            return Ok($"Set this server's welcome message to ```{message}```\n\n{sendingTest}",
                _ => WelcomeService.JoinAsync(new UserJoinedEventArgs(Context.User)));
        }

        [Command("WelcomeColor", "WelcomeColour", "Wcl")]
        [Description("Sets the color used for welcome embeds for this guild.")]
        [Remarks("Usage: |prefix|welcomecolor {color}")]
        [RequireGuildAdmin]
        public Task<ActionResult> WelcomeColorAsync([Remainder] Color color)
        {
            Context.GuildData.Configuration.Welcome.WelcomeColor = color.RawValue;
            Db.UpdateData(Context.GuildData);
            return Ok("Successfully set this server's welcome message embed color!");
        }

        [Command("LeavingMessage", "Lmsg")]
        [Description("Sets or shows the leaving message used to say bye for this guild.")]
        [Remarks("Usage: |prefix|leavingmessage [message]")]
        [RequireGuildAdmin]
        public Task<ActionResult> LeavingMessageAsync([Remainder] string message = null)
        {

            if (message is null)
            {
                return Ok(
                    $"The current leaving message for this server is ```\n{Context.GuildData.Configuration.Welcome.WelcomeMessage}```");
            }

            Context.GuildData.Configuration.Welcome.LeavingMessage = message;
                Db.UpdateData(Context.GuildData);
                var welcomeChannel = Context.Guild.GetTextChannel(Context.GuildData.Configuration.Welcome.WelcomeChannel);
                var sendingTest = Context.GuildData.Configuration.Welcome.WelcomeChannel == 0 || welcomeChannel is null
                    ? "Not sending a test message, as you do not have a welcome channel set. " +
                      "Set a welcome channel to fully complete the setup!"
                    : $"Sending a test message to {welcomeChannel.Mention}.";
                if (welcomeChannel is null || Context.GuildData.Configuration.Welcome.WelcomeChannel is 0) return None();

                return Ok($"Set this server's leaving message to ```{message}```\n\n{sendingTest}",
                    _ => WelcomeService.LeaveAsync(new UserLeftEventArgs(Context.User)));
        }
    }
}