using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using SIVA.Core.Files.Readers;
using SIVA.Helpers;

namespace SIVA.Core.Discord.Modules.Owner {
    public class SetStatusCommand : SivaCommand {
        [Command("SetStatus")]
        public async Task SetStatus([Remainder] string status) {
            if (!UserUtils.IsBotOwner(Context.User)) {
                await Context.Message.AddReactionAsync(new Emoji(RawEmoji.X));
                return;
            }

            var embed = new EmbedBuilder();
            var config = ServerConfig.Get(Context.Guild);
            embed.WithAuthor(Context.User);
            embed.WithColor(config.EmbedColourR, config.EmbedColourG, config.EmbedColourB);
            switch (status.ToLower()) {
                case "dnd":
                    await Siva.GetInstance().SetStatusAsync(UserStatus.DoNotDisturb);
                    embed.WithDescription("Set the status to Do Not Disturb.");
                    break;
                case "idle":
                    await Siva.GetInstance().SetStatusAsync(UserStatus.Idle);
                    embed.WithDescription("Set the status to Idle.");
                    break;
                case "invisible":
                    await Siva.GetInstance().SetStatusAsync(UserStatus.Invisible);
                    embed.WithDescription("Set the status to Invisible.");
                    break;
                case "online":
                    await Siva.GetInstance().SetStatusAsync(UserStatus.Online);
                    embed.WithDescription("Set the status to Online.");
                    break;
                default:
                    await Siva.GetInstance().SetStatusAsync(UserStatus.Online);
                    embed.WithDescription(
                        "Your option wasn't known, so I set the status to Online.\nAvailable options for this command are `dnd`, `idle`, `invisible`, or `online`.");
                    break;
            }

            await Context.Channel.SendMessageAsync("", false, embed.Build());
        }
    }
}