using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Volte.Core.Discord;
using Volte.Core.Files.Readers;
using Volte.Helpers;

namespace Volte.Core.Modules.Admin {
    public partial class AdminModule : VolteModule {
        [Command("WelcomeChannel"), Alias("Wc")]
        public async Task WelcomeChannel(SocketTextChannel channel) {
            if (!UserUtils.IsAdmin(Context)) {
                await React(Context.SMessage, RawEmoji.X);
                return;
            }

            var config = Db.GetConfig(Context.Guild);
            config.WelcomeChannel = channel.Id;
            Db.UpdateConfig(config);
            await Context.Channel.SendMessageAsync("", false,
                CreateEmbed(Context, $"Set this server's welcome channel to **{channel.Name}**"));
        }

        [Command("WelcomeMessage"), Alias("Wmsg")]
        public async Task WelcomeMessage([Remainder] string message = "") {
            if (!UserUtils.IsAdmin(Context)) {
                await React(Context.SMessage, RawEmoji.X);
                return;
            }

            var config = Db.GetConfig(Context.Guild);
            
            if (message.Equals("")) {
                await Context.Channel.SendMessageAsync("", false,
                    CreateEmbed(Context, $"The current welcome message for this server is ```\n{config.WelcomeMessage}```"));
            }
            else {
                config.WelcomeMessage = message;
                Db.UpdateConfig(config);
                var welcomeChannel =
                    VolteBot.Client.GetGuild(Context.Guild.Id).GetTextChannel(config.WelcomeChannel);
                var sendingTest = config.WelcomeChannel == 0 ? "Not sending a test message as you do not have a welcome channel set." : $"Sending a test message to **{welcomeChannel.Name}**.";
                await Context.Channel.SendMessageAsync("", false,
                    CreateEmbed(Context,
                        $"Set this server's welcome message to ```{message}```\n\n{sendingTest}"));

                if (config.WelcomeChannel != 0) {
                    var welcomeMessage = config.WelcomeMessage
                        .Replace("{ServerName}", Context.Guild.Name)
                        .Replace("{UserMention}", Context.User.Mention)
                        .Replace("{UserTag}", $"{Context.User.Username}#{Context.User.Discriminator}");
                    var embed = CreateEmbed(Context, welcomeMessage).ToEmbedBuilder()
                        .WithThumbnailUrl(Context.Guild.IconUrl);
                    await welcomeChannel.SendMessageAsync("", false, embed.Build());
                } 
                
            }
        }

        [Command("WelcomeColour"), Alias("WelcomeColor", "Wcl")]
        public async Task WelcomeColour(int r, int g, int b) {
            if (!UserUtils.IsAdmin(Context)) {
                await React(Context.SMessage, RawEmoji.X);
                return;
            }

            if (r > 255 || g > 255 || b > 255) {
                await Reply(Context.Channel,
                    CreateEmbed(Context,
                        "You cannot have an RGB value greater than 255. Either the R, G, or B value you entered exceeded 255 in value."));
                return;
            }

            var config = Db.GetConfig(Context.Guild);
            config.WelcomeColourR = r;
            config.WelcomeColourG = g;
            config.WelcomeColourB = b;
            Db.UpdateConfig(config);
            await Reply(Context.Channel,
                CreateEmbed(Context,
                    $"Successfully set this server's welcome message embed colour to `{r}, {g}, {b}`!"));
        }

        [Command("LeavingMessage"), Alias("Lmsg")]
        public async Task LeavingMessage([Remainder]string message = "") {
            if (!UserUtils.IsAdmin(Context)) {
                await React(Context.SMessage, RawEmoji.X);
                return;
            }

            var config = Db.GetConfig(Context.Guild);
            
            if (message.Equals("")) {
                await Context.Channel.SendMessageAsync("", false,
                    CreateEmbed(Context, $"The current leaving message for this server is ```\n{config.WelcomeMessage}```"));
            }
            else {
                config.LeavingMessage = message;
                Db.UpdateConfig(config);
                var welcomeChannel =
                    VolteBot.Client.GetGuild(Context.Guild.Id).GetTextChannel(config.WelcomeChannel);
                var sendingTest = config.WelcomeChannel == 0 
                    ? "Not sending a test message, as you do not have a welcome channel set. " +
                      "Set a welcome channel to fully complete the setup!" 
                    : $"Sending a test message to **{welcomeChannel.Mention}**.";
                await Reply(Context.Channel,
                    CreateEmbed(Context,
                        $"Set this server's leaving message to ```{message}```\n\n{sendingTest}"));

                if (config.WelcomeChannel != 0) {
                    var welcomeMessage = config.LeavingMessage
                        .Replace("{ServerName}", Context.Guild.Name)
                        .Replace("{UserMention}", Context.User.Mention)
                        .Replace("{UserTag}", $"{Context.User.Username}#{Context.User.Discriminator}");
                    var embed = new EmbedBuilder()
                        .WithColor(config.WelcomeColourR, config.WelcomeColourG, config.WelcomeColourB)
                        .WithDescription(welcomeMessage)
                        .WithThumbnailUrl(Context.Guild.IconUrl);
                    await welcomeChannel.SendMessageAsync("", false, embed.Build());
                } 
                
            }
        }
    }
}