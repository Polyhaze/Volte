using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Qmmands;
using Volte.Commands.Preconditions;
using Volte.Data.Models.EventArgs;
using Volte.Data.Models.Results;
using Volte.Services;

namespace Volte.Commands.Modules
{
    public partial class AdminModule : VolteModule
    {
        public WelcomeService WelcomeService { get; set; }

        [Command("WelcomeChannel", "Wc")]
        [Description("Sets the channel used for welcoming new users for this guild.")]
        [Remarks("Usage: |prefix|welcomechannel {#channel}")]
        [RequireGuildAdmin]
        public Task<VolteCommandResult> WelcomeChannelAsync([Remainder] SocketTextChannel channel)
        {
            var data = Db.GetData(Context.Guild);
            data.Configuration.Welcome.WelcomeChannel = channel.Id;
            Db.UpdateData(data);
            return Ok($"Set this server's welcome channel to {channel.Mention}.");
        }

        [Command("WelcomeMessage", "Wmsg")]
        [Description(
            "Sets or shows the welcome message used to welcome new users for this guild. Only in effect when the bot isn't using the welcome image generating API.")]
        [Remarks("Usage: |prefix|welcomemessage [message]")]
        [RequireGuildAdmin]
        public async Task<VolteCommandResult> WelcomeMessageAsync([Remainder] string message = null)
        {
            var data = Db.GetData(Context.Guild);

            if (message is null)
            {
                return Ok(
                    $"The current welcome message for this server is ```\n{data.Configuration.Welcome.WelcomeMessage}```");
            }

            data.Configuration.Welcome.WelcomeMessage = message;
            Db.UpdateData(data);
            var welcomeChannel = await Context.Guild.GetTextChannelAsync(data.Configuration.Welcome.WelcomeChannel);
            var sendingTest = data.Configuration.Welcome.WelcomeChannel is 0 || welcomeChannel is null
                ? "Not sending a test message as you do not have a welcome channel set." +
                  "Set a welcome channel to fully complete the setup!"
                : $"Sending a test message to {welcomeChannel.Mention}.";
            if (welcomeChannel is null || data.Configuration.Welcome.WelcomeChannel is 0) return None();

            await WelcomeService.JoinAsync(new UserJoinedEventArgs(Context.User));
            return Ok($"Set this server's welcome message to ```{message}```\n\n{sendingTest}");
        }

        [Command("WelcomeColor", "WelcomeColour", "Wcl")]
        [Description("Sets the color used for welcome embeds for this guild.")]
        [Remarks("Usage: |prefix|welcomecolor {r} {g} {b}")]
        [RequireGuildAdmin]
        public Task<VolteCommandResult> WelcomeColorAsync(int r, int g, int b)
        {
            if (r > 255 || g > 255 || b > 255)
            {
                return BadRequest(
                    "You cannot have an RGB value greater than 255. Either the R, G, or B value you entered exceeded 255 in value.");
            }

            var data = Db.GetData(Context.Guild);
            data.Configuration.Welcome.WelcomeColor = new Color(r, g, b).RawValue;
            Db.UpdateData(data);
            return Ok($"Successfully set this server's welcome message embed colour to `{r}, {g}, {b}`!");
        }

        [Command("LeavingMessage", "Lmsg")]
        [Description("Sets or shows the leaving message used to say bye for this guild.")]
        [Remarks("Usage: |prefix|leavingmessage [message]")]
        [RequireGuildAdmin]
        public async Task<VolteCommandResult> LeavingMessageAsync([Remainder] string message = null)
        {
            var data = Db.GetData(Context.Guild);

            if (message is null)
            {
                return Ok(
                    $"The current leaving message for this server is ```\n{data.Configuration.Welcome.WelcomeMessage}```");
            }
            else
            {
                data.Configuration.Welcome.LeavingMessage = message;
                Db.UpdateData(data);
                var welcomeChannel = await Context.Guild.GetTextChannelAsync(data.Configuration.Welcome.WelcomeChannel);
                var sendingTest = data.Configuration.Welcome.WelcomeChannel == 0 || welcomeChannel is null
                    ? "Not sending a test message, as you do not have a welcome channel set. " +
                      "Set a welcome channel to fully complete the setup!"
                    : $"Sending a test message to {welcomeChannel.Mention}.";
                if (welcomeChannel is null || data.Configuration.Welcome.WelcomeChannel is 0) return None();

                await WelcomeService.LeaveAsync(new UserLeftEventArgs(Context.User));
                return Ok($"Set this server's leaving message to ```{message}```\n\n{sendingTest}");
            }
        }
    }
}