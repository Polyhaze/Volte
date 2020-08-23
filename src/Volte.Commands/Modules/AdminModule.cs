using System;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using Gommon;
using Qmmands;
using Volte.Commands.Checks;
using Volte.Commands.Results;
using Volte.Core.Models.Guild;
using Volte.Services;

namespace Volte.Commands.Modules
{
    [RequireGuildAdmin]
    public sealed class AdminModule : VolteModule 
    {
        public WelcomeService WelcomeService { get; set; }

        [Group("Welcome", "W")]
        public sealed class WelcomeModule : VolteModule
        {
            [Command("Channel", "C")]
            [Description("Sets the channel used for welcoming new users for this guild.")]
            [Remarks("welcome channel {Channel}")]
            public Task<ActionResult> WelcomeChannelAsync([Remainder]DiscordChannel channel)
            {
                ModifyData(data =>
                {
                    data.Configuration.Welcome.WelcomeChannel = channel.Id;
                    return data;
                });
                return Ok($"Set this guild's welcome channel to {channel.Mention}.");
            }

            [Command("Message", "Msg")]
            [Description(
                "Sets or shows the welcome message used to welcome new users for this guild.")]
            [Remarks("welcomemessage [String]")]
            public Task<ActionResult> WelcomeMessageAsync([Remainder]string message = null)
            {
                if (message is null)
                {
                    return Ok(new StringBuilder()
                        .AppendLine("The current welcome message for this guild is: ```")
                        .AppendLine(Context.GuildData.Configuration.Welcome.WelcomeMessage)
                        .Append("```")
                        .ToString());
                }

                ModifyData(data =>
                {
                    data.Configuration.Welcome.WelcomeMessage = message;
                    return data;
                });
                var welcomeChannel =
                    Context.Guild.GetChannel(Context.GuildData.Configuration.Welcome.WelcomeChannel);
                var sendingTest = welcomeChannel is null
                    ? "Not sending a test message as you do not have a welcome channel set." +
                      "Set a welcome channel to fully complete the setup!"
                    : $"Sending a test message to {welcomeChannel.Mention}. This message will have all formatting and placeholders replaced with an actual value.";

                return Ok(new StringBuilder()
                        .AppendLine($"Set this guild's welcome message to ```{message}```")
                        .AppendLine()
                        .AppendLine($"{sendingTest}").ToString(),
                    async _ =>
                    {
                        if (welcomeChannel is not null)
                        {
                            await new DiscordEmbedBuilder()
                                .WithColor(Context.GuildData.Configuration.Welcome.WelcomeColor)
                                .WithDescription(WelcomeOptions.FormatMessage(message, Context.Member))
                                .WithThumbnail(Context.Member.AvatarUrl)
                                .WithCurrentTimestamp()
                                .SendToAsync(welcomeChannel);
                        }
                    });
            }

            [Command("Color", "Colour", "Cl")]
            [Description("Sets the color used for welcome embeds for this guild.")]
            [Remarks("welcome color {Color}")]
            public Task<ActionResult> WelcomeColorAsync([Remainder]DiscordColor color)
            {
                ModifyData(data =>
                {
                    data.Configuration.Welcome.WelcomeColor = color.Value;
                    return data;
                });
                return Ok("Successfully set this guild's welcome message embed color!");
            }

            [Command("LeavingMessage", "Lmsg")]
            [Description("Sets or shows the leaving message used to say bye for this guild.")]
            [Remarks("welcome leavingmessage [String]")]
            public Task<ActionResult> LeavingMessageAsync([Remainder]string message = null)
            {
                if (message is null)
                {
                    return Ok(new StringBuilder()
                        .AppendLine("The current leaving message for this guild is ```")
                        .AppendLine(Context.GuildData.Configuration.Welcome.LeavingMessage)
                        .Append("```")
                        .ToString());
                }

                ModifyData(data =>
                {
                    data.Configuration.Welcome.LeavingMessage = message;
                    return data;
                });
                var welcomeChannel =
                    Context.Guild.GetChannel(Context.GuildData.Configuration.Welcome.WelcomeChannel);
                var sendingTest = welcomeChannel is null
                    ? "Not sending a test message, as you do not have a welcome channel set. " +
                      "Set a welcome channel to fully complete the setup!"
                    : $"Sending a test message to {welcomeChannel.Mention}.";

                return Ok(new StringBuilder()
                        .AppendLine($"Set this Guild's leaving message to ```{message}```")
                        .AppendLine()
                        .AppendLine($"{sendingTest}")
                        .ToString(),
                    async _ =>
                    {
                        if (welcomeChannel is not null)
                        {
                            await new DiscordEmbedBuilder()
                                .WithColor(Context.GuildData.Configuration.Welcome.WelcomeColor)
                                .WithDescription(WelcomeOptions.FormatMessage(message, Context.Member))
                                .WithThumbnail(Context.Member.AvatarUrl)
                                .WithCurrentTimestamp()
                                .SendToAsync(welcomeChannel);
                        }
                    });
            }

            [Command("DmMessage", "Dmm")]
            [Description("Sets the message to be (attempted to) sent to members upon joining.")]
            [Remarks("welcome dmmessage [String]")]
            public Task<ActionResult> WelcomeDmMessageAsync(string message = null)
            {
                if (message is null)
                {
                    return Ok(
                        $"Unset the WelcomeDmMessage that was previously set to: {Formatter.InlineCode(Context.GuildData.Configuration.Welcome.WelcomeDmMessage)}");
                }

                ModifyData(data =>
                {
                    data.Configuration.Welcome.WelcomeDmMessage = message;
                    return data;
                });
                return Ok($"Set the WelcomeDmMessage to: ```{message}```\n\nAttempting to send a test message.",
                    async _ =>
                    {
                        try
                        {
                            await new DiscordEmbedBuilder()
                                .WithColor(Context.GuildData.Configuration.Welcome.WelcomeColor)
                                .WithDescription(WelcomeOptions.FormatMessage(message, Context.Member))
                                .WithThumbnail(Context.Member.AvatarUrl)
                                .WithCurrentTimestamp()
                                .SendToAsync(Context.Member);
                        }
                        catch (Exception)
                        {
                            
                        }
                        
                    });
            }
        }

        [Command("GuildPrefix", "Gp")]
        [Description("Sets the command prefix for this guild.")]
        [Remarks("guildprefix {String}")]
        public Task<ActionResult> GuildPrefixAsync([Remainder]string newPrefix)
        {
            ModifyData(data =>
            {
                data.Configuration.CommandPrefix = newPrefix;
                return data;
            });
            return Ok($"Set this guild's prefix to **{newPrefix}**.");
        }

        [Command("RemRole", "Rr")]
        [Description("Remove a role from the mentioned user.")]
        [Remarks("remrole {Member} {Role}")]
        public async Task<ActionResult> RemRoleAsync(DiscordMember user, [Remainder] DiscordRole role)
        {
            if (role.Position > Context.Guild.CurrentMember.Hierarchy)
            {
                return BadRequest("Role position is too high for me to be able to remove it from anyone.");
            }

            await user.RevokeRoleAsync(role);
            return Ok($"Removed the role **{role.Name}** from {user.Mention}!");
        }

        [Command("QuoteLinkReply", "QuoteLink", "QuoteReply", "JumpUrlReply", "Qrl", "Qlr")]
        [Description("Enables or disables the Quote link parsing and sending into a channel that a 'Quote URL' is posted to for this guild.")]
        [Remarks("quotelinkreply {Boolean}")]
        public Task<ActionResult> QuoteLinkReplyCommandAsync(bool enabled)
        {
            ModifyData(data =>
            {
                data.Extras.AutoParseQuoteUrls = enabled;
                return data;
            });
            return Ok(enabled ? "Enabled QuoteLinkReply for this guild." : "Disabled QuoteLinkReply for this guild.");
        }

        [Command("PingChecks")]
        [Description("Enable/Disable checking for @everyone and @here for this guild.")]
        [Remarks("pingchecks {Boolean}")]
        public Task<ActionResult> PingChecksAsync(bool enabled)
        {
            ModifyData(data =>
            {
                data.Configuration.Moderation.MassPingChecks = enabled;
                return data;
            });
            return Ok(enabled ? "MassPingChecks has been enabled." : "MassPingChecks has been disabled.");
        }

        [Command("ModRole")]
        [Description("Sets the role able to use Moderation commands for the current guild.")]
        [Remarks("modrole {Role}")]
        public Task<ActionResult> ModRoleAsync([Remainder]DiscordRole role)
        {
            ModifyData(data =>
            {
                data.Configuration.Moderation.ModRole = role.Id;
                return data;
            });
            return Ok($"Set **{role.Name}** as the Moderator role for this guild.");
        }

        [Command("ModLog")]
        [Description("Sets the channel to be used for mod log.")]
        [Remarks("modlog {Channel}")]
        public Task<ActionResult> ModLogAsync(DiscordChannel c)
        {
            ModifyData(data =>
            {
                data.Configuration.Moderation.ModActionLogChannel = c.Id;
                return data;
            });
            return Ok($"Set {c.Mention} as the channel to be used by mod log.");
        }

        [Command("TagShow", "TagSh")]
        [Description("Toggles whether or not Tags requested in your guild will be in an embed and be shown with the person who requested the Tag.")]
        [Remarks("tagshow {Boolean}")]   
        public Task<ActionResult> ShowRequesterAndEmbedTagsAsync(bool enabled)
        {
            ModifyData(data =>
            {
                data.Configuration.EmbedTagsAndShowAuthor = enabled; 
                return data;
            });
            return Ok(enabled
                ? "Tags will now show their requester and be displayed in an embed!"
                : "Tags will **NO LONGER** show their requester and be displayed in an embed!");
        }

        [Command("DeleteMessageOnCommand", "Dmoc")]
        [Description("Enable/Disable deleting the command message upon execution of a command for this guild.")]
        [Remarks("deletemessageoncommand {Boolean}")]
        public Task<ActionResult> DeleteMessageOnCommandAsync(bool enabled)
        {
            ModifyData(data =>
            {
                data.Configuration.DeleteMessageOnCommand = enabled;
                return data;
            });
            return Ok(enabled
                ? "Enabled DeleteMessageOnCommand in this guild."
                : "Disabled DeleteMessageOnCommand in this guild.");
        }

        [Command("DeleteMessageOnTagCommand", "Dmotc")]
        [Description(
            "Enable/Disable deleting the command message upon usage of the tag retrieval command for this guild.")]
        [Remarks("deletemessageontagcommand {Boolean}")]
        public Task<ActionResult> DeleteMessageOnTagCommand(bool enabled)
        {
            ModifyData(data =>
            {
                data.Configuration.DeleteMessageOnTagCommandInvocation = enabled;
                return data;
            });
            return Ok(enabled
                ? "Enabled DeleteMessageOnTagCommand in this guild."
                : "Disabled DeleteMessageOnTagCommand in this guild.");
        }

        [Group("Blacklist", "Bl")]
        [RequireGuildAdmin]
        public sealed class BlacklistModule : VolteModule
        {
            [Command("Add")]
            [Description("Adds a given word/phrase to the blacklist for this guild.")]
            [Remarks("blacklist add {String}")]
            public Task<ActionResult> BlacklistAddAsync([Remainder]string phrase)
            {
                if (!Context.GuildData.Configuration.Moderation.Blacklist.ContainsIgnoreCase(phrase))
                {
                    ModifyData(data =>
                    {
                        data.Configuration.Moderation.Blacklist.Add(phrase);
                        return data;
                    });
                    return Ok($"Added **{phrase}** to the blacklist.");
                }

                return BadRequest($"**{phrase} is already in the blacklist.**");
            }

            [Command("Remove", "Rem")]
            [Description("Removes a given word/phrase from the blacklist for this guild.")]
            [Remarks("blacklist remove {String}")]
            public Task<ActionResult> BlacklistRemoveAsync([Remainder]string phrase)
            {
                if (Context.GuildData.Configuration.Moderation.Blacklist.ContainsIgnoreCase(phrase))
                {
                    var i = Context.GuildData.Configuration.Moderation.Blacklist.IndexOf(phrase);
                    ModifyData(data =>
                    {
                        data.Configuration.Moderation.Blacklist.RemoveAt(i);
                        return data;
                    });
                }

                return BadRequest($"**{phrase}** doesn't exist in the blacklist.");
            }

            [Command("Clear", "Cl")]
            [Description("Clears the blacklist for this guild.")]
            [Remarks("blacklist clear")]
            public Task<ActionResult> BlacklistClearAsync()
            {
                var count = Context.GuildData.Configuration.Moderation.Blacklist.Count;
                ModifyData(data =>
                {
                    data.Configuration.Moderation.Blacklist.Clear();
                    return data;
                });
                return Ok(
                    $"Cleared the this guild's blacklist, containing **{count}** words.");
            }

            [Command("Action", "A")]
            [Description(
                "Sets the action performed when a member uses a blacklisted word/phrase. I.e. says a swear, gets warned. Default is Nothing.")]
            [Remarks("blacklist action {nothing/warn/kick/ban}")]
            public Task<ActionResult> BlacklistActionAsync(string input)
            {
                var action = BlacklistActions.DetermineAction(input);

                ModifyData(data =>
                {
                    data.Configuration.Moderation.BlacklistAction = action;
                    return data;
                });


                return Ok($"Set **{action}** as the action performed when a member uses a blacklisted word/phrase.");
            }
        }

        [Command("Autorole")]
        [Description("Sets the role to be used for Autorole.")]
        [Remarks("autorole {Role}")]
        public Task<ActionResult> AutoroleAsync([Remainder]DiscordRole role)
        {
            ModifyData(data =>
            {
                data.Configuration.Autorole = role.Id;
                return data;
            });
            return Ok($"Successfully set **{role.Name}** as the role to be given to members upon joining this guild.");
        }

        [Command("Antilink", "Al")]
        [Description("Enable/Disable Antilink for the current guild.")]
        [Remarks("antilink {Boolean}")]
        public Task<ActionResult> AntilinkAsync(bool enabled)
        {
            ModifyData(data =>
            {
                data.Configuration.Moderation.Antilink = enabled;
                return data;
            });
            return Ok(enabled ? "Antilink has been enabled." : "Antilink has been disabled.");
        }

        [Command("AdminRole")]
        [Description("Sets the role able to use Admin commands for the current guild.")]
        [Remarks("adminrole {Role}")]
        public Task<ActionResult> AdminRoleAsync([Remainder]DiscordRole role)
        {
            ModifyData(data =>
            {
                data.Configuration.Moderation.AdminRole = role.Id;
                return data;
            });
            return Ok($"Set **{role.Name}** as the Admin role for this guild.");
        }

        [Command("AddRole", "Ar")]
        [Description("Grants a role to the mentioned user.")]
        [Remarks("addrole {Member} {Role}")]
        [RequireBotGuildPermission(Permissions.ManageRoles)]
        public async Task<ActionResult> AddRoleAsync(DiscordMember user, [Remainder] DiscordRole role)
        {
            if (role.Position > Context.Guild.CurrentMember.Hierarchy)
                return BadRequest("Role position is too high for me to be able to grant it to anyone.");

            await user.GrantRoleAsync(role);
            return Ok($"Added the role **{role.Name}** to {user.Mention}!");
        }
    }
}