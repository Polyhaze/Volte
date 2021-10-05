using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Volte.Helpers;
using Volte.Interactions;

namespace Volte.Commands.Application
{
    public class SettingsCommand : ApplicationCommand
    {
        public SettingsCommand() : base("settings", "See or modify your guild's settings.", true) => Signature(o =>
        {
            o.SubcommandGroup("admin-role", "Get or set the current guild's Admin role.", x =>
            {
                x.Subcommand("get", "Get the current guild's Admin role.");
                x.Subcommand("set", "Set the current guild's Admin role.", opts =>
                    opts.RequiredRole("role", "The Admin role you want.")
                );
            });
            o.SubcommandGroup("mod-role", "Get or set the current guild's Moderator role.", x =>
            {
                x.Subcommand("get", "Get the current guild's Moderator role.");
                x.Subcommand("set", "Set the current guild's Moderator role.", opts =>
                    opts.RequiredRole("role", "The Moderator role you want.")
                );
            });
            o.SubcommandGroup("auto-role", "Get or set the current guild's Autorole.", x =>
            {
                x.Subcommand("get", "Get the current guild's Autorole.");
                x.Subcommand("set", "Set the current guild's Autorole.", opts =>
                    opts.RequiredRole("role", "The role to be given when new members join this guild.")
                );
            });
            o.SubcommandGroup("anti-link", "Get or set the current guild's Antilink setting.", x =>
            {
                x.Subcommand("get", "Get the current guild's Antilink setting.");
                x.Subcommand("set", "Set the current guild's Antilink setting.", opts =>
                    opts.RequiredBoolean("enabled", "Whether or not to enable this setting.")
                );
            });
            o.SubcommandGroup("embed-tags", "Get or set the current guild's tag embedding setting.", x =>
            {
                x.Subcommand("get", "Get the current guild's tag embedding setting.");
                x.Subcommand("set", "Set the current guild's tag embedding setting.", opts =>
                    opts.RequiredBoolean("enabled", "Whether or not to enable this setting.")
                );
            });
            o.SubcommandGroup("mod-log-channel", "Get or set the current guild's modlog channel.", x =>
            {
                x.Subcommand("get", "Get the current guild's modlog channel.");
                x.Subcommand("set", "Set the current guild's modlog channel.", opts =>
                    opts.OptionalChannel("channel",
                        "The new modlog channel. Omitting this will disable the modlog.")
                );
            });
            o.SubcommandGroup("mass-mention-checks", "Get or set the current guild's mass mention checks setting.",
                x =>
                {
                    x.Subcommand("get", "Get the current guild's mass mention checks setting.");
                    x.Subcommand("set", "Set the current guild's mass mention checks setting.", opts =>
                        opts.RequiredBoolean("enabled", "Whether or not to enable this setting.")
                    );
                });
            o.SubcommandGroup("prefix", "Get or set the current guild's text command prefix.", x =>
            {
                x.Subcommand("get", "Get the current guild's text command prefix.");
                x.Subcommand("set", "Set the current guild's text command prefix.", opts =>
                    opts.RequiredString("text", "The new prefix for text commands.")
                );
            });
            o.SubcommandGroup("auto-quote", "Get or set the current guild's Auto-quote setting.", x =>
            {
                x.Subcommand("get", "Get the current guild's Auto-quote setting.");
                x.Subcommand("set", "Set the current guild's Auto-quote setting.", opts =>
                    opts.RequiredBoolean("enabled", "Whether or not to enable this setting.")
                );
            });
            o.SubcommandGroup("reply-inline",
                "Get or set the current guild's text command inline replying setting.", x =>
                {
                    x.Subcommand("get", "Get the current guild's text command inline replying setting.");
                    x.Subcommand("set", "Set the current guild's text command inline replying setting.", opts =>
                        opts.RequiredBoolean("enabled", "Whether or not to enable this setting.")
                    );
                });
            o.SubcommandGroup("show-moderator",
                "Get or set the current guild's moderator in punishment DM setting.", x =>
                {
                    x.Subcommand("get", "Get the current guild's moderator in punishment DM setting.");
                    x.Subcommand("set", "Set the current guild's moderator in punishment DM setting.", opts =>
                        opts.RequiredBoolean("enabled", "Whether or not to enable this setting.")
                    );
                });
            o.SubcommandGroup("account-age-warnings", "Get or set the current guild's account age warning setting.",
                x =>
                {
                    x.Subcommand("get", "Get the current guild's account age warning setting.");
                    x.Subcommand("set", "Set the current guild's account age warning setting.", opts =>
                        opts.RequiredBoolean("enabled", "Whether or not to enable this setting.")
                    );
                });
            o.SubcommandGroup("verification-role",
                "Get or set the current guild's role-based verification settings.", x =>
                {
                    x.Subcommand("get", "Get the current guild's role-based verification settings.");
                    x.Subcommand("set", "Set the current guild's role-based verification settings.", opts =>
                        {
                            opts.RequiredString("type", "The type of verification role you'd like to set.", sco =>
                            {
                                sco.AddChoice("Unverified", "u");
                                sco.AddChoice("Verified", "v");
                            });
                            opts.RequiredRole("role", "The role to use.");
                        }
                    );
                });
            o.Subcommand("dump", "Dumps the current guild configuration for support purposes.");
        });
        
        public override Task<bool> RunSlashChecksAsync(SlashCommandContext ctx) => Task.FromResult(ctx.IsAdmin(ctx.GuildUser));

        public override async Task HandleSlashCommandAsync(SlashCommandContext ctx)
        {
            var reply = ctx.CreateReplyBuilder().WithEphemeral();
            
            if (!await RunSlashChecksAsync(ctx))
            {
                await reply.WithEmbed(x => x.WithTitle("You are not a server administrator.").WithErrorColor())
                    .RespondAsync();
                return;
            }
            
            var subcommandGroup =
                ctx.Options.First().Value; //setting to inspect/modify, or individual subcommands
            var subcommand = subcommandGroup.Options?.FirstOrDefault(); //get or set
            var argument =
                subcommand?.Options.FirstOrDefault(); //null if subcommand = get; value present if subcommand = set.

            switch (subcommandGroup.Name)
            {
                case "dump":
                    reply.WithEmbedFrom(
                        $"{await HttpHelper.PostToGreemPasteAsync(ctx.GuildSettings.ToString(), ctx.Services, "json")}");
                    break;
                case "admin-role":
                    if (subcommand?.Name is "get")
                        reply.WithEmbedFrom(
                            $"The current Admin role for this guild is {ctx.Guild.GetRole(ctx.GuildSettings.Configuration.Moderation.AdminRole).Mention}.");
                    else
                    {
                        var newRole = argument!.GetAsRole();
                        reply.WithEmbedFrom($"The new Admin role for this guild is {newRole.Mention}.");
                        ctx.ModifyGuildSettings(data => data.Configuration.Moderation.AdminRole = newRole.Id);
                    }

                    break;
                case "mod-role":
                    if (subcommand?.Name is "get")
                        reply.WithEmbedFrom(
                            $"The current Moderator role for this guild is {ctx.Guild.GetRole(ctx.GuildSettings.Configuration.Moderation.ModRole).Mention}.");
                    else
                    {
                        var newRole = argument!.GetAsRole();
                        reply.WithEmbedFrom($"The new Moderator role for this guild is {newRole.Mention}.");
                        ctx.ModifyGuildSettings(data => data.Configuration.Moderation.ModRole = newRole.Id);
                    }

                    break;
                case "auto-role":
                    if (subcommand?.Name is "get")
                        reply.WithEmbedFrom(
                            $"The current Autorole for this guild is {ctx.Guild.GetRole(ctx.GuildSettings.Configuration.Autorole).Mention}.");
                    else
                    {
                        var newRole = argument!.GetAsRole();
                        reply.WithEmbedFrom($"The new Autorole for this guild is {newRole.Mention}.");
                        ctx.ModifyGuildSettings(data => data.Configuration.Autorole = newRole.Id);
                    }

                    break;
                case "anti-link":
                    if (subcommand?.Name is "get")
                        reply.WithEmbedFrom(ctx.GuildSettings.Configuration.Moderation.Antilink
                            ? "Antilink is currently enabled."
                            : "Antilink is currently disabled.");
                    else
                    {
                        var newSetting = argument!.GetAsBoolean();
                        reply.WithEmbedFrom(newSetting ? "Antilink has been enabled." : "Antilink has been disabled.");
                        ctx.ModifyGuildSettings(data => data.Configuration.Moderation.Antilink = newSetting);
                    }

                    break;
                case "embed-tags":
                    if (subcommand?.Name is "get")
                        reply.WithEmbedFrom(ctx.GuildSettings.Configuration.EmbedTagsAndShowAuthor
                            ? "Tags are currently shown inside embeds."
                            : "Tags are currently *not* shown inside embeds.");
                    else
                    {
                        var newSetting = argument!.GetAsBoolean();
                        reply.WithEmbedFrom(newSetting
                            ? "Tags will show inside an embed."
                            : "Tags will no longer show inside an embed.");
                        ctx.ModifyGuildSettings(data => data.Configuration.EmbedTagsAndShowAuthor = newSetting);
                    }

                    break;
                case "mod-log-channel":
                    if (subcommand?.Name is "get")
                    {
                        var channel =
                            ctx.Guild.GetTextChannel(ctx.GuildSettings.Configuration.Moderation.ModActionLogChannel);
                        reply.WithEmbedFrom(channel is null
                            ? "The modlog channel is currently not set."
                            : $"The current modlog channel is {channel.Mention}.");
                    }
                    else
                    {
                        var newChannel = argument!.GetAsGuildChannel();
                        if (!(newChannel is SocketTextChannel textChannel))
                            reply.WithEmbedFrom("You can only use text channels.");
                        else
                        {
                            reply.WithEmbedFrom($"The new modlog channel for this guild is {textChannel.Mention}.");
                            ctx.ModifyGuildSettings(data =>
                                data.Configuration.Moderation.ModActionLogChannel = newChannel.Id);
                        }
                    }

                    break;
                case "mass-mention-checks":
                    if (subcommand?.Name is "get")
                        reply.WithEmbedFrom(ctx.GuildSettings.Configuration.Moderation.MassPingChecks
                            ? "Mass mention checks are currently enabled."
                            : "Mass mention checks are currently disabled.");
                    else
                    {
                        var newSetting = argument!.GetAsBoolean();
                        reply.WithEmbedFrom(newSetting
                            ? "Enabled mass mention checks."
                            : "Disabled mass mention checks.");
                        ctx.ModifyGuildSettings(data => data.Configuration.Moderation.MassPingChecks = newSetting);
                    }

                    break;
                case "prefix":
                    if (subcommand?.Name is "get")
                        reply.WithEmbedFrom(
                            $"The text command prefix for this guild is {ctx.GuildSettings.Configuration.CommandPrefix}.");
                    else
                    {
                        var newPrefix = argument!.GetAsString();
                        reply.WithEmbedFrom($"The new text command prefix for this guild is {Format.Bold(newPrefix)}.");
                        ctx.ModifyGuildSettings(data => data.Configuration.CommandPrefix = newPrefix);
                    }

                    break;
                case "auto-quote":
                    if (subcommand?.Name is "get")
                        reply.WithEmbedFrom(ctx.GuildSettings.Extras.AutoParseQuoteUrls
                            ? "Automatic quoting is currently enabled."
                            : "Automatic quoting is currently disabled.");
                    else
                    {
                        var newSetting = argument!.GetAsBoolean();
                        reply.WithEmbedFrom(newSetting
                            ? "Enabled automatic quoting."
                            : "Disabled automatic quoting.");
                        ctx.ModifyGuildSettings(data => data.Extras.AutoParseQuoteUrls = newSetting);
                    }

                    break;
                case "reply-inline":
                    if (subcommand?.Name is "get")
                        reply.WithEmbedFrom(ctx.GuildSettings.Configuration.ReplyInline
                            ? "Text command inline replying is currently enabled."
                            : "Text command inline replying is currently disabled.");
                    else
                    {
                        var newSetting = argument!.GetAsBoolean();
                        reply.WithEmbedFrom(newSetting
                            ? "Enabled text command inline replying."
                            : "Disabled text command inline replying.");
                        ctx.ModifyGuildSettings(data => data.Configuration.ReplyInline = newSetting);
                    }

                    break;
                case "show-moderator":
                    if (subcommand?.Name is "get")
                        reply.WithEmbedFrom(ctx.GuildSettings.Configuration.Moderation.ShowResponsibleModerator
                            ? "Punishment DMs will now have the responsible moderator as the embed author."
                            : "Punishment DMs will keep the responsible moderator's identity private.");
                    else
                    {
                        var newSetting = argument!.GetAsBoolean();
                        reply.WithEmbedFrom(newSetting
                            ? "Enabled showing responsible moderator."
                            : "Disabled showing responsible moderator.");
                        ctx.ModifyGuildSettings(data =>
                            data.Configuration.Moderation.ShowResponsibleModerator = newSetting);
                    }

                    break;
                case "account-age-warnings":
                    if (subcommand?.Name is "get")
                        reply.WithEmbedFrom(ctx.GuildSettings.Configuration.Moderation.CheckAccountAge
                            ? "New account warnings are currently enabled."
                            : "New account warnings are currently disabled.");
                    else
                    {
                        var newSetting = argument!.GetAsBoolean();
                        reply.WithEmbedFrom(newSetting
                            ? "Enabled new account warnings."
                            : "Disabled new account warnings.");
                        ctx.ModifyGuildSettings(data => data.Configuration.Moderation.CheckAccountAge = newSetting);
                    }

                    break;

                case "verification-role":
                    if (subcommand?.Name is "get")
                        reply.WithEmbedFrom(new StringBuilder()
                            .AppendLine(
                                $"Verified: {ctx.Guild.GetRole(ctx.GuildSettings.Configuration.Moderation.VerifiedRole)?.Mention ?? "not set."}")
                            .AppendLine(
                                $"Unverified: {ctx.Guild.GetRole(ctx.GuildSettings.Configuration.Moderation.UnverifiedRole)?.Mention ?? "not set."}")
                            .ToString());
                    else
                    {
                        var newRole = subcommand!.GetOption("role").GetAsRole();
                        if (subcommand.GetOption("type").GetAsString() is "v")
                        {
                            reply.WithEmbedFrom($"The new Verified user role for this guild is {newRole.Mention}.");
                            ctx.ModifyGuildSettings(data => data.Configuration.Moderation.VerifiedRole = newRole.Id);
                        }
                        else
                        {
                            reply.WithEmbedFrom($"The new Unverified user role for this guild is {newRole.Mention}.");
                            ctx.ModifyGuildSettings(data => data.Configuration.Moderation.UnverifiedRole = newRole.Id);
                        }
                    }

                    break;
            }

            await reply.RespondAsync();
        }
    }
}