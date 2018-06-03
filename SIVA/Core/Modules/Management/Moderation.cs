using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using SIVA.Core.Bot;
using SIVA.Core.JsonFiles;

namespace SIVA.Core.Modules.Management
{
    public class Moderation : SivaModule
    {
        public string Count = "";

        [Command("Ban")]
        [RequireUserPermission(GuildPermission.BanMembers)]
        public async Task BanUser(SocketGuildUser user, [Remainder] string reason = "Banned by a moderator.")
        {
            var config = GuildConfig.GetOrCreateConfig(Context.Guild.Id);
            if (Helpers.UserHasRole(Context, config.ModRole))
            {
                await Context.Guild.AddBanAsync(user, 7, reason);
                var embed = new EmbedBuilder();
                embed.WithDescription(
                    Bot.Internal.Utilities.GetFormattedLocaleMsg("BanText", user.Mention, Context.User.Mention));
                embed.WithFooter(Bot.Internal.Utilities.GetFormattedLocaleMsg("CommandFooter", Context.User.Username));
                embed.WithColor(new Color(config.EmbedColour1, config.EmbedColour2, config.EmbedColour3));
                await ReplyAsync("", false, embed);
            }
            else
            {
                var e = Helpers.CreateEmbed(Context,
                    Bot.Internal.Utilities.GetFormattedLocaleMsg("NotEnoughPermission", Context.User.Username));
                await Helpers.SendMessage(Context, e);
            }
        }

        [Command("Softban")]
        [Alias("Sb")]
        [RequireUserPermission(GuildPermission.BanMembers)]
        public async Task BanThenUnbanUser(SocketGuildUser user)
        {
            var config = GuildConfig.GetOrCreateConfig(Context.Guild.Id);
            if (Helpers.UserHasRole(Context, config.ModRole))
            {
                var embed = Helpers.CreateEmbed(Context,
                    $"{Context.User.Mention} softbanned <@{user.Id}>, deleting the last 7 days of messages from that user.");
                await Helpers.SendMessage(Context, embed);
                await Context.Guild.AddBanAsync(user, 7);
                await Context.Guild.RemoveBanAsync(user);
            }
            else
            {
                var e = Helpers.CreateEmbed(Context,
                    Bot.Internal.Utilities.GetFormattedLocaleMsg("NotEnoughPermission", Context.User.Username));
                await Helpers.SendMessage(Context, e);
            }
        }

        [Command("IdBan")]
        [RequireUserPermission(GuildPermission.BanMembers)]
        public async Task BanUserById(ulong userid, [Remainder] string reason = "")
        {
            var config = GuildConfig.GetOrCreateConfig(Context.Guild.Id);
            if (Helpers.UserHasRole(Context, config.ModRole))
            {
                if (reason == "") reason = $"Banned by {Context.User.Username}#{Context.User.Discriminator}";
                await Context.Guild.AddBanAsync(userid, 7, reason);
                var embed = Helpers.CreateEmbed(Context,
                    Bot.Internal.Utilities.GetFormattedLocaleMsg("BanText", $"<@{userid}>", $"<@{Context.User.Id}>"));

                await Helpers.SendMessage(Context, embed);
            }
            else
            {
                var e = Helpers.CreateEmbed(Context,
                    Bot.Internal.Utilities.GetFormattedLocaleMsg("NotEnoughPermission", Context.User.Username));
                await Helpers.SendMessage(Context, e);
            }
        }


        [Command("Kick")]
        [RequireUserPermission(GuildPermission.KickMembers)]
        public async Task KickUser(SocketGuildUser user, [Remainder] string reason = "")
        {
            var config = GuildConfig.GetOrCreateConfig(Context.Guild.Id);
            if (Helpers.UserHasRole(Context, config.ModRole))
            {
                await user.KickAsync(reason);
                var embed = Helpers.CreateEmbed(Context,
                    Bot.Internal.Utilities.GetFormattedLocaleMsg("KickUserMsg", user.Mention, Context.User.Mention));

                await Helpers.SendMessage(Context, embed);
            }
            else
            {
                var e = Helpers.CreateEmbed(Context,
                    Bot.Internal.Utilities.GetFormattedLocaleMsg("NotEnoughPermission", Context.User.Username));
                await Helpers.SendMessage(Context, e);
            }
        }

        [Command("Purge")]
        [RequireUserPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task PurgeMessages(int amount)
        {
            var config = GuildConfig.GetOrCreateConfig(Context.Guild.Id);
            if (Helpers.UserHasRole(Context, config.ModRole))
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
                    await Task.Delay(5000);
                    await msg.DeleteAsync();
                }
            }
            else
            {
                var e = Helpers.CreateEmbed(Context,
                    Bot.Internal.Utilities.GetFormattedLocaleMsg("NotEnoughPermission", Context.User.Username));
                await Helpers.SendMessage(Context, e);
            }
        }
    }
}