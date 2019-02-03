using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Volte.Helpers;

namespace Volte.Core.Modules.Admin {
    public partial class AdminModule : VolteModule {
        [Command("WelcomeChannel"), Alias("Wc")]
        [Summary("Sets the channel used for welcoming new users for this guild.")]
        [Remarks("Usage: |prefix|welcomechannel {#channel}")]
        public async Task WelcomeChannel(SocketTextChannel channel) {
            if (!UserUtils.IsAdmin(Context)) {
                await Context.ReactFailure();
                return;
            }

            var config = Db.GetConfig(Context.Guild);
            config.WelcomeChannel = channel.Id;
            Db.UpdateConfig(config);
            await Reply(Context.Channel,
                CreateEmbed(Context, $"Set this server's welcome channel to **{channel.Name}**"));
        }

        [Command("WelcomeMessage"), Alias("Wmsg")]
        [Summary("Sets or shows the welcome message used to welcome new users for this guild.")]
        [Remarks("Usage: |prefix|welcomemessage [message]")]
        public async Task WelcomeMessage([Remainder] string message = null) {
            if (!UserUtils.IsAdmin(Context)) {
                await Context.ReactFailure();
                return;
            }

            var config = Db.GetConfig(Context.Guild);

            if (message is null) {
                await Reply(Context.Channel,
                    CreateEmbed(Context,
                        $"The current welcome message for this server is ```\n{config.WelcomeMessage}```"));
            }
            else {
                config.WelcomeMessage = message;
                Db.UpdateConfig(config);
                var welcomeChannel = await Context.Guild.GetTextChannelAsync(config.WelcomeChannel);
                var sendingTest = config.WelcomeChannel == 0 || welcomeChannel is null
                    ? "Not sending a test message as you do not have a welcome channel set." +
                      "Set a welcome channel to fully complete the setup!"
                    : $"Sending a test message to **{welcomeChannel.Name}**.";
                await Reply(Context.Channel,
                    CreateEmbed(Context,
                        $"Set this server's welcome message to ```{message}```\n\n{sendingTest}"));
                if (welcomeChannel is null) return;
                if (config.WelcomeChannel != 0) {
                    var welcomeMessage = config.WelcomeMessage
                        .Replace("{ServerName}", Context.Guild.Name)
                        .Replace("{UserMention}", Context.User.Mention)
                        .Replace("{UserName}", Context.User.Username)
                        .Replace("{UserTag}", Context.User.Discriminator);
                    var embed = CreateEmbed(Context, welcomeMessage).ToEmbedBuilder()
                        .WithThumbnailUrl(Context.User.GetAvatarUrl());
                    await welcomeChannel.SendMessageAsync(string.Empty, false, embed.Build());
                }
            }
        }

        [Command("WelcomeColor"), Alias("WelcomeColour", "Wcl")]
        [Summary("Sets the color used for welcome embeds for this guild.")]
        [Remarks("Usage: |prefix|welcomecolor {r} {g} {b}")]
        public async Task WelcomeColor(int r, int g, int b) {
            if (!UserUtils.IsAdmin(Context)) {
                await Context.ReactFailure();
                return;
            }

            if (r > 255 || g > 255 || b > 255) {
                await Reply(Context.Channel,
                    CreateEmbed(Context,
                        "You cannot have an RGB value greater than 255. Either the R, G, or B value you entered exceeded 255 in value."));
                return;
            }

            var config = Db.GetConfig(Context.Guild);
            config.WelcomeColorR = r;
            config.WelcomeColorG = g;
            config.WelcomeColorB = b;
            Db.UpdateConfig(config);
            await Reply(Context.Channel,
                CreateEmbed(Context,
                    $"Successfully set this server's welcome message embed colour to `{r}, {g}, {b}`!"));
        }

        [Command("LeavingMessage"), Alias("Lmsg")]
        [Summary("Sets or shows the leaving message used to say bye for this guild.")]
        [Remarks("Usage: |prefix|leavingmessage [message]")]
        public async Task LeavingMessage([Remainder] string message = null) {
            if (!UserUtils.IsAdmin(Context)) {
                await Context.ReactFailure();
                return;
            }

            var config = Db.GetConfig(Context.Guild);

            if (message is null) {
                await Context.Channel.SendMessageAsync(string.Empty, false,
                    CreateEmbed(Context,
                        $"The current leaving message for this server is ```\n{config.WelcomeMessage}```"));
            }
            else {
                config.LeavingMessage = message;
                Db.UpdateConfig(config);
                var welcomeChannel = await Context.Guild.GetTextChannelAsync(config.WelcomeChannel);
                var sendingTest = config.WelcomeChannel == 0 || welcomeChannel is null
                    ? "Not sending a test message, as you do not have a welcome channel set. " +
                      "Set a welcome channel to fully complete the setup!"
                    : $"Sending a test message to **{welcomeChannel.Mention}**.";
                await Reply(Context.Channel,
                    CreateEmbed(Context,
                        $"Set this server's leaving message to ```{message}```\n\n{sendingTest}"));
                if (welcomeChannel is null) return;

                if (config.WelcomeChannel != 0) {
                    var welcomeMessage = config.LeavingMessage
                        .Replace("{ServerName}", Context.Guild.Name)
                        .Replace("{UserMention}", Context.User.Mention)
                        .Replace("{UserName}", Context.User.Username)
                        .Replace("{UserTag}", Context.User.Discriminator);
                    var embed = new EmbedBuilder()
                        .WithColor(config.WelcomeColorR, config.WelcomeColorG, config.WelcomeColorB)
                        .WithDescription(welcomeMessage)
                        .WithThumbnailUrl(Context.User.GetAvatarUrl());
                    await welcomeChannel.SendMessageAsync(string.Empty, false, embed.Build());
                }
            }
        }
    }
}