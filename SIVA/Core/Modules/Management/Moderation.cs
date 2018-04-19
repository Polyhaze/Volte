using Discord.Commands;
using Discord;
using System.Threading.Tasks;
using Discord.WebSocket;
using System.Linq;
using SIVA.Core.JsonFiles;
using System.Collections.Generic;
using System.Threading;
using SIVA.Core.Bot;

namespace SIVA.Core.Modules.Management
{
    public class Moderation : ModuleBase<SocketCommandContext>
    {
        public string Count = "";

        [Command("Ban")]
        [RequireUserPermission(GuildPermission.BanMembers)]
        public async Task BanUser(SocketGuildUser user, [Remainder]string reason = "")
        {
            var config = GuildConfig.GetOrCreateConfig(Context.Guild.Id);
            await Context.Guild.AddBanAsync(user, 7, reason);
            var embed = new EmbedBuilder();
            embed.WithDescription(Bot.Utilities.GetFormattedLocaleMsg("BanText", user.Mention, Context.User.Mention));
            embed.WithFooter(Bot.Utilities.GetFormattedLocaleMsg("CommandFooter", Context.User.Username));
            embed.WithColor(new Color(config.EmbedColour1, config.EmbedColour2, config.EmbedColour3));
            await ReplyAsync("", false, embed);

        }

        [Command("Softban"), Alias("Sb")]
        [RequireUserPermission(GuildPermission.BanMembers)]
        public async Task BanThenUnbanUser(SocketGuildUser user)
        {
            var embed = Helpers.CreateEmbed(Context, $"{Context.User.Mention} softbanned <@{user.Id}>, deleting the last 7 days of messages from that user.");
            await Helpers.SendMessage(Context, embed);
            await Context.Guild.AddBanAsync(user, 7);
            await Context.Guild.RemoveBanAsync(user);

        }

        [Command("IdBan")]
        [RequireUserPermission(GuildPermission.BanMembers)]
        public async Task BanUserById(ulong userid, [Remainder]string reason = "")
        {
            if (reason == "")
            {
                reason = $"Banned by {Context.User.Username}#{Context.User.Discriminator}";
            }
            await Context.Guild.AddBanAsync(userid, 7, reason);
            var embed = Helpers.CreateEmbed(Context, Bot.Utilities.GetFormattedLocaleMsg("BanText", $"<@{userid}>", $"<@{Context.User.Id}>"));

            await Helpers.SendMessage(Context, embed);
        }

        [Command("Rename")]
        [RequireUserPermission(GuildPermission.ManageNicknames)]
        public async Task SetUsersNickname(SocketGuildUser user, [Remainder]string nick)
        {
            var config = GuildConfig.GetOrCreateConfig(Context.Guild.Id);
            await user.ModifyAsync(x => x.Nickname = nick);
            var embed = Helpers.CreateEmbed(Context, $"Set <@{user.Id}>'s nickname on this server to **{nick}**!");

            await Helpers.SendMessage(Context, embed);
        }

        [Command("Kick")]
        [RequireUserPermission(GuildPermission.KickMembers)]
        public async Task KickUser(SocketGuildUser user, [Remainder]string reason = "")
        {
            await user.KickAsync(reason);
            var embed = Helpers.CreateEmbed(Context, Bot.Utilities.GetFormattedLocaleMsg("KickUserMsg", user.Mention, Context.User.Mention));

            await Helpers.SendMessage(Context, embed);
        }


        [Command("AddRole"), Alias("Ar")]
        [RequireUserPermission(GuildPermission.ManageRoles)]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task GiveUserSpecifiedRole(SocketGuildUser user, [Remainder]string role)
        {
            var targetRole = user.Guild.Roles.FirstOrDefault(r => r.Name == role);

            var embed = Helpers.CreateEmbed(Context, Bot.Utilities.GetFormattedLocaleMsg("AddRoleCommandText", role, user.Username + "#" + user.Discriminator));

            await user.AddRoleAsync(targetRole);
            await Helpers.SendMessage(Context, embed);
        }

        [Command("RemRole"), Alias("Rr")]
        [RequireUserPermission(GuildPermission.ManageRoles)]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task TakeAwaySpecifiedRole(SocketGuildUser user, [Remainder]string role)
        {
            var targetRole = user.Guild.Roles.FirstOrDefault(r => r.Name == role);

            var embed = Helpers.CreateEmbed(Context, Bot.Utilities.GetFormattedLocaleMsg("RemRoleCommandText", role, user.Username + "#" + user.Discriminator));

            await user.RemoveRoleAsync(targetRole);
            await Helpers.SendMessage(Context, embed);
        }

        [Command("Purge")]
        [RequireUserPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task PurgeMessages(int amount)
        {
            if (amount < 1)
            {
                await ReplyAsync("You cannot delete less than 1 message.");
            }
            else
            {
                var messages = await Context.Channel.GetMessagesAsync(amount).Flatten();
                await Context.Channel.DeleteMessagesAsync(messages);
                var msg = await ReplyAsync($"Deleted {amount} messages.");
                Thread.Sleep(5000);
                await msg.DeleteAsync();
            }
        }

        /*[Command("Warn")]
        [RequireUserPermission(GuildPermission.KickMembers)]
        public async Task WarnUser(SocketGuildUser user, [Remainder]string reason)
        {
            var config = GuildConfig.GetOrCreateConfig(Context.Guild.Id);
            var embed = new EmbedBuilder();

            embed.WithDescription(Bot.Utilities.GetFormattedLocaleMsg("WarnCommandText", user.Mention, reason));
            embed.WithFooter(Bot.Utilities.GetFormattedLocaleMsg("CommandFooter", Context.User.Username));
            embed.WithColor(new Color(config.EmbedColour1, config.EmbedColour2, config.EmbedColour3));
            var ua = UserAccounts.GetAccount(user);
            ua.Warns.Add(Context.Guild.Id, reason);
            ua.WarnCount = (uint)ua.Warns.Count;
            UserAccounts.SaveAccounts();
            await ReplyAsync("", false, embed);

        }

        [Command("ClearWarns"), Alias("Cw")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task ClearUsersWarns(SocketGuildUser user)
        {
            var config = GuildConfig.GetOrCreateConfig(Context.Guild.Id);
            var ua = UserAccounts.GetAccount(user);
            var embed = new EmbedBuilder();
            embed.WithDescription($"Cleared the warn(s) for {user.Mention}");
            embed.WithFooter(Bot.Utilities.GetFormattedLocaleMsg("CommandFooter", Context.User.Username));
            embed.WithColor(new Color(config.EmbedColour1, config.EmbedColour2, config.EmbedColour3));
            ua.WarnCount = (uint)ua.Warns.Count();
            foreach (KeyValuePair<ulong, string> warns in ua.Warns)
            {
                if (warns.Key == config.ServerId)
                {
                    ua.Warns.Remove(warns.Key);
                }
            }
            UserAccounts.SaveAccounts();
            
            await ReplyAsync("", false, embed);

        }

        [Command("Warns"), Priority(0)]
        public async Task WarnsAmountForGivenUser()
        {
            var config = GuildConfig.GetOrCreateConfig(Context.Guild.Id);
            var embed = new EmbedBuilder();
            var ua = UserAccounts.GetAccount(Context.User);
            Count = ua.WarnCount == 1 ? "WarnsSingulText" : "WarnsPluralText";
            embed.WithFooter(Bot.Utilities.GetFormattedLocaleMsg("CommandFooter", Context.User.Username));
            embed.WithColor(new Color(config.EmbedColour1, config.EmbedColour2, config.EmbedColour3));
            embed.WithDescription($"{Context.User.Mention} has {ua.WarnCount} warns and their most recent warn is `{ua.Warns.Last().Value}`");
            await ReplyAsync("", false, embed);

        }

        [Command("Warns"), Priority(1)]
        public async Task WarnsAmountForGivenUser(SocketGuildUser user)
        {
            var config = GuildConfig.GetOrCreateConfig(Context.Guild.Id);
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
            embed.WithFooter(Bot.Utilities.GetFormattedLocaleMsg("CommandFooter", Context.User.Username));
            embed.WithColor(new Color(config.EmbedColour1, config.EmbedColour2, config.EmbedColour3));
            embed.WithDescription($"{user.Mention} has {ua.WarnCount} warns and their most recent warn is `{ua.Warns.Last().Value}`");

            await ReplyAsync("", false, embed);

        }*/
    }
}
