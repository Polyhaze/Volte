using Discord.Commands;
using System.Threading.Tasks;
using Discord.WebSocket;
using SIVA.Core.Config;
using System.Linq;

namespace SIVA.Modules
{
    public class Support : ModuleBase<SocketCommandContext>
    {

        [Command("SupportReactionEmoji"), Alias("SRE")]
        [RequireUserPermission(Discord.GuildPermission.Administrator)]
        public async Task AddReactToJson(string emoji)
        {
            var config = GuildConfig.GetGuildConfig(Context.Guild.Id) ?? GuildConfig.CreateGuildConfig(Context.Guild.Id);
            config.ReactionEmoji = emoji;
            GuildConfig.SaveGuildConfig();
            await Context.Channel.SendMessageAsync($":{emoji}: set as the Support Ticket close emoji for this Guild.");
        }

        [Command("SupportCloseOwnTicket"), Alias("SCOT")]
        [RequireUserPermission(Discord.GuildPermission.Administrator)]
        public async Task AddBooleanToJson(bool arg)
        {
            var config = GuildConfig.GetGuildConfig(Context.Guild.Id) ?? GuildConfig.CreateGuildConfig(Context.Guild.Id);
            config.CanCloseOwnTicket = arg;
            GuildConfig.SaveGuildConfig();
            await Context.Channel.SendMessageAsync($"{arg} set as the Support Ticket `CanCloseOwnTicket` option.");
        }

        [Command("SupportChannelName"), Alias("SCN")]
        [RequireUserPermission(Discord.GuildPermission.Administrator)]
        public async Task AddChannelToConfig(string arg)
        {
            var config = GuildConfig.GetGuildConfig(Context.Guild.Id) ?? GuildConfig.CreateGuildConfig(Context.Guild.Id);
            config.SupportChannelName = arg;
            GuildConfig.SaveGuildConfig();
            await Context.Channel.SendMessageAsync($"{arg} set as the Support channel name.");
        }

        [Command("SupportCategoryId"), Alias("SCI")]
        [RequireUserPermission(Discord.GuildPermission.Administrator)]
        public async Task SetIdIntoConfig(ulong id)
        {
            var config = GuildConfig.GetGuildConfig(Context.Guild.Id) ?? GuildConfig.CreateGuildConfig(Context.Guild.Id);
            config.SupportCategoryId = id;
            GuildConfig.SaveGuildConfig();
            await Context.Channel.SendMessageAsync($"{id.ToString()} set as the support channel category ID.");
        }

        [Command("SupportRole"), Alias("SR")]
        [RequireUserPermission(Discord.GuildPermission.Administrator)]
        public async Task SetRoleInConfig(string role)
        {
            var config = GuildConfig.GetGuildConfig(Context.Guild.Id) ?? GuildConfig.CreateGuildConfig(Context.Guild.Id);
            config.SupportRole = role;
            GuildConfig.SaveGuildConfig();
            await Context.Channel.SendMessageAsync($"`{role}` set as the role to manage tickets.");
        }

        [Command("SupportCloseTicket"), Alias("SCT", "Close"), Priority(0)] 
        [RequireUserPermission(Discord.GuildPermission.ManageChannels)]
        public async Task CloseTicket()
        {
            var config = GuildConfig.GetGuildConfig(Context.Guild.Id);
            if (config == null) return;
            var supportChannel = Context.Guild.Channels.FirstOrDefault(c => c.Name == $"{config.SupportChannelName}-{Context.User.Id}");

            if (config.CanCloseOwnTicket == false)
            {
                await Context.Channel.SendMessageAsync("This server doesn't allow you to close your own ticket!");
            }
            else
            {
                if (supportChannel == null)
                {
                    await Context.Channel.SendMessageAsync("You don't have a support channel made.");
                }
                else
                {
                    await supportChannel.DeleteAsync();
                    await Context.Channel.SendMessageAsync($"Your ticket - \"{supportChannel.Name}\" - has been deleted.");
                }
            }
        }

        [Command("SupportCloseTicket"), Alias("SCT", "Close"), Priority(1), RequireUserPermission(Discord.GuildPermission.ManageChannels)]
        public async Task CloseTicket(SocketGuildUser user)
        {
            var config = GuildConfig.GetGuildConfig(Context.Guild.Id);
            if (config == null) return;
            var supportChannel = Context.Guild.Channels.FirstOrDefault(c => c.Name == $"{config.SupportChannelName}-{user.Id}");

            if (config.CanCloseOwnTicket == false)
            {
                await Context.Channel.SendMessageAsync("This server doesn't allow you to close your own ticket!");
            }
            else
            {
                if (supportChannel == null)
                {
                    await Context.Channel.SendMessageAsync("You don't have a support channel made.");
                }
                else
                {
                    await supportChannel.DeleteAsync();
                    await Context.Channel.SendMessageAsync($"Your ticket - \"{supportChannel.Name}\" - has been deleted.");
                }
            }
        }
    }
}
