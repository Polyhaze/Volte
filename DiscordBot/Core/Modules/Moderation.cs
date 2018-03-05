using Discord.Commands;
using Discord;
using SIVA.Core.Config;
using System.Threading.Tasks;
using Discord.WebSocket;
using System.Linq;

namespace SIVA.Core.Modules
{
    public class Moderation : ModuleBase<SocketCommandContext>
    {
        private DiscordSocketClient _client;

        public string Count = "";

        [Command("Ban")]
        [RequireUserPermission(GuildPermission.BanMembers)]
        public async Task BanUser(SocketGuildUser user, [Remainder]string reason = "")
        {
            var config = GuildConfig.GetGuildConfig(Context.Guild.Id);

            await Context.Guild.AddBanAsync(user, reason: reason);
            var embed = new EmbedBuilder();
            embed.WithDescription(Utilities.GetFormattedLocaleMsg("BanText", user.Mention, Context.User.Mention));
            embed.WithFooter(Utilities.GetFormattedLocaleMsg("CommandFooter", Context.User.Username));
            embed.WithColor(new Color(SIVA.Config.bot.DefaultEmbedColour));

            var Case = config.ModlogCase;
            var lCase = Case + 1;
            config.ModlogCase = lCase;
            GuildConfig.SaveGuildConfig();

            var nembed = new EmbedBuilder();
            var channel = _client.GetGuild(Context.Guild.Id).GetTextChannel(config.ChannelId);
            nembed.WithDescription($"Case: {lCase} - Type: Ban\nUser: {user.Mention} ({user.Id})\nModerator: {Context.User.Username}#{Context.User.Discriminator}\nReason: {reason}");
            nembed.WithFooter($"Guild Owner: {Context.Guild.Owner.Username}#{Context.Guild.Owner.Discriminator}");
            nembed.WithColor(SIVA.Config.bot.DefaultEmbedColour);
            await channel.SendMessageAsync("", false, nembed);
            await SendMessage(embed);
        }

        [Command("Softban"), Alias("Sb")]
        [RequireUserPermission(GuildPermission.BanMembers)]
        public async Task BanThenUnbanUser(SocketGuildUser user)
        {
            var embed = new EmbedBuilder();
            await Context.Guild.AddBanAsync(user, pruneDays: 7);
            await Context.Guild.RemoveBanAsync(user);
            embed.WithDescription($"{Context.User.Mention} softbanned <@{user.Id}>, deleting the last 7 days of messages from that user.");
            embed.WithColor(SIVA.Config.bot.DefaultEmbedColour);
            embed.WithFooter(Utilities.GetFormattedLocaleMsg("CommandFooter", Context.User.Username));

            await SendMessage(embed);
        }

        [Command("IdBan")]
        [RequireUserPermission(GuildPermission.BanMembers)]
        public async Task BanUserById(ulong userid)
        {
            var config = GuildConfig.GetGuildConfig(Context.Guild.Id);

            await Context.Guild.AddBanAsync(userid);
            var embed = new EmbedBuilder();
            embed.WithDescription(Utilities.GetFormattedLocaleMsg("BanText", $"<@{userid}>", $"<@{Context.User.Id}>"));
            embed.WithFooter(Utilities.GetFormattedLocaleMsg("CommandFooter", Context.User.Username));
            embed.WithColor(new Color(SIVA.Config.bot.DefaultEmbedColour));

            var Case = config.ModlogCase;
            var lCase = Case + 1;
            config.ModlogCase = lCase;
            GuildConfig.SaveGuildConfig();
            var nembed = new EmbedBuilder();

            var channel = _client.GetGuild(Context.Guild.Id).GetTextChannel(config.ChannelId);
            nembed.WithDescription($"Case: {lCase} - Type: User ID Ban\nUser: <@{userid}> ({userid})\nModerator: {Context.User.Username}#{Context.User.Discriminator}");
            nembed.WithFooter($"Guild Owner: {Context.Guild.Owner.Username}#{Context.Guild.Owner.Discriminator}");
            nembed.WithColor(SIVA.Config.bot.DefaultEmbedColour);
            await channel.SendMessageAsync("", false, nembed);
            await Context.Channel.SendMessageAsync("", false, embed);
        }

        [Command("Rename")]
        [RequireUserPermission(GuildPermission.ManageNicknames)]
        public async Task SetUsersNickname(SocketGuildUser user, [Remainder]string nick)
        {
            await user.ModifyAsync(x => x.Nickname = nick);
            var embed = new EmbedBuilder();
            embed.WithDescription($"Set <@{user.Id}>'s nickname on this server to **{nick}**!");
            embed.WithColor(SIVA.Config.bot.DefaultEmbedColour);
            embed.WithFooter(Utilities.GetFormattedLocaleMsg("CommandFooter", Context.User.Username));

            await SendMessage(embed);
        }

        [Command("Kick")]
        [RequireUserPermission(GuildPermission.KickMembers)]
        public async Task KickUser(SocketGuildUser user, [Remainder]string reason = "")
        {

            var config = GuildConfig.GetGuildConfig(Context.Guild.Id);

            await user.KickAsync();
            var embed = new EmbedBuilder();
            embed.WithDescription(Utilities.GetFormattedLocaleMsg("KickUserMsg", user.Mention, Context.User.Mention));
            embed.WithFooter(Utilities.GetFormattedLocaleMsg("CommandFooter", Context.User.Username));
            embed.WithColor(new Color(SIVA.Config.bot.DefaultEmbedColour));
            await SendMessage(embed);

            var Case = config.ModlogCase;
            var lCase = Case + 1;
            config.ModlogCase = lCase;
            GuildConfig.SaveGuildConfig();

            var channel = _client.GetGuild(Context.Guild.Id).GetTextChannel(config.ChannelId);
            embed.WithDescription($"Case: {lCase} - Type: Kick\nUser: <@{user.Id}> ({user.Id})\nModerator: {Context.User.Username}#{Context.User.Discriminator}\nReason: {reason}");
            embed.WithFooter($"Guild Owner: {Context.Guild.Owner.Username}#{Context.Guild.Owner.Discriminator}");
            embed.WithColor(SIVA.Config.bot.DefaultEmbedColour);
            await channel.SendMessageAsync("", false, embed);

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
            var targetRole = user.Guild.Roles.FirstOrDefault(r => r.Name == role);

            var embed = new EmbedBuilder();
            embed.WithFooter(Utilities.GetFormattedLocaleMsg("CommandFooter", Context.User.Username));
            embed.WithColor(SIVA.Config.bot.DefaultEmbedColour);
            embed.WithDescription(Utilities.GetFormattedLocaleMsg("AddRoleCommandText", role, user.Username + "#" + user.Discriminator));

            await user.AddRoleAsync(targetRole);
            await SendMessage(embed);
        }

        [Command("RemRole"), Alias("RR")]
        [RequireUserPermission(GuildPermission.ManageRoles)]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task TakeAwaySpecifiedRole(SocketGuildUser user, [Remainder]string role)
        {
            var targetRole = user.Guild.Roles.FirstOrDefault(r => r.Name == role);

            var embed = new EmbedBuilder();
            embed.WithFooter(Utilities.GetFormattedLocaleMsg("CommandFooter", Context.User.Username));
            embed.WithColor(SIVA.Config.bot.DefaultEmbedColour);
            embed.WithDescription(Utilities.GetFormattedLocaleMsg("RemRoleCommandText", role, user.Username + "#" + user.Discriminator));

            await user.RemoveRoleAsync(targetRole);
            await SendMessage(embed);
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
        [RequireUserPermission(GuildPermission.KickMembers)]
        public async Task WarnUser(SocketGuildUser user, [Remainder]string reason)
        {
            var embed = new EmbedBuilder();
            embed.WithDescription(Utilities.GetFormattedLocaleMsg("WarnCommandText", user.Mention, reason));
            embed.WithFooter(Utilities.GetFormattedLocaleMsg("CommandFooter", Context.User.Username));
            embed.WithColor(SIVA.Config.bot.DefaultEmbedColour);
            var ua = UserAccounts.UserAccounts.GetAccount(user);
            ua.Warns.Add(reason);
            ua.WarnCount = (uint)ua.Warns.Count;
            UserAccounts.UserAccounts.SaveAccounts();

            await SendMessage(embed);

        }

        [Command("ClearWarns"), Alias("CW")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task ClearUsersWarns(SocketGuildUser user)
        {
            var ua = UserAccounts.UserAccounts.GetAccount(user);
            var embed = new EmbedBuilder();
            embed.WithDescription($"{ua.WarnCount} warn(s) cleared for {user.Mention}");
            embed.WithFooter(Utilities.GetFormattedLocaleMsg("CommandFooter", Context.User.Username));
            embed.WithColor(SIVA.Config.bot.DefaultEmbedColour);
            ua.WarnCount = 0;
            ua.Warns.Clear();
            UserAccounts.UserAccounts.SaveAccounts();

            await SendMessage(embed);
        }

        [Command("Warns"), Priority(0)]
        public async Task WarnsAmountForGivenUser()
        {
            var embed = new EmbedBuilder();
            var ua = UserAccounts.UserAccounts.GetAccount(Context.User);
            Count = ua.WarnCount == 1 ? "WarnsSingulText" : "WarnsPluralText";
            embed.WithFooter(Utilities.GetFormattedLocaleMsg("CommandFooter", Context.User.Username));
            embed.WithColor(SIVA.Config.bot.DefaultEmbedColour);
            embed.WithDescription(Utilities.GetFormattedLocaleMsg(Count, Context.User.Mention, ua.WarnCount.ToString()));

            await SendMessage(embed);
        }

        [Command("Warns"), Priority(1)]
        public async Task WarnsAmountForGivenUser(SocketGuildUser user)
        {
            var embed = new EmbedBuilder();
            var ua = UserAccounts.UserAccounts.GetAccount(user);
            if (ua.WarnCount == 1)
            {
                Count = "WarnsSingulText";
            }
            else
            {
                Count = "WarnsPluralText";
            }
            embed.WithFooter(Utilities.GetFormattedLocaleMsg("CommandFooter", Context.User.Username));
            embed.WithColor(SIVA.Config.bot.DefaultEmbedColour);
            embed.WithDescription(Utilities.GetFormattedLocaleMsg(Count, user.Mention, ua.WarnCount.ToString()));

            await SendMessage(embed);
        }

        public async Task SendMessage(Embed embed, string text = "", bool isTts = false)
        {
            await Context.Channel.SendMessageAsync(text, isTts, embed);
        }
    }
}
