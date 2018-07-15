using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using SIVA.Core.Files.Readers;
using SIVA.Helpers;

namespace SIVA.Core.Discord.Modules.Admin.Configuration
{
    public class CustomCommandCommands : SIVACommand
    {
        [Command("CustomCommandAdd"), Alias("Cca")]
        public async Task CustomCommandAdd(string name, [Remainder]string response)
        {
            if (!UserUtils.IsAdmin(Context))
            {
                await Context.Message.AddReactionAsync(new Emoji("❌"));
                return;
            }

            var config = ServerConfig.Get(Context.Guild);
            config.CustomCommands.Add(name, response);
            ServerConfig.Save();
            await Context.Channel.SendMessageAsync("", false,
                Utils.CreateEmbed(Context, "")
                    .ToEmbedBuilder()
                    .AddField("Command Name", name)
                    .AddField("Command Response", response)
                    .Build()
                );
        }

        [Command("CustomCommandRem"), Alias("Ccr")]
        public async Task CustomCommandRem(string cmdName)
        {
            var config = ServerConfig.Get(Context.Guild);
            var embed = new EmbedBuilder()
                .WithColor(config.EmbedColourR, config.EmbedColourG, config.EmbedColourB)
                .WithAuthor(Context.User);
            if (!UserUtils.IsAdmin(Context))
            {
                await Context.Message.AddReactionAsync(new Emoji("❌"));
                return;
            }

            if (config.CustomCommands.Keys.Contains(cmdName))
            {
                config.CustomCommands.Remove(cmdName);
                ServerConfig.Save();
                embed.WithDescription($"Removed **{cmdName}** from this server's Custom Commands.");
            }
            else
            {
                embed.WithDescription($"**{cmdName}** is not a command on this server.");
            }

            await Context.Channel.SendMessageAsync("", false, embed.Build());
        }

        [Command("CustomCommandClear"), Alias("Ccc")]
        public async Task CustomCommandClear()
        {
            if (!UserUtils.IsAdmin(Context))
            {
                await Context.Message.AddReactionAsync(new Emoji("❌"));
                return;
            }

            var config = ServerConfig.Get(Context.Guild);
            await Context.Channel.SendMessageAsync("", false,
                Utils.CreateEmbed(Context,
                    $"Cleared the blacklist, containing {config.CustomCommands.Count} commands."));
            config.CustomCommands.Clear();
            ServerConfig.Save();

        }
    }
}