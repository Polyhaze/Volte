using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Gommon;
using Volte.Core.Helpers;

namespace Volte.Commands.Interaction.Commands
{
    public class SettingsCommand : ApplicationCommand
    {
        public SettingsCommand() : base("settings", "See or modify your guild's settings.") { }

        public override async Task HandleSlashCommandAsync(SlashCommandContext ctx)
        {
            var reply = ctx.CreateReplyBuilder().WithEphemeral();
            var subcommandGroup =
                ctx.Backing.Data.Options.First(); //setting to inspect/modify, or individual subcommands
            var subcommand = subcommandGroup.Options?.FirstOrDefault(); //get or set
            var argument = subcommand?.Options?.FirstOrDefault()
                    ?.Value; //null if subcommand = get; value present if subcommand = set.

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
                        var newRole = argument.Cast<SocketRole>();
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
                        var newRole = argument.Cast<SocketRole>();
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
                        var newRole = argument.Cast<SocketRole>();
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
                        var newSetting = argument.Cast<bool>();
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
                        var newSetting = argument.Cast<bool>();
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
                        var newChannel = argument.Cast<SocketChannel>();
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
                        var newSetting = argument.Cast<bool>();
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
                        var newPrefix = argument!.ToString();
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
                        var newSetting = argument.Cast<bool>();
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
                        var newSetting = argument.Cast<bool>();
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
                        var newSetting = argument.Cast<bool>();
                        reply.WithEmbedFrom(newSetting
                            ? "Enabled showing responsible moderator."
                            : "Disabled showing responsible moderator.");
                        ctx.ModifyGuildSettings(data => data.Configuration.Moderation.ShowResponsibleModerator = newSetting);
                    }
                    break;
                case "account-age-warnings":
                    if (subcommand?.Name is "get")
                        reply.WithEmbedFrom(ctx.GuildSettings.Configuration.Moderation.CheckAccountAge
                            ? "New account warnings are currently enabled."
                            : "New account warnings are currently disabled.");
                    else
                    {
                        var newSetting = argument.Cast<bool>();
                        reply.WithEmbedFrom(newSetting
                            ? "Enabled new account warnings."
                            : "Disabled new account warnings.");
                        ctx.ModifyGuildSettings(data => data.Configuration.Moderation.CheckAccountAge = newSetting);
                    }
                    break;
                
                case "verification-role":
                    if (subcommand?.Name is "get")
                        reply.WithEmbedFrom(new StringBuilder()
                            .AppendLine($"Verified: {ctx.Guild.GetRole(ctx.GuildSettings.Configuration.Moderation.VerifiedRole)?.Mention ?? "not set."}")
                            .AppendLine($"Unverified: {ctx.Guild.GetRole(ctx.GuildSettings.Configuration.Moderation.UnverifiedRole)?.Mention?? "not set."}")
                            .ToString());
                    else
                    {
                        var newRole = subcommand.GetOptionOfValue<SocketRole>("role");
                        if (subcommand.GetOptionOfValue<string>("type") == "v")
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

        public override SlashCommandBuilder GetSlashBuilder(IServiceProvider provider)
            => new SlashCommandBuilder()
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("dump")
                    .WithDescription("Dumps the current guild configuration for support purposes.")
                    .WithType(ApplicationCommandOptionType.SubCommand))
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("admin-role")
                    .WithDescription("Get or set the current guild's Admin role.")
                    .WithType(ApplicationCommandOptionType.SubCommandGroup)
                    .AddOption(new SlashCommandOptionBuilder()
                        .WithName("get")
                        .WithDescription("Get the current guild's Admin role.")
                        .WithType(ApplicationCommandOptionType.SubCommand))
                    .AddOption(new SlashCommandOptionBuilder()
                        .WithName("set")
                        .WithDescription("Set the current guild's Admin role.")
                        .WithType(ApplicationCommandOptionType.SubCommand)
                        .AddOption(new SlashCommandOptionBuilder()
                            .WithName("role")
                            .WithDescription("The admin role you want.")
                            .WithType(ApplicationCommandOptionType.Role)
                            .WithRequired(true))))
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("mod-role")
                    .WithDescription("Get or set the current guild's Moderator role.")
                    .WithType(ApplicationCommandOptionType.SubCommandGroup)
                    .AddOption(new SlashCommandOptionBuilder()
                        .WithName("get")
                        .WithDescription("Get the current guild's Moderator role.")
                        .WithType(ApplicationCommandOptionType.SubCommand))
                    .AddOption(new SlashCommandOptionBuilder()
                        .WithName("set")
                        .WithDescription("Set the current guild's Moderator role.")
                        .WithType(ApplicationCommandOptionType.SubCommand)
                        .AddOption(new SlashCommandOptionBuilder()
                            .WithName("role")
                            .WithDescription("The moderator role you want.")
                            .WithType(ApplicationCommandOptionType.Role)
                            .WithRequired(true))))
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("auto-role")
                    .WithDescription("Get or set the current guild's Autorole setting.")
                    .WithType(ApplicationCommandOptionType.SubCommandGroup)
                    .AddOption(new SlashCommandOptionBuilder()
                        .WithName("get")
                        .WithDescription("Get the current guild's Autorole setting.")
                        .WithType(ApplicationCommandOptionType.SubCommand))
                    .AddOption(new SlashCommandOptionBuilder()
                        .WithName("set")
                        .WithDescription("Set the current guild's Autorole setting.")
                        .WithType(ApplicationCommandOptionType.SubCommand)
                        .AddOption(new SlashCommandOptionBuilder()
                            .WithName("role")
                            .WithDescription("The role to be given when new members join this guild.")
                            .WithType(ApplicationCommandOptionType.Role)
                            .WithRequired(true))))
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("anti-link")
                    .WithDescription("Get or set the current guild's Anti-link setting.")
                    .WithType(ApplicationCommandOptionType.SubCommandGroup)
                    .AddOption(new SlashCommandOptionBuilder()
                        .WithName("get")
                        .WithDescription("Get the current guild's Anti-link setting.")
                        .WithType(ApplicationCommandOptionType.SubCommand))
                    .AddOption(new SlashCommandOptionBuilder()
                        .WithName("set")
                        .WithDescription("Set the current guild's Anti-link setting.")
                        .WithType(ApplicationCommandOptionType.SubCommand)
                        .AddOption(new SlashCommandOptionBuilder()
                            .WithName("enabled")
                            .WithDescription("Whether or not to enable this setting.")
                            .WithType(ApplicationCommandOptionType.Boolean)
                            .WithRequired(true))))
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("embed-tags")
                    .WithDescription("Get or set the current guild's tag embedding setting.")
                    .WithType(ApplicationCommandOptionType.SubCommandGroup)
                    .AddOption(new SlashCommandOptionBuilder()
                        .WithName("get")
                        .WithDescription("Get the current guild's tag embedding setting.")
                        .WithType(ApplicationCommandOptionType.SubCommand))
                    .AddOption(new SlashCommandOptionBuilder()
                        .WithName("set")
                        .WithDescription("Set the current guild's tag embedding setting.")
                        .WithType(ApplicationCommandOptionType.SubCommand)
                        .AddOption(new SlashCommandOptionBuilder()
                            .WithName("enabled")
                            .WithDescription("Whether or not to enable this setting.")
                            .WithType(ApplicationCommandOptionType.Boolean)
                            .WithRequired(true))))
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("mod-log-channel")
                    .WithDescription("Get or set the current guild's modlog channel.")
                    .WithType(ApplicationCommandOptionType.SubCommandGroup)
                    .AddOption(new SlashCommandOptionBuilder()
                        .WithName("get")
                        .WithDescription("Get the current guild's modlog channel.")
                        .WithType(ApplicationCommandOptionType.SubCommand))
                    .AddOption(new SlashCommandOptionBuilder()
                        .WithName("set")
                        .WithDescription("Set the current guild's modlog channel.")
                        .WithType(ApplicationCommandOptionType.SubCommand)
                        .AddOption(new SlashCommandOptionBuilder()
                            .WithName("channel")
                            .WithDescription(
                                "The channel to send modlog messages to. Omitting this will disable the mod log.")
                            .WithType(ApplicationCommandOptionType.Channel)
                            .WithRequired(false))))
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("mass-mention-checks")
                    .WithDescription("Get or set the current guild's mass mention checks setting.")
                    .WithType(ApplicationCommandOptionType.SubCommandGroup)
                    .AddOption(new SlashCommandOptionBuilder()
                        .WithName("get")
                        .WithDescription("Get the current guild's mass mention checks setting.")
                        .WithType(ApplicationCommandOptionType.SubCommand))
                    .AddOption(new SlashCommandOptionBuilder()
                        .WithName("set")
                        .WithDescription("Set the current guild's mass mention checks setting.")
                        .WithType(ApplicationCommandOptionType.SubCommand)
                        .AddOption(new SlashCommandOptionBuilder()
                            .WithName("enabled")
                            .WithDescription("Whether or not to enable this setting.")
                            .WithType(ApplicationCommandOptionType.Boolean)
                            .WithRequired(true))))
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("prefix")
                    .WithDescription("Get or set the current guild's text command prefix.")
                    .WithType(ApplicationCommandOptionType.SubCommandGroup)
                    .AddOption(new SlashCommandOptionBuilder()
                        .WithName("get")
                        .WithDescription("Get the current guild's text command prefix.")
                        .WithType(ApplicationCommandOptionType.SubCommand))
                    .AddOption(new SlashCommandOptionBuilder()
                        .WithName("set")
                        .WithDescription("Set the current guild's text command prefix.")
                        .WithType(ApplicationCommandOptionType.SubCommand)
                        .AddOption(new SlashCommandOptionBuilder()
                            .WithName("prefix")
                            .WithDescription("The new prefix for use with text commands.")
                            .WithType(ApplicationCommandOptionType.String)
                            .WithRequired(true))))
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("auto-quote")
                    .WithDescription("Get or set the current guild's Auto-quote setting.")
                    .WithType(ApplicationCommandOptionType.SubCommandGroup)
                    .AddOption(new SlashCommandOptionBuilder()
                        .WithName("get")
                        .WithDescription("Get the current guild's Auto-quote setting.")
                        .WithType(ApplicationCommandOptionType.SubCommand))
                    .AddOption(new SlashCommandOptionBuilder()
                        .WithName("set")
                        .WithDescription("Set the current guild's Auto-quote setting.")
                        .WithType(ApplicationCommandOptionType.SubCommand)
                        .AddOption(new SlashCommandOptionBuilder()
                            .WithName("enabled")
                            .WithDescription("Whether or not to enable this setting.")
                            .WithType(ApplicationCommandOptionType.Boolean)
                            .WithRequired(true))))
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("reply-inline")
                    .WithDescription("Get or set the current guild's text command inline replying setting.")
                    .WithType(ApplicationCommandOptionType.SubCommandGroup)
                    .AddOption(new SlashCommandOptionBuilder()
                        .WithName("get")
                        .WithDescription("Get the current guild's text command inline replying setting.")
                        .WithType(ApplicationCommandOptionType.SubCommand))
                    .AddOption(new SlashCommandOptionBuilder()
                        .WithName("set")
                        .WithDescription("Set the current guild's text command inline replying setting.")
                        .WithType(ApplicationCommandOptionType.SubCommand)
                        .AddOption(new SlashCommandOptionBuilder()
                            .WithName("enabled")
                            .WithDescription("Whether or not to enable this setting.")
                            .WithType(ApplicationCommandOptionType.Boolean)
                            .WithRequired(true))))
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("show-moderator")
                    .WithDescription("Get or set the current guild's moderator in punishment DM setting.")
                    .WithType(ApplicationCommandOptionType.SubCommandGroup)
                    .AddOption(new SlashCommandOptionBuilder()
                        .WithName("get")
                        .WithDescription("Get the current guild's moderator in punishment DM setting.")
                        .WithType(ApplicationCommandOptionType.SubCommand))
                    .AddOption(new SlashCommandOptionBuilder()
                        .WithName("set")
                        .WithDescription("Set the current guild's moderator in punishment DM setting.")
                        .WithType(ApplicationCommandOptionType.SubCommand)
                        .AddOption(new SlashCommandOptionBuilder()
                            .WithName("enabled")
                            .WithDescription("Whether or not to enable this setting.")
                            .WithType(ApplicationCommandOptionType.Boolean)
                            .WithRequired(true))))
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("account-age-warnings")
                    .WithDescription("Get or set the current guild's account age warning setting.")
                    .WithType(ApplicationCommandOptionType.SubCommandGroup)
                    .AddOption(new SlashCommandOptionBuilder()
                        .WithName("get")
                        .WithDescription("Get the current guild's account age warning setting.")
                        .WithType(ApplicationCommandOptionType.SubCommand))
                    .AddOption(new SlashCommandOptionBuilder()
                        .WithName("set")
                        .WithDescription("Set the current guild's account age warning setting.")
                        .WithType(ApplicationCommandOptionType.SubCommand)
                        .AddOption(new SlashCommandOptionBuilder()
                            .WithName("enabled")
                            .WithDescription("Whether or not to enable this setting.")
                            .WithType(ApplicationCommandOptionType.Boolean)
                            .WithRequired(true))))
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("verification-role")
                    .WithDescription("Get or set the current guild's role-based verification settings.")
                    .WithType(ApplicationCommandOptionType.SubCommandGroup)
                    .AddOption(new SlashCommandOptionBuilder()
                        .WithName("get")
                        .WithDescription("Get the current guild's role-based verification settings.")
                        .WithType(ApplicationCommandOptionType.SubCommand))
                    .AddOption(new SlashCommandOptionBuilder()
                        .WithName("set")
                        .WithDescription("Set the current guild's role-based verification settings.")
                        .WithType(ApplicationCommandOptionType.SubCommand)
                        .AddOption(new SlashCommandOptionBuilder()
                            .WithName("type")
                            .WithDescription("The type of verification role you'd like to set.")
                            .WithType(ApplicationCommandOptionType.String)
                            .WithRequired(true)
                            .AddChoice("Unverified", "u")
                            .AddChoice("Verified", "v"))
                        .AddOption(new SlashCommandOptionBuilder()
                            .WithName("role")
                            .WithDescription("The role to use.")
                            .WithType(ApplicationCommandOptionType.Role)
                            .WithRequired(true))));
    }
}