using Discord.Commands;
using Discord;
using System.Threading.Tasks;
using Discord.WebSocket;
using DiscordBot.Core.UserAccounts;
using System.Linq;

namespace DiscordBot.Modules
{
    public class Moderation : ModuleBase<SocketCommandContext>
    {
        public string Count = "";

        [Command("Ban")]
        [RequireUserPermission(GuildPermission.BanMembers)]
        public async Task BanUser(SocketGuildUser user)
        {
            await Context.Guild.AddBanAsync(user);
            var embed = new EmbedBuilder();
            embed.WithDescription(Utilities.GetFormattedAlert("BanText", user.Mention, Context.User.Mention));
            embed.WithFooter(Utilities.GetFormattedAlert("CommandFooter", Context.User.Username));
            embed.WithColor(new Color(Config.bot.defaultEmbedColour));
            await Context.Channel.SendMessageAsync("", false, embed);
        }

        [Command("IdBan")]
        [RequireUserPermission(GuildPermission.BanMembers)]
        public async Task BanUserById(ulong userid)
        {
            await Context.Guild.AddBanAsync(userid);
            var embed = new EmbedBuilder();
            embed.WithDescription(Utilities.GetFormattedAlert("BanText", $"<@{userid}>", $"<@{Context.User.Id}"));
            embed.WithFooter(Utilities.GetFormattedAlert("CommandFooter", Context.User.Username));
            embed.WithColor(new Color(Config.bot.defaultEmbedColour));
            await Context.Channel.SendMessageAsync("", false, embed);
        }

        [Command("Kick")]
        [RequireUserPermission(GuildPermission.KickMembers)]
        public async Task KickUser(SocketGuildUser user)
        {
            await user.KickAsync();
            var embed = new EmbedBuilder();
            embed.WithDescription(Utilities.GetFormattedAlert("KickUserMsg", user.Mention, Context.User.Mention));
            embed.WithFooter(Utilities.GetFormattedAlert("CommandFooter", Context.User.Username));
            embed.WithColor(new Color(Config.bot.defaultEmbedColour));

            await Context.Channel.SendMessageAsync("", false, embed);

        }

        /*[Command("ModLog")]
        public async Task SetModlogChannel()
        {
            var config = Modlogs.GetModlogConfig(Context.Guild.Id);
            if (config == null)
            {
                var newConfig = Modlogs.CreateModlogConfig(Context.Guild.Id, Context.Channel.Id);
                await Context.Channel.SendMessageAsync("Modlog config created and channel set to current channel!");
            }
            else
            {
                config.channelId = Context.Channel.Id;
                Modlogs.SaveModlogConfig();
                await Context.Channel.SendMessageAsync("Modlog channel set to current channel!");
            }
        }*/

        [Command("AddRole"), Alias("AR")]
        [RequireUserPermission(GuildPermission.ManageRoles)]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task GiveUserSpecifiedRole(SocketGuildUser user, [Remainder]string role)
        {
            var targetRole = user.Guild.Roles.Where(r => r.Name == role).FirstOrDefault();

            var embed = new EmbedBuilder();
            embed.WithFooter(Utilities.GetFormattedAlert("CommandFooter", Context.User.Username));
            embed.WithColor(Config.bot.defaultEmbedColour);
            embed.WithDescription(Utilities.GetFormattedAlert("AddRoleCommandText", role, user.Username + "#" + user.Discriminator.ToString()));

            await user.AddRoleAsync(targetRole);
            await Context.Channel.SendMessageAsync("", false, embed);
        }

        [Command("RemRole"), Alias("RR")]
        [RequireUserPermission(GuildPermission.ManageRoles)]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task TakeAwaySpecifiedRole(SocketGuildUser user, [Remainder]string role)
        {
            var targetRole = user.Guild.Roles.Where(r => r.Name == role).FirstOrDefault();

            var embed = new EmbedBuilder();
            embed.WithFooter(Utilities.GetFormattedAlert("CommandFooter", Context.User.Username));
            embed.WithColor(Config.bot.defaultEmbedColour);
            embed.WithDescription(Utilities.GetFormattedAlert("RemRoleCommandText", role, user.Username + "#" + user.Discriminator.ToString()));

            await user.RemoveRoleAsync(targetRole);
            await Context.Channel.SendMessageAsync("", false, embed);
        }

        [Command("AutoRole")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task AutoRoleRoleAdd([Remainder]string arg = "")
        {
            SocketUser target = null;
            var mentionedUser = Context.Message.MentionedUsers.FirstOrDefault();
            target = mentionedUser ?? Context.User;

            var config = AutoRoles.GetOrCreateAutoroleConfig(Context.Guild.Id, arg);
            config.roleToApply = arg;
            AutoRoles.SaveAutoroleConfig();

            var embed = new EmbedBuilder();
            embed.WithDescription(Utilities.GetFormattedAlert("AutoroleCommandText", arg));
            embed.WithThumbnailUrl(Context.Guild.IconUrl);
            embed.WithColor(Config.bot.defaultEmbedColour);

            await Context.Channel.SendMessageAsync("", false, embed);


        }

        [Command("Purge")]
        [RequireUserPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task PurgeMessages(int amount)
        {
            amount++;
            if (amount < 1)
            {
                await Context.Channel.SendMessageAsync("You cannot delete 0 messages, ya dingus!");
            }
            else
            {
                var messages = await Context.Channel.GetMessagesAsync(amount).Flatten();
                await Context.Channel.DeleteMessagesAsync(messages);
            }
        }

        [Command("Warn")]
        public async Task WarnUser(SocketGuildUser user, [Remainder]string reason)
        {
            var embed = new EmbedBuilder();
            embed.WithDescription(Utilities.GetFormattedAlert("WarnCommandText", user.Mention, reason));
            embed.WithFooter(Utilities.GetFormattedAlert("CommandFooter", Context.User.Username));
            embed.WithColor(Config.bot.defaultEmbedColour);
            var ua = UserAccounts.GetAccount(user);
            ua.Warns.Add(reason);
            ua.WarnCount = (uint)ua.Warns.Count;
            UserAccounts.SaveAccounts();

            await SendMessage("", false, embed);

        }

        [Command("Warns")]
        public async Task WarnsAmountForGivenUser()
        {
            var embed = new EmbedBuilder();
            var ua = UserAccounts.GetAccount(Context.User);
            if (ua.WarnCount == 1)
            {
                Count = "WarnsSingulText";
            }
            else
            {
                Count = "WarnsPluralText";
            }
            embed.WithFooter(Utilities.GetFormattedAlert("CommandFooter", Context.User.Username));
            embed.WithColor(Config.bot.defaultEmbedColour);
            embed.WithDescription(Utilities.GetFormattedAlert(Count, Context.User.Mention, ua.WarnCount.ToString()));

            await SendMessage("", false, embed);
        }

        [Command("Warns")]
        public async Task WarnsAmountForGivenUser(SocketGuildUser user)
        {
            var embed = new EmbedBuilder();
            var ua = UserAccounts.GetAccount(user);
            if (ua.WarnCount == 1)
            {
                Count = "WarnsSingulText";
            }
            else
            {
                Count = "WarnsPluralText";
            }
            embed.WithFooter(Utilities.GetFormattedAlert("CommandFooter", Context.User.Username));
            embed.WithColor(Config.bot.defaultEmbedColour);
            embed.WithDescription(Utilities.GetFormattedAlert(Count, user.Mention, ua.WarnCount.ToString()));

            await SendMessage("", false, embed);
        }

        public async Task SendMessage(string text, bool IsTTS, Embed embed)
        {
            await Context.Channel.SendMessageAsync(text, IsTTS, embed);
        }

        
        //[Command("Mute")]

        [Command("Shutdown")]
        public async Task Shutdown()
        {
            if (Context.User.Id == Config.bot.botOwner)
            { 
                var embed = new EmbedBuilder();
                embed.WithDescription(Utilities.GetFormattedAlert("LoggingOutMsg", Context.User.Mention));
                embed.WithColor(Config.bot.defaultEmbedColour);
                embed.WithFooter(Utilities.GetFormattedAlert("CommandFooter", Context.User.Username));
                await Context.Channel.SendMessageAsync("", false, embed);
                await Context.Client.LogoutAsync();
                await Context.Client.StopAsync();

            }
            else
            {
                var embed = new EmbedBuilder();
                embed.WithDescription(Utilities.GetFormattedAlert("NotEnoughPermission", Context.User.Mention));
                embed.WithColor(new Color(Config.bot.defaultEmbedColour));
                embed.WithFooter(Utilities.GetFormattedAlert("CommandFooter", Context.User.Username));
                await Context.Channel.SendMessageAsync("", false, embed);
            }
        }
        
    }
}
