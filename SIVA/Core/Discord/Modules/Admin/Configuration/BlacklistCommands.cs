using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using SIVA.Core.Files.Readers;
using SIVA.Helpers;

namespace SIVA.Core.Discord.Modules.Admin.Configuration
{
    public class BlacklistCommands : SIVACommand
    {
        [Command("BlacklistAdd"), Alias("BlAdd")]
        public async Task BlacklistAdd([Remainder]string arg)
        {
            if (!UserUtils.IsAdmin(Context))
            {
                await Context.Message.AddReactionAsync(new Emoji("❌"));
                return;
            }

            var config = ServerConfig.Get(Context.Guild);
            
            config.Blacklist.Add(arg);
            await Context.Channel.SendMessageAsync("", false,
                Utils.CreateEmbed(Context, $"Added **{arg}** to the blacklist."));
        }

        [Command("BlacklistRemove"), Alias("BlRem")]
        public async Task BlacklistRemove([Remainder]string arg)
        {
            if (!UserUtils.IsAdmin(Context))
            {
                await Context.Message.AddReactionAsync(new Emoji("❌"));
                return;
            }

            var config = ServerConfig.Get(Context.Guild);
            if (config.Blacklist.Contains(arg))
            {
                config.Blacklist.Remove(arg);
                await Context.Channel.SendMessageAsync("", false,
                    Utils.CreateEmbed(Context, $"Removed **{arg}** from the word blacklist."));
            }
            else
            {
                await Context.Channel.SendMessageAsync("", false,
                    Utils.CreateEmbed(Context, $"**{arg}** doesn't exist in the blacklist."));
            }
        }

        [Command("BlacklistClear"), Alias("BlCl")]
        public async Task BlacklistClear()
        {
            if (!UserUtils.IsAdmin(Context))
            {
                await Context.Message.AddReactionAsync(new Emoji("❌"));
                return;
            }

            var config = ServerConfig.Get(Context.Guild);
            config.Blacklist.Clear();
            await Context.Channel.SendMessageAsync("", false,
                Utils.CreateEmbed(Context, "Cleared the word blacklist."));
        }
    }
}