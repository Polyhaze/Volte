using Discord.Commands;
using System.Threading.Tasks;
using Discord.WebSocket;
using SIVA.Core.JsonFiles;
using System.Linq;
using Discord;
using SIVA.Core.Bot;

namespace SIVA.Core.Modules.Management
{
    
    public class Admin : SivaModule
    {
        
        [Command("ServerName")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task ModifyServerName([Remainder]string name)
        {
            var config = GuildConfig.GetOrCreateConfig(Context.Guild.Id);
            await Context.Guild.ModifyAsync(x => x.Name = name);
            var embed = new EmbedBuilder();
            embed.WithDescription($"Set this server's name to **{name}**!");
            embed.WithColor(new Color(config.EmbedColour1, config.EmbedColour2, config.EmbedColour3));
            embed.WithFooter(Bot.Internal.Utilities.GetFormattedLocaleMsg("CommandFooter", Context.User.Username));

            await SendMessage(embed);
        }

        [Command("BlacklistAdd"), Alias("Bladd")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task AddStringToBl([Remainder]string bl)
        {
            var config = GuildConfig.GetOrCreateConfig(Context.Guild.Id);
            config.Blacklist.Add(bl);
            GuildConfig.SaveGuildConfig();
            var embed = new EmbedBuilder();
            embed.WithDescription($"Added {bl} to the Blacklist.");
            embed.WithColor(new Color(config.EmbedColour1, config.EmbedColour2, config.EmbedColour3));
            embed.WithFooter(Bot.Internal.Utilities.GetFormattedLocaleMsg("CommandFooter", Context.User.Username));

            await SendMessage(embed);
        }

        [Command("BlacklistRemove"), Alias("Blrem")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task RemoveStringFromBl([Remainder]string bl)
        {
            var config = GuildConfig.GetOrCreateConfig(Context.Guild.Id);
            var embed = new EmbedBuilder();
            embed.WithColor(new Color(config.EmbedColour1, config.EmbedColour2, config.EmbedColour3));
            embed.WithFooter(Bot.Internal.Utilities.GetFormattedLocaleMsg("CommandFooter", Context.User.Username));
            if (!config.Blacklist.Contains(bl))
            {
                embed.WithDescription($"`{bl}` isn't present in the Blacklist.");
            }
            else
            {
                embed.WithDescription($"Removed {bl} from the Blacklist.");
                config.Blacklist.Remove(bl);
                GuildConfig.SaveGuildConfig();
            }

            await SendMessage(embed);
        }

        [Command("BlacklistClear"), Alias("Blcl")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task ClearBlacklist()
        {
            var config = GuildConfig.GetOrCreateConfig(Context.Guild.Id);
            config.Blacklist.Clear();
            GuildConfig.SaveGuildConfig();
            var embed = new EmbedBuilder();
            embed.WithDescription("Cleared the Blacklist for this server.");
            embed.WithColor(new Color(config.EmbedColour1, config.EmbedColour2, config.EmbedColour3));
            embed.WithFooter(Bot.Internal.Utilities.GetFormattedLocaleMsg("CommandFooter", Context.User.Username));

            await SendMessage(embed);
        }

        [Command("CustomCommandAdd"), Alias("Cca")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task AddCustomCommand(string commandName, [Remainder]string commandValue)
        {
            var config = GuildConfig.GetOrCreateConfig(Context.Guild.Id);
            config.CustomCommands.Add(commandName, commandValue);
            GuildConfig.SaveGuildConfig();
            var embed = new EmbedBuilder()
                .AddField("Command Name", $"__{commandName}__")
                .AddField("Bot Response", $"**{commandValue}**")
                .WithColor(new Color(config.EmbedColour1, config.EmbedColour2, config.EmbedColour3))
                .WithFooter(Bot.Internal.Utilities.GetFormattedLocaleMsg("CommandFooter", Context.User.Username));

            await SendMessage(embed);
        }

        [Command("CustomCommandRem"), Alias("Ccr")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task RemCustomCommand(string commandName)
        {
            var config = GuildConfig.GetGuildConfig(Context.Guild.Id);
            var embed = new EmbedBuilder()
                .WithColor(new Color(config.EmbedColour1, config.EmbedColour2, config.EmbedColour3))
                .WithFooter(Bot.Internal.Utilities.GetFormattedLocaleMsg("CommandFooter", Context.User.Username));
            if (config.CustomCommands.Keys.Contains(commandName))
            {
                embed.WithDescription($"Removed **{commandName}** as a command!");
                config.CustomCommands.Remove(commandName);
                GuildConfig.SaveGuildConfig();
            }
            else
            {
                embed.WithDescription($"**{commandName}** isn't a command on this server.");
            }

            await SendMessage(embed);
        }

        [Command("ServerLogging"), Alias("Sl")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task SetServerLoggingChannel(bool isEnabled, SocketTextChannel chnl = null)
        {
            string lol;
            var config = GuildConfig.GetOrCreateConfig(Context.Guild.Id);
            if (isEnabled) { lol = "Enabled server logging"; } else { lol = "Disabled server logging"; }
            if (chnl == null) { chnl = (SocketTextChannel)Context.Channel; }
            config.IsServerLoggingEnabled = isEnabled;
            config.ServerLoggingChannel = chnl.Id;
            GuildConfig.SaveGuildConfig();
            var embed = Helpers.CreateEmbed(Context, $"{lol}, and set the channel to <#{chnl.Id}>.");
            await Helpers.SendMessage(Context, embed);
        }

        [Command("AdminRole")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task SetServerAdminRole(string roleName)
        {
            var config = GuildConfig.GetOrCreateConfig(Context.Guild.Id);
            var embed = new EmbedBuilder()
                .WithColor(new Color(config.EmbedColour1, config.EmbedColour2, config.EmbedColour3))
                .WithFooter(Bot.Internal.Utilities.GetFormattedLocaleMsg("CommandFooter", Context.User.Username));
            var role = Context.Guild.Roles.FirstOrDefault(x => x.Name == roleName);
            if (role == null)
            {
                embed.WithDescription($"The role `{roleName}` doesn't exist on this server. Remember that this command is cAsE sEnSiTiVe.");
            }
            else
            {
                embed.WithDescription($"Set the Administrator role to **{roleName}** for this server!");
                config.AdminRole = role.Id;
                GuildConfig.SaveGuildConfig();
            }

            await SendMessage(embed);
        }

        [Command("ModRole")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task SetServerModRole(string roleName)
        {
            var config = GuildConfig.GetOrCreateConfig(Context.Guild.Id);
            var embed = new EmbedBuilder()
                .WithColor(new Color(config.EmbedColour1, config.EmbedColour2, config.EmbedColour3))
                .WithFooter(Bot.Internal.Utilities.GetFormattedLocaleMsg("CommandFooter", Context.User.Username));

            var role = Context.Guild.Roles.FirstOrDefault(x => x.Name == roleName);
            if (role == null)
            {
                embed.WithDescription($"The role `{roleName}` doesn't exist on this server. Remember that this command is cAsE sEnSiTiVe.");
            }
            else
            {
                embed.WithDescription($"Set the Moderator role to **{roleName}** for this server!");
                config.ModRole = role.Id;
                GuildConfig.SaveGuildConfig();
            }

            await SendMessage(embed);
        }

        [Command("EmbedColour"), Alias("Ec", "EmbedColor")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task SetColorForDonatorsIntoJson(int r, int g, int b)
        {
            var config = GuildConfig.GetGuildConfig(Context.Guild.Id);
            var embed = new EmbedBuilder()
                .WithFooter(Bot.Internal.Utilities.GetFormattedLocaleMsg("CommandFooter", Context.User.Username));
            if (config.VerifiedGuild)
            {
                config.EmbedColour1 = r;
                config.EmbedColour2 = g;
                config.EmbedColour3 = b;
                GuildConfig.SaveGuildConfig();
                embed.WithDescription($"Set the embed colour for this guild to `{r} {g} {b}`!");
                embed.WithColor(new Color(r, g, b));
            }
            else
            {
                embed.WithDescription($"This feature and command is for donators only! Consider donating to unlock: `{config.CommandPrefix}donate`.");
                embed.WithColor(new Color(config.EmbedColour1, config.EmbedColour2, config.EmbedColour3));
            }

            await ReplyAsync("", false, embed);
        }

        [Command("PengChecks"), Alias("Pc")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task SetBoolToJson(bool arg)
        {
            var config = GuildConfig.GetGuildConfig(Context.Guild.Id) ??
                         GuildConfig.CreateGuildConfig(Context.Guild.Id);
            var embed = new EmbedBuilder();
            embed.WithColor(new Color(config.EmbedColour1, config.EmbedColour2, config.EmbedColour3));
            embed.WithFooter(Bot.Internal.Utilities.GetFormattedLocaleMsg("CommandFooter", Context.User.Username));
            embed.WithDescription(arg
                ? "Enabled mass peng checks for this server."
                : "Disabled mass peng checks for this server.");

            config.MassPengChecks = arg;
            GuildConfig.SaveGuildConfig();
            await SendMessage(embed);
        }

        [Command("Antilink"), Alias("Al")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task SetBoolIntoConfig(bool setting)
        {
            var config = GuildConfig.GetGuildConfig(Context.Guild.Id) ?? GuildConfig.CreateGuildConfig(Context.Guild.Id);
            config.Antilink = setting;
            GuildConfig.SaveGuildConfig();
            var embed = new EmbedBuilder();
            embed.WithFooter(Bot.Internal.Utilities.GetFormattedLocaleMsg("CommandFooter", Context.User.Username));
            embed.WithColor(new Color(config.EmbedColour1, config.EmbedColour2, config.EmbedColour3));
            if (setting) embed.WithDescription("Enabled Antilink for this server.");
            if (setting == false) embed.WithDescription("Disabled Antilink for this server.");
            await ReplyAsync("", false, embed);

        }

        [Command("TruthOrDare"), Alias("Tod")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task DisableSlashEnableTod(bool setting)
        {
            var config = GuildConfig.GetOrCreateConfig(Context.Guild.Id);
            config.IsTodEnabled = setting;
            GuildConfig.SaveGuildConfig();
            var embed = new EmbedBuilder()
                .WithColor(new Color(config.EmbedColour1, config.EmbedColour2, config.EmbedColour3))
                .WithFooter(Bot.Internal.Utilities.GetFormattedLocaleMsg("CommandFooter", Context.User.Username));
            if (setting) embed.WithDescription("Enabled Truth or Dare for this server.");
            if (setting == false) embed.WithDescription("Disabled Truth or Dare for this server.");
            await ReplyAsync("", false, embed);
        }

        [Command("AntilinkIgnore"), Alias("Ali")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task SetChannelToBeIgnored(string type, SocketGuildChannel chnl = null)
        {
            var config = GuildConfig.GetOrCreateConfig(Context.Guild.Id);
            var embed = new EmbedBuilder()
                .WithColor(new Color(config.EmbedColour1, config.EmbedColour2, config.EmbedColour3))
                .WithFooter(Bot.Internal.Utilities.GetFormattedLocaleMsg("CommandFooter", Context.User.Username));
            switch (type)
            {
                case "add":
                case "Add":
                    config.AntilinkIgnoredChannels.Add(chnl.Id);
                    GuildConfig.SaveGuildConfig();
                    embed.WithDescription($"Added <#{chnl.Id}> to the list of ignored channels for Antilink.");
                    break;
                case "rem":
                case "Rem":
                    config.AntilinkIgnoredChannels.Remove(chnl.Id);
                    GuildConfig.SaveGuildConfig();
                    embed.WithDescription($"Removed <#{chnl.Id}> from the list of ignored channels for Antilink.");
                    break;
                case "clear":
                case "Clear":
                    config.AntilinkIgnoredChannels.Clear();
                    GuildConfig.SaveGuildConfig();
                    embed.WithDescription("List of channels to be ignored by Antilink has been cleared.");
                    break;
                default:
                    embed.WithDescription($"Valid types are `add`, `rem`, and `clear`. Syntax: `{config.CommandPrefix}ali {{add/rem/clear}} [channelMention]`");
                    break;
            }

            await SendMessage(embed);
        }

        [Command("AddXp")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task AddXp(uint xp)
        {
            var account = UserAccounts.GetAccount(Context.User);
            account.Xp += xp;
            UserAccounts.SaveAccounts();
            await ReplyAsync($"You gained {xp} XP.");
        }

        [Command("Award")]
        [RequireUserPermission(GuildPermission.Administrator)]
        [RequireOwner]
        public async Task GiveUserMoney(SocketGuildUser user, int amt)
        {
            var config = GuildConfig.GetOrCreateConfig(Context.Guild.Id);
            var embed = new EmbedBuilder();
            var acc = UserAccounts.GetAccount(user);
            acc.Money += amt;
            UserAccounts.SaveAccounts();
            embed.WithDescription($"{amt} added to **{user.Username}#{user.Discriminator}**'s balance.");
            embed.WithColor(new Color(config.EmbedColour1, config.EmbedColour2, config.EmbedColour3));

            await SendMessage(embed);
        }


        [Command("SupportCloseOwnTicket"), Alias("SCOT")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task AddBooleanToJson(bool arg)
        {
            var config = GuildConfig.GetGuildConfig(Context.Guild.Id) ?? GuildConfig.CreateGuildConfig(Context.Guild.Id);
            config.CanCloseOwnTicket = arg;
            GuildConfig.SaveGuildConfig();
            var embed = Helpers.CreateEmbed(Context, $"{arg} set as the Support Ticket `CanCloseOwnTicket` option.");
            await Helpers.SendMessage(Context, embed);
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

        [Command("SupportRole"), Alias("SR")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task SetRoleInConfig([Remainder]string role)
        {
            var config = GuildConfig.GetGuildConfig(Context.Guild.Id) ?? GuildConfig.CreateGuildConfig(Context.Guild.Id);
            config.SupportRole = role;
            GuildConfig.SaveGuildConfig();
            await ReplyAsync($"`{role}` set as the role to manage tickets.");
        }

        [Command("SupportCloseTicket"), Alias("SCT", "Close"), Priority(0)] 
        [RequireUserPermission(GuildPermission.ManageChannels)]
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

        [Command("SelfRoleAdd"), Alias("SRA")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task AddStringToList([Remainder]string role)
        {
            var config = GuildConfig.GetGuildConfig(Context.Guild.Id);
            var embed = new EmbedBuilder()
                .WithDescription($"Added the {role} to the Config.")
                .WithFooter(Bot.Internal.Utilities.GetFormattedLocaleMsg("CommandFooter", Context.User.Username))
                .WithColor(new Color(config.EmbedColour1, config.EmbedColour2, config.EmbedColour3));
            config.SelfRoles.Add(role);
            GuildConfig.SaveGuildConfig();
            await SendMessage(embed);
        }

        [Command("SelfRoleRem"), Alias("SRR")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task RemoveStringFromList([Remainder]string role)
        {
            var config = GuildConfig.GetGuildConfig(Context.Guild.Id);
            var embed = new EmbedBuilder()
                .WithFooter(Bot.Internal.Utilities.GetFormattedLocaleMsg("CommandFooter", Context.User.Username))
                .WithColor(new Color(config.EmbedColour1, config.EmbedColour2, config.EmbedColour3));
            if (config.SelfRoles.Contains(role))
            {
                config.SelfRoles.Remove(role);
                embed.WithDescription($"Removed {role} from the Self Roles list.");
            }
            else
            {
                embed.WithDescription("That role doesn't exist in your Guild Config.");
            }

            await SendMessage(embed);
        }

        [Command("SelfRoleClear"), Alias("SRC")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task ClearListFromConfig()
        {
            var config = GuildConfig.GetGuildConfig(Context.Guild.Id);
            var embed = new EmbedBuilder()
                .WithColor(new Color(config.EmbedColour1, config.EmbedColour2, config.EmbedColour3))
                .WithFooter(Bot.Internal.Utilities.GetFormattedLocaleMsg("CommandFooter", Context.User.Username));
            if (config == null)
            {
                embed.WithDescription("You don't have a Guild Config created.");
            }
            else
            {
                embed.WithDescription($"Cleared {config.SelfRoles.Count} roles from the self role list.");
                config.SelfRoles.Clear();
            }

            await SendMessage(embed);
        }


        [Command("SupportCloseTicket"), Alias("SCT", "Close"), 
         Priority(1), 
         RequireUserPermission(GuildPermission.ManageChannels)]
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
            embed.WithColor(new Color(config.EmbedColour1, config.EmbedColour2, config.EmbedColour3));
            embed.WithFooter(Bot.Internal.Utilities.GetFormattedLocaleMsg("CommandFooter", Context.User.Username));
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
            embed.WithColor(new Color(config.EmbedColour1, config.EmbedColour2, config.EmbedColour3));
            embed.WithFooter(Bot.Internal.Utilities.GetFormattedLocaleMsg("CommandFooter", Context.User.Username));
            config.WelcomeMessage = msg;
            GuildConfig.SaveGuildConfig();
            await SendMessage(embed);

            if (config.WelcomeChannel != 0)
            {
                var a = config.WelcomeMessage.Replace("{UserMention}", Context.User.Mention);
                var b = a.Replace("{ServerName}", Context.Guild.Name);
                var c = b.Replace("{UserName}", Context.User.Username);
                var d = c.Replace("{OwnerMention}", Context.Guild.Owner.Mention);
                var e = d.Replace("{UserTag}", Context.User.DiscriminatorValue.ToString());

                var channel = Context.Guild.GetTextChannel(config.WelcomeChannel);
                var embed2 = new EmbedBuilder();
                embed2.WithDescription(e);
                embed2.WithColor(new Color(config.WelcomeColour1, config.WelcomeColour2, config.WelcomeColour3));
                embed2.WithFooter($"Guild Owner: {Context.Guild.Owner.Username}#{Context.Guild.Owner.Discriminator}");
                embed2.WithThumbnailUrl(Context.Guild.IconUrl);
                await channel.SendMessageAsync("", false, embed2);
            }
        }

        [Command("WelcomeMessage"), Alias("Wmsg"), Priority(1)]
        [RequireUserPermission(GuildPermission.ManageGuild)]
        public async Task SendWMSGToUser()
        {
            var config = GuildConfig.GetGuildConfig(Context.Guild.Id) ??
                         GuildConfig.CreateGuildConfig(Context.Guild.Id);
            var embed = new EmbedBuilder();
            embed.WithDescription($"The welcome message for this server is: `{config.WelcomeMessage}`");
            embed.WithColor(new Color(config.EmbedColour1, config.EmbedColour2, config.EmbedColour3));
            await SendMessage(embed);
        }

        [Command("LeavingMessage"), Alias("Lmsg")]
        [RequireUserPermission(GuildPermission.ManageGuild)]
        public async Task SetTextIntoConfigLol([Remainder]string msg)
        {
            var config = GuildConfig.GetGuildConfig(Context.Guild.Id) ??
                         GuildConfig.CreateGuildConfig(Context.Guild.Id);
            var embed = new EmbedBuilder();
            embed.WithDescription($"Set this guild's leaving message to:\n\n ```{msg}```\n\nSending a test welcome message to <#{config.WelcomeChannel}>");
            embed.WithColor(new Color(config.EmbedColour1, config.EmbedColour2, config.EmbedColour3));
            embed.WithFooter(Bot.Internal.Utilities.GetFormattedLocaleMsg("CommandFooter", Context.User.Username));
            config.LeavingMessage = msg;
            GuildConfig.SaveGuildConfig();
            await SendMessage(embed);

            if (config.WelcomeChannel != 0)
            {
                var a = config.WelcomeMessage.Replace("{UserMention}", Context.User.Mention);
                var b = a.Replace("{ServerName}", Context.Guild.Name);
                var c = b.Replace("{UserName}", Context.User.Username);
                var d = c.Replace("{OwnerMention}", Context.Guild.Owner.Mention);
                var e = d.Replace("{UserTag}", Context.User.DiscriminatorValue.ToString());

                var channel = Context.Guild.GetTextChannel(config.WelcomeChannel);
                var embed2 = new EmbedBuilder();
                embed2.WithDescription(e);
                embed2.WithColor(new Color(config.WelcomeColour1, config.WelcomeColour2, config.WelcomeColour3));
                embed2.WithFooter($"Guild Owner: {Context.Guild.Owner.Username}#{Context.Guild.Owner.Discriminator}");
                embed2.WithThumbnailUrl(Context.Guild.IconUrl);
                await channel.SendMessageAsync("", false, embed2);
            }
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
            embed.WithColor(new Color(config.EmbedColour1, config.EmbedColour2, config.EmbedColour3));
            embed.WithDescription(Bot.Internal.Utilities.GetFormattedLocaleMsg("WelcomeColourText", arg1, arg2, arg3));
            embed.WithFooter(Bot.Internal.Utilities.GetFormattedLocaleMsg("CommandFooter", Context.User.Username));

            await SendMessage(embed);
        }

        [Command("Levels"), Alias("L")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task Leveling(bool arg)
        {
            var config = GuildConfig.GetGuildConfig(Context.Guild.Id) ?? GuildConfig.CreateGuildConfig(Context.Guild.Id);
            var embed = new EmbedBuilder();
            embed.WithFooter(Bot.Internal.Utilities.GetFormattedLocaleMsg("CommandFooter", Context.User.Username));
            embed.WithColor(new Color(config.EmbedColour1, config.EmbedColour2, config.EmbedColour3));
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
            embed.WithFooter(Bot.Internal.Utilities.GetFormattedLocaleMsg("CommandFooter", Context.User.Username));
            embed.WithColor(new Color(config.EmbedColour1, config.EmbedColour2, config.EmbedColour3));
            config.CommandPrefix = prefix;
            GuildConfig.SaveGuildConfig();
            await SendMessage(embed);
        }

        [Command("AutoRole")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task AutoRoleRoleAdd([Remainder]string arg = "")
        {

            var config = GuildConfig.GetOrCreateConfig(Context.Guild.Id);
            config.Autorole = arg;
            GuildConfig.SaveGuildConfig();

            var embed = new EmbedBuilder();
            embed.WithDescription(Bot.Internal.Utilities.GetFormattedLocaleMsg("AutoroleCommandText", arg));
            embed.WithThumbnailUrl(Context.Guild.IconUrl);
            embed.WithColor(new Color(config.EmbedColour1, config.EmbedColour2, config.EmbedColour3));

            await SendMessage(embed);


        }
        
        public async Task SendMessage(Embed embed, string message = "", bool isTts = false)
        {
            await ReplyAsync(message, isTts, embed);
        }
    }
}
