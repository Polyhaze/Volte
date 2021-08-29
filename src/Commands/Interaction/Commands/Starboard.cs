using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Gommon;
using Volte.Core.Helpers;

namespace Volte.Commands.Interaction.Commands
{
    public sealed class StarboardCommand : ApplicationCommand
    {
        public StarboardCommand() : base("starboard", "See or modify this guild's starboard settings.") { }

        public override SlashCommandBuilder GetSlashBuilder(IServiceProvider provider)
            => new SlashCommandBuilder()
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("config")
                    .WithDescription("Enable/disable, or automatically setup the starboard system.")
                    .WithType(ApplicationCommandOptionType.SubCommand))
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("minimum-stars")
                    .WithDescription(
                        "The amount of stars required on a message before it is sent to the starboard channel.")
                    .WithType(ApplicationCommandOptionType.SubCommandGroup)
                    .AddOption(new SlashCommandOptionBuilder()
                        .WithName("get")
                        .WithDescription("Show the current minimum star requirement.")
                        .WithType(ApplicationCommandOptionType.SubCommand))
                    .AddOption(new SlashCommandOptionBuilder()
                        .WithName("set")
                        .WithDescription("Change the minimum star requirement.")
                        .WithType(ApplicationCommandOptionType.SubCommand)
                        .AddOption(new SlashCommandOptionBuilder()
                            .WithName("amount")
                            .WithDescription("The new minimum star requirement.")
                            .WithType(ApplicationCommandOptionType.Integer)
                            .WithRequired(true))))
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("channel")
                    .WithDescription("The channel to send starboard messages to.")
                    .WithType(ApplicationCommandOptionType.SubCommandGroup)
                    .AddOption(new SlashCommandOptionBuilder()
                        .WithName("get")
                        .WithDescription("Show the current starboard channel.")
                        .WithType(ApplicationCommandOptionType.SubCommand))
                    .AddOption(new SlashCommandOptionBuilder()
                        .WithName("set")
                        .WithDescription("Change the starboard channel.")
                        .WithType(ApplicationCommandOptionType.SubCommand)
                        .AddOption(new SlashCommandOptionBuilder()
                            .WithName("channel")
                            .WithDescription("The new starboard channel.")
                            .WithType(ApplicationCommandOptionType.Channel)
                            .WithRequired(true))));

        public override async Task HandleSlashCommandAsync(SlashCommandContext ctx)
        {
            var reply = ctx.CreateReplyBuilder().WithEphemeral();
            var subcommandGroup =
                ctx.Options.Values.First(); //setting to inspect/modify, or individual subcommands
            var subcommand = subcommandGroup.Options?.FirstOrDefault(); //get or set
            var argument = subcommand?.Options?.FirstOrDefault()
                ?.Value; //null if subcommand = get; value present if subcommand = set.

            switch (subcommandGroup.Name)
            {
                case "channel":
                    if (subcommand?.Name is "get")
                    {
                        var channel =
                            ctx.Guild.GetTextChannel(ctx.GuildSettings.Configuration.Starboard.StarboardChannel);
                        reply.WithEmbedFrom(channel is null
                            ? "The starboard channel is currently not set."
                            : $"The current starboard channel is {channel.Mention}.");
                    }
                    else
                    {
                        var newChannel = argument.Cast<SocketChannel>();
                        if (!(newChannel is SocketTextChannel textChannel))
                            reply.WithEmbedFrom("You can only use text channels.");
                        else
                        {
                            reply.WithEmbedFrom($"The new starboard channel is {textChannel.Mention}.");
                            ctx.ModifyGuildSettings(data =>
                                data.Configuration.Starboard.StarboardChannel = newChannel.Id);
                        }
                    }

                    break;
                case "minimum-stars":
                    if (subcommand?.Name is "get")
                        reply.WithEmbedFrom(
                            $"The current minimum star requirement is **{ctx.GuildSettings.Configuration.Starboard.StarsRequiredToPost}**.");
                    else
                    {
                        var newRequirement = int.Parse(argument?.ToString()!);
                        reply.WithEmbedFrom($"The new minimum star requirement is **{newRequirement}**.");
                        ctx.ModifyGuildSettings(data =>
                            data.Configuration.Starboard.StarsRequiredToPost = newRequirement);
                    }

                    break;
                case "config":
                    reply.WithEmbeds(ctx.CreateEmbedBuilder()
                            .WithTitle("Would you like to change anything?")
                            .AppendDescriptionLine($"{DiscordHelper.BallotBoxWithCheck}: Enable the starboard.")
                            .AppendDescriptionLine($"{DiscordHelper.OctagonalSign}: Disable the starboard.")
                            .AppendDescriptionLine($"{DiscordHelper.SpaceInvader}: Setup the starboard. " +
                                                   "This will create a channel, named `starboard`, with read-only permissions for the `@everyone` role, as well as enabling the starboard feature.")
                            .AppendDescriptionLine("**NOTE**: This will overwrite your current starboard channel."))
                        .WithActionRows(GetConfigCommandButtons().AsActionRow());
                    break;
            }

            await reply.RespondAsync();
        }

        private IEnumerable<IMessageComponent> GetConfigCommandButtons()
            => new[]
                {
                    ButtonBuilder.CreateSuccessButton("Enable", "starboard:config:on",
                        DiscordHelper.BallotBoxWithCheck.ToEmoji()),
                    ButtonBuilder.CreateDangerButton("Disable", "starboard:config:off",
                        DiscordHelper.OctagonalSign.ToEmoji()),
                    ButtonBuilder.CreateSecondaryButton("Setup", "starboard:config:setup",
                        DiscordHelper.SpaceInvader.ToEmoji())
                }.Select(x => x.Build());
        

        public override async Task HandleComponentAsync(MessageComponentContext ctx)
        {
            var reply = ctx.CreateReplyBuilder().WithEphemeral()
                .WithActionRows(GetConfigCommandButtons().AsActionRow());

            if (ctx.CustomIdParts[1] is "config")
                switch (ctx.CustomIdParts[2])
                {
                    case "on":
                        reply.WithEmbedFrom("Starboard has been enabled.");
                        ctx.ModifyGuildSettings(data => data.Configuration.Starboard.Enabled = true);
                        break;
                    case "off":
                        reply.WithEmbedFrom("Starboard has been disabled.");
                        ctx.ModifyGuildSettings(data => data.Configuration.Starboard.Enabled = false);
                        break;
                    case "setup":
                        var channel = await ctx.Guild.CreateTextChannelAsync("starboard", props =>
                        {
                            props.CategoryId = ctx.TextChannel.CategoryId;
                            props.PermissionOverwrites = new Overwrite(
                                    ctx.Guild.EveryoneRole.Id,
                                    PermissionTarget.Role,
                                    new OverwritePermissions(viewChannel: PermValue.Allow,
                                        sendMessages: PermValue.Deny))
                                .AsSingletonList();
                        });

                        ctx.ModifyGuildSettings(data =>
                        {
                            data.Configuration.Starboard.Enabled = true;
                            data.Configuration.Starboard.StarboardChannel = channel.Id;
                        });
                        reply.WithEmbedFrom(
                            $"Successfully configured the Starboard functionality, and any starred messages will go to {channel.Mention}.");
                        break;
                }

            await reply.RespondAsync();
        }
    }
}