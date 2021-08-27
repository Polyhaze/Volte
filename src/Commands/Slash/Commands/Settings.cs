using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Gommon;
using Volte.Core.Helpers;

namespace Volte.Commands.Slash.Commands
{
    public class SettingsCommand : SlashCommand
    {
        public SettingsCommand() : base("settings", "See or modify your guild's settings.") { }

        public override async Task HandleAsync(SlashCommandContext ctx)
        {
            var reply = ctx.CreateReplyBuilder().WithEphemeral();
            var subcommandGroup =
                ctx.Backing.Data.Options.First(); //setting to inspect/modify, or individual subcommands
            var subcommand = subcommandGroup.Options?.FirstOrDefault(); //get or set
            var argument =
                subcommand?.Options?.FirstOrDefault()
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
            }


            await reply.RespondAsync();
        }

        public override SlashCommandBuilder GetCommandBuilder(IServiceProvider provider)
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