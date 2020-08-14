using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Qmmands;
using Volte.Core.Models.EventArgs;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public sealed partial class WelcomeModule
    {
        [Command("Channel", "C")]
        [Description("Sets the channel used for welcoming new users for this guild.")]
        [Remarks("welcome channel {Channel}")]
        public Task<ActionResult> WelcomeChannelAsync([Remainder] SocketTextChannel channel)
        {
            ModifyData(data =>
            {
                data.Configuration.Welcome.WelcomeChannel = channel.Id;
                return data;
            });
            return Ok($"Set this guild's welcome channel to {channel.Mention}.");
        }

        [Command("Message", "Msg")]
        [Description(
            "Sets or shows the welcome message used to welcome new users for this guild.")]
        [Remarks("welcomemessage [String]")]
        public Task<ActionResult> WelcomeMessageAsync([Remainder] string message = null)
        {
            if (message is null)
            {
                return Ok(new StringBuilder()
                    .AppendLine("The current welcome message for this guild is: ```")
                    .AppendLine(Context.GuildData.Configuration.Welcome.WelcomeMessage)
                    .Append("```")
                    .ToString());
            }

            ModifyData(data =>
            {
                data.Configuration.Welcome.WelcomeMessage = message;
                return data;
            });
            var welcomeChannel = Context.Guild.GetTextChannel(Context.GuildData.Configuration.Welcome.WelcomeChannel);
            var sendingTest = Context.GuildData.Configuration.Welcome.WelcomeChannel is 0 || welcomeChannel is null
                ? "Not sending a test message as you do not have a welcome channel set." +
                  "Set a welcome channel to fully complete the setup!"
                : $"Sending a test message to {welcomeChannel.Mention}. This message will have all formatting and placeholders replaced with an actual value.";
            if (welcomeChannel is null || Context.GuildData.Configuration.Welcome.WelcomeChannel is 0) return None();

            return Ok(new StringBuilder()
                .AppendLine($"Set this guild's welcome message to ```{message}```")
                .AppendLine()
                .AppendLine($"{sendingTest}").ToString(),
                _ => WelcomeService.JoinAsync(new UserJoinedEventArgs(Context.User)));
        }

        [Command("Color", "Colour", "Cl")]
        [Description("Sets the color used for welcome embeds for this guild.")]
        [Remarks("welcome color {Color}")]
        public Task<ActionResult> WelcomeColorAsync([Remainder] Color color)
        {
            ModifyData(data =>
            {
                data.Configuration.Welcome.WelcomeColor = color.RawValue;
                return data;
            });
            return Ok("Successfully set this guild's welcome message embed color!");
        }

        [Command("LeavingMessage", "Lmsg")]
        [Description("Sets or shows the leaving message used to say bye for this guild.")]
        [Remarks("welcome leavingmessage [String]")]
        public Task<ActionResult> LeavingMessageAsync([Remainder] string message = null)
        {

            if (message is null)
            {
                return Ok(new StringBuilder()
                    .AppendLine("The current leaving message for this guild is ```")
                    .AppendLine(Context.GuildData.Configuration.Welcome.LeavingMessage)
                    .Append("```")
                    .ToString());
            }

            ModifyData(data =>
            {
                data.Configuration.Welcome.LeavingMessage = message;
                return data;
            });
            var welcomeChannel = Context.Guild.GetTextChannel(Context.GuildData.Configuration.Welcome.WelcomeChannel);
                var sendingTest = Context.GuildData.Configuration.Welcome.WelcomeChannel == 0 || welcomeChannel is null
                    ? "Not sending a test message, as you do not have a welcome channel set. " +
                      "Set a welcome channel to fully complete the setup!"
                    : $"Sending a test message to {welcomeChannel.Mention}.";
                if (welcomeChannel is null || Context.GuildData.Configuration.Welcome.WelcomeChannel is 0) return None();

            return Ok(new StringBuilder()
                    .AppendLine($"Set this server's leaving message to ```{message}```")
                    .AppendLine()
                    .AppendLine($"{sendingTest}").ToString(),
                _ => WelcomeService.LeaveAsync(new UserLeftEventArgs(Context.User)));
        }

        [Command("DmMessage", "Dmm")]
        [Description("Sets the message to be (attempted to) sent to members upon joining.")]
        [Remarks("welcome dmmessage [String]")]
        public Task<ActionResult> WelcomeDmMessageAsync(string message = null)
        {
            if (message is null)
            {
                return Ok(
                    $"Unset the WelcomeDmMessage that was previously set to: {Format.Code(Context.GuildData.Configuration.Welcome.WelcomeDmMessage)}");
            }

            ModifyData(data =>
            {
                data.Configuration.Welcome.WelcomeDmMessage = message;
                return data;
            });
            return Ok($"Set the WelcomeDmMessage to: ```{message}```\n\nAttempting to send a test message.", 
                _ => WelcomeService.JoinDmAsync(new UserJoinedEventArgs(Context.User)));
        }
    }
}