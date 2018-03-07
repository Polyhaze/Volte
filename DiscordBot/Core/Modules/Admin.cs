using Discord.Commands;
using System.Threading.Tasks;
using Discord.WebSocket;
using SIVA.Core.Config;
using System.Linq;
using Discord;

namespace SIVA.Core.Modules
{
    public class Admin : ModuleBase<SocketCommandContext>
    {
        [Command("ServerName")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task ModifyServerName([Remainder]string name)
        {
            await Context.Guild.ModifyAsync(x => x.Name = name);
            var embed = new EmbedBuilder();
            embed.WithDescription($"Set this server's name to **{name}**!");
            embed.WithColor(SIVA.Config.bot.DefaultEmbedColour);
            embed.WithFooter(Utilities.GetFormattedLocaleMsg("CommandFooter", Context.User.Username));

            await SendMessage(embed);
        }

        [Command("PengChecks"), Alias("Pc")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task SetBoolToJson(bool arg)
        {
            var config = GuildConfig.GetGuildConfig(Context.Guild.Id) ??
                         GuildConfig.CreateGuildConfig(Context.Guild.Id);
            var embed = new EmbedBuilder();
            embed.WithColor(SIVA.Config.bot.DefaultEmbedColour);
            embed.WithFooter(Utilities.GetFormattedLocaleMsg("CommandFooter", Context.User.Username));
            if (arg == true)
            {
                embed.WithDescription("Enabled mass peng checks for this server.");
            }
            else
            {
                embed.WithDescription("Disabled mass peng checks for this server.");
            }

            config.MassPengChecks = arg;
            GuildConfig.SaveGuildConfig();
            await SendMessage(embed);
        }

        [Command("Antilink")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task SetBoolIntoConfig(bool setting)
        {
            var config = GuildConfig.GetGuildConfig(Context.Guild.Id) ?? GuildConfig.CreateGuildConfig(Context.Guild.Id);
            if (setting == true || setting == false) config.Antilink = setting;
            GuildConfig.SaveGuildConfig();
            var embed = new EmbedBuilder();
            embed.WithFooter(Utilities.GetFormattedLocaleMsg("CommandFooter", Context.User.Username));
            embed.WithColor(SIVA.Config.bot.DefaultEmbedColour);
            if (setting == true || setting == false)
            {
                if (setting == true) embed.WithDescription("Enabled Antilink for this server.");
                if (setting == false) embed.WithDescription("Disabled Antilink for this server.");
            }
            await ReplyAsync("", false, embed);
            
        }

        [Command("AddXp")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task AddXP(uint xp)
        {
            var account = UserAccounts.UserAccounts.GetAccount(Context.User);
            account.XP += xp;
            UserAccounts.UserAccounts.SaveAccounts();
            await ReplyAsync($"You gained {xp} XP.");
        }

        [Command("ModlogChannel"), Alias("Mc")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task AddIdIntoConfig(SocketGuildChannel chnl)
        {
            var config = GuildConfig.GetGuildConfig(Context.Guild.Id) ?? GuildConfig.CreateGuildConfig(Context.Guild.Id);
            config.ChannelId = chnl.Id;
            GuildConfig.SaveGuildConfig();
            var embed = new EmbedBuilder();
            embed.WithDescription($"Set #{chnl.Name} as the modlog channel for this server.");
            embed.WithColor(SIVA.Config.bot.DefaultEmbedColour);
            embed.WithFooter(Utilities.GetFormattedLocaleMsg("CommandFooter", Context.User.Username));

            await SendMessage(embed);
        }

        [Command("SupportReactionEmoji"), Alias("SRE")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task AddReactToJson(string emoji)
        {
            var config = GuildConfig.GetGuildConfig(Context.Guild.Id) ?? GuildConfig.CreateGuildConfig(Context.Guild.Id);
            config.ReactionEmoji = emoji;
            GuildConfig.SaveGuildConfig();
            await ReplyAsync($":{emoji}: set as the Support Ticket close emoji for this Guild.");
        }

        [Command("SupportCloseOwnTicket"), Alias("SCOT")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task AddBooleanToJson(bool arg)
        {
            var config = GuildConfig.GetGuildConfig(Context.Guild.Id) ?? GuildConfig.CreateGuildConfig(Context.Guild.Id);
            config.CanCloseOwnTicket = arg;
            GuildConfig.SaveGuildConfig();
            await ReplyAsync($"{arg} set as the Support Ticket `CanCloseOwnTicket` option.");
        }

        [Command("SupportChannelName"), Alias("SCN")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task AddChannelToConfig(string arg)
        {
            var config = GuildConfig.GetGuildConfig(Context.Guild.Id) ?? GuildConfig.CreateGuildConfig(Context.Guild.Id);
            config.SupportChannelName = arg;
            GuildConfig.SaveGuildConfig();
            await ReplyAsync($"{arg} set as the Support channel name.");
        }

        [Command("SupportCategoryId"), Alias("SCI")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task SetIdIntoConfig(ulong id)
        {
            var config = GuildConfig.GetGuildConfig(Context.Guild.Id) ?? GuildConfig.CreateGuildConfig(Context.Guild.Id);
            config.SupportCategoryId = id;
            GuildConfig.SaveGuildConfig();
            await ReplyAsync($"{id.ToString()} set as the support channel category ID.");
        }

        [Command("SupportRole"), Alias("SR")]
        [RequireUserPermission(Discord.GuildPermission.Administrator)]
        public async Task SetRoleInConfig(string role)
        {
            var config = GuildConfig.GetGuildConfig(Context.Guild.Id) ?? GuildConfig.CreateGuildConfig(Context.Guild.Id);
            config.SupportRole = role;
            GuildConfig.SaveGuildConfig();
            await ReplyAsync($"`{role}` set as the role to manage tickets.");
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
                await ReplyAsync("This server doesn't allow you to close your own ticket!");
            }
            else
            {
                if (supportChannel == null)
                {
                    await ReplyAsync("You don't have a support channel made.");
                }
                else
                {
                    await supportChannel.DeleteAsync();
                    await ReplyAsync($"Your ticket - \"{supportChannel.Name}\" - has been deleted.");
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
                await ReplyAsync("This server doesn't allow you to close your own ticket!");
            }
            else
            {
                if (supportChannel == null)
                {
                    await ReplyAsync("You don't have a support channel made.");
                }
                else
                {
                    await supportChannel.DeleteAsync();
                    await ReplyAsync($"Your ticket - \"{supportChannel.Name}\" - has been deleted.");
                }
            }
        }

        [Command("WelcomeChannel"), Alias("Wc")]
        [RequireUserPermission(GuildPermission.ManageGuild)]
        public async Task SetIdIntoConfig(SocketGuildChannel chnl)
        {
            var config = GuildConfig.GetGuildConfig(Context.Guild.Id) ??
                         GuildConfig.CreateGuildConfig(Context.Guild.Id);
            var embed = new EmbedBuilder();
            embed.WithDescription($"Set this guild's welcome channel to #{chnl}.");
            embed.WithColor(SIVA.Config.bot.DefaultEmbedColour);
            embed.WithFooter(Utilities.GetFormattedLocaleMsg("CommandFooter", Context.User.Username));
            config.WelcomeChannel = chnl.Id;
            GuildConfig.SaveGuildConfig();
            await SendMessage(embed);
        }

        [Command("WelcomeMessage"), Alias("Wmsg"), Priority(0)]
        [RequireUserPermission(GuildPermission.ManageGuild)]
        public async Task SetTextIntoConfig([Remainder]string msg)
        {
            var config = GuildConfig.GetGuildConfig(Context.Guild.Id) ??
                         GuildConfig.CreateGuildConfig(Context.Guild.Id);
            var embed = new EmbedBuilder();
            embed.WithDescription($"Set this guild's welcome message to:\n\n ```{msg}```");
            embed.WithColor(SIVA.Config.bot.DefaultEmbedColour);
            embed.WithFooter(Utilities.GetFormattedLocaleMsg("CommandFooter", Context.User.Username));
            config.WelcomeMessage = msg;
            GuildConfig.SaveGuildConfig();
            await SendMessage(embed);
        }

        [Command("WelcomeMessage"), Alias("Wmsg"), Priority(1)]
        [RequireUserPermission(GuildPermission.ManageGuild)]
        public async Task SetTextIntoConfig()
        {
            var config = GuildConfig.GetGuildConfig(Context.Guild.Id) ??
                         GuildConfig.CreateGuildConfig(Context.Guild.Id);
            var embed = new EmbedBuilder();
            embed.WithDescription(config.WelcomeMessage);
            embed.WithColor(SIVA.Config.bot.DefaultEmbedColour);
            embed.WithFooter(Utilities.GetFormattedLocaleMsg("CommandFooter", Context.User.Username));
            await SendMessage(embed);
        }

        [Command("LeavingMessage"), Alias("Lmsg")]
        [RequireUserPermission(GuildPermission.ManageGuild)]
        public async Task SetTextIntoConfig1([Remainder]string msg)
        {
            var config = GuildConfig.GetGuildConfig(Context.Guild.Id) ??
                         GuildConfig.CreateGuildConfig(Context.Guild.Id);
            var embed = new EmbedBuilder();
            embed.WithDescription($"Set this guild's leaving message to:\n\n ```{msg}```");
            embed.WithColor(SIVA.Config.bot.DefaultEmbedColour);
            embed.WithFooter(Utilities.GetFormattedLocaleMsg("CommandFooter", Context.User.Username));
            config.LeavingMessage = msg;
            GuildConfig.SaveGuildConfig();
            await SendMessage(embed);
        }

        [Command("WelcomeColour"), Alias("Wcl", "WelcomeColor")]
        [RequireUserPermission(GuildPermission.ManageGuild)]
        public async Task SetIntIntoConfig(int arg1, int arg2, int arg3)
        {
            var config = GuildConfig.GetGuildConfig(Context.Guild.Id) ??
                         GuildConfig.CreateGuildConfig(Context.Guild.Id);
            config.WelcomeColour1 = arg1;
            config.WelcomeColour2 = arg2;
            config.WelcomeColour3 = arg3;
            GuildConfig.SaveGuildConfig();
            var embed = new EmbedBuilder();
            embed.WithColor(SIVA.Config.bot.DefaultEmbedColour);
            embed.WithDescription(Utilities.GetFormattedLocaleMsg("WelcomeColourText", arg1, arg2, arg3));
            embed.WithFooter(Utilities.GetFormattedLocaleMsg("CommandFooter", Context.User.Username));

            await SendMessage(embed);
        }

        [Command("Levels"), Alias("L")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task Leveling(bool arg)
        {
            var config = GuildConfig.GetGuildConfig(Context.Guild.Id) ?? GuildConfig.CreateGuildConfig(Context.Guild.Id);
            var embed = new EmbedBuilder();
            embed.WithFooter(Utilities.GetFormattedLocaleMsg("CommandFooter", Context.User.Username));
            embed.WithColor(SIVA.Config.bot.DefaultEmbedColour);
            embed.WithDescription(arg ? "Enabled leveling for this server." : "Disabled leveling for this server.");
            config.Leveling = arg;
            GuildConfig.SaveGuildConfig();

            await SendMessage(embed);
        }

        [Command("ServerPrefix")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task SetGuildPrefix([Remainder]string prefix)
        {
            var config = GuildConfig.GetGuildConfig(Context.Guild.Id) ?? GuildConfig.CreateGuildConfig(Context.Guild.Id);
            var embed = new EmbedBuilder();
            embed.WithDescription("Done.");
            embed.WithFooter(Utilities.GetFormattedLocaleMsg("CommandFooter", Context.User.Username));
            embed.WithColor(SIVA.Config.bot.DefaultEmbedColour);
            config.CommandPrefix = prefix;
            GuildConfig.SaveGuildConfig();
            await SendMessage(embed);
        }

        [Command("AutoRole")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task AutoRoleRoleAdd([Remainder]string arg = "")
        {

            var config = GuildConfig.GetOrCreateConfig(Context.Guild.Id);
            config.RoleToApply = arg;
            GuildConfig.SaveGuildConfig();

            var embed = new EmbedBuilder();
            embed.WithDescription(Utilities.GetFormattedLocaleMsg("AutoroleCommandText", arg));
            embed.WithThumbnailUrl(Context.Guild.IconUrl);
            embed.WithColor(SIVA.Config.bot.DefaultEmbedColour);

            await SendMessage(embed);


        }
        
        public async Task SendMessage(Embed embed, string message = "", bool isTTS = false)
        {
            await ReplyAsync(message, isTTS, embed);
        }
    }
}
