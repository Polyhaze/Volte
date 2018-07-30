using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using SIVA.Core.Files.Readers;

namespace SIVA.Core.Discord.Modules.Information.MiscInfo {
    public class UserInfoCommand : SIVACommand {
        [Command("UserInfo"), Alias("UI", "UserI", "UInfo")]
        public async Task UserInfo(SocketGuildUser user = null) {
            var config = ServerConfig.Get(Context.Guild);
            var actualUser = user ?? (SocketGuildUser)Context.User;
            var embed = new EmbedBuilder()
                .WithAuthor(Context.User)
                .WithColor(config.EmbedColourR, config.EmbedColourG, config.EmbedColourB)
                .WithThumbnailUrl(actualUser.GetAvatarUrl())
                .WithTitle("User Info")

                .AddField("User ID", actualUser.Id)
                .AddField("Game", actualUser.Activity.Name ?? "Nothing")
                .AddField("Created At",
                    $"Month: {actualUser.CreatedAt.Month}\nDay: {actualUser.CreatedAt.Day}\nYear: {actualUser.CreatedAt.Year}")
                .AddField("Status", actualUser.Status)
                .AddField("Is Bot", actualUser.IsBot);

            await Context.Channel.SendMessageAsync("", false, embed.Build());


        }
    }
}