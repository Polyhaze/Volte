using Discord.Commands;
using Discord;
using System.Threading.Tasks;
using Discord.WebSocket;
using SIVA.Core.UserAccounts;
using System.Linq;
using SIVA.Core.Config;

namespace SIVA.Modules
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
            var targetRole = user.Guild.Roles.FirstOrDefault(r => r.Name == role);

            var embed = new EmbedBuilder();
            embed.WithFooter(Utilities.GetFormattedAlert("CommandFooter", Context.User.Username));
            embed.WithColor(Config.bot.defaultEmbedColour);
            embed.WithDescription(Utilities.GetFormattedAlert("AddRoleCommandText", role, user.Username + "#" + user.Discriminator));

            await user.AddRoleAsync(targetRole);
            await Context.Channel.SendMessageAsync("", false, embed);
        }

        [Command("RemRole"), Alias("RR")]
        [RequireUserPermission(GuildPermission.ManageRoles)]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task TakeAwaySpecifiedRole(SocketGuildUser user, [Remainder]string role)
        {
            var targetRole = user.Guild.Roles.FirstOrDefault(r => r.Name == role);

            var embed = new EmbedBuilder();
            embed.WithFooter(Utilities.GetFormattedAlert("CommandFooter", Context.User.Username));
            embed.WithColor(Config.bot.defaultEmbedColour);
            embed.WithDescription(Utilities.GetFormattedAlert("RemRoleCommandText", role, user.Username + "#" + user.Discriminator));

            await user.RemoveRoleAsync(targetRole);
            await Context.Channel.SendMessageAsync("", false, embed);
        }

        [Command("Levels"), Alias("L")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task Leveling(bool arg)
        {
            var config = GuildConfig.GetGuildConfig(Context.Guild.Id) ?? GuildConfig.CreateGuildConfig(Context.Guild.Id);
            var embed = new EmbedBuilder();
            embed.WithFooter(Utilities.GetFormattedAlert("CommandFooter", Context.User.Username));
            embed.WithColor(Config.bot.defaultEmbedColour);
            embed.WithDescription(arg ? "Enabled leveling for this server." : "Disabled leveling for this server.");
            config.Leveling = arg;
            GuildConfig.SaveGuildConfig();

            await Context.Channel.SendMessageAsync("", false, embed);
        }

        [Command("ServerPrefix")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task SetGuildPrefix([Remainder]string prefix)
        {
            var config = GuildConfig.GetGuildConfig(Context.Guild.Id) ?? GuildConfig.CreateGuildConfig(Context.Guild.Id);
            GuildConfig.SaveGuildConfig();
            var embed = new EmbedBuilder();
            embed.WithDescription("Done.");
            embed.WithFooter(Utilities.GetFormattedAlert("CommandFooter", Context.User.Username));
            embed.WithColor(Config.bot.defaultEmbedColour);
            config.CommandPrefix = prefix;
            await Context.Channel.SendMessageAsync("", false, embed);
        }

        [Command("AutoRole")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task AutoRoleRoleAdd([Remainder]string arg = "")
        {

            var config = GuildConfig.GetOrCreateConfig(Context.Guild.Id);
            config.RoleToApply = arg;
            GuildConfig.SaveGuildConfig();

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

        [Command("ClearWarns"), Alias("CW")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task ClearUsersWarns(SocketGuildUser user)
        {
            var ua = UserAccounts.GetAccount(user);
            var embed = new EmbedBuilder();
            embed.WithDescription($"{ua.WarnCount} warn(s) cleared for {user.Mention}");
            embed.WithFooter(Utilities.GetFormattedAlert("CommandFooter", Context.User.Username));
            embed.WithColor(Config.bot.defaultEmbedColour);
            ua.WarnCount = 0;
            ua.Warns.Clear();
            UserAccounts.SaveAccounts();

            await SendMessage("", false, embed);
        }

        [Command("Warns"), Priority(0)]
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

        [Command("Warns"), Priority(1)]
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

        public async Task SendMessage(string text, bool isTts, Embed embed)
        {
            await Context.Channel.SendMessageAsync(text, isTts, embed);
        }
    }
}
