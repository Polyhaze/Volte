using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Gommon;
using Volte.Helpers;
using Volte.Interactions;

namespace Volte.Commands.Application
{
    public sealed class StarboardCommand : ApplicationCommand
    {
        public StarboardCommand() : base("starboard", "See or modify this guild's starboard settings.", true) =>
            Signature(o =>
            {
                o.SubcommandGroup("minimum-stars",
                    "The amount of stars required on a message before it is sent to the starboard channel.",
                    x =>
                    {
                        x.Subcommand("get", "Show the current minimum star requirement.");

                        x.Subcommand("set", "Change the minimum star requirement.",
                            opt => opt.RequiredInteger("amount", "The new minimum star requirement."));
                    });

                o.SubcommandGroup("channel", "The channel to send starboard messages to.",
                    x =>
                    {
                        x.Subcommand("get", "Show the current starboard channel.");

                        x.Subcommand("set", "Change the starboard channel.",
                            opt => opt.RequiredChannel("channel", "The new starboard channel."));
                    });
                o.Subcommand("config", "Enable/disable, or automatically setup the starboard system.");
            });

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
                ctx.Options.Values.First(); //setting to inspect/modify, or individual subcommands
            var subcommand = subcommandGroup.Options.FirstOrDefault(); //get or set
            var argument =
                subcommand?.Options.FirstOrDefault(); //null if subcommand = get; value present if subcommand = set.

            switch (subcommandGroup.Name)
            {
                case "channel":
                    if (subcommand!.Name is "get")
                    {
                        var channel =
                            ctx.Guild.GetTextChannel(ctx.GuildSettings.Configuration.Starboard.StarboardChannel);
                        reply.WithEmbedFrom(channel is null
                            ? "The starboard channel is currently not set."
                            : $"The current starboard channel is {channel.Mention}.");
                    }
                    else
                    {
                        var newChannel = argument!.GetAsGuildChannel();
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
                    if (subcommand!.Name is "get")
                        reply.WithEmbedFrom(
                            $"The current minimum star requirement is **{ctx.GuildSettings.Configuration.Starboard.StarsRequiredToPost}**.");
                    else
                    {
                        var newRequirement = argument!.GetAsInteger();
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
                        .WithActionRows(GetConfigCommandButtons(ctx.GuildSettings.Configuration.Starboard.Enabled)
                            .AsActionRow());
                    break;
            }

            await reply.RespondAsync();
        }

        private IEnumerable<IMessageComponent> GetConfigCommandButtons(bool enabled)
            => new[]
            {
                ButtonBuilder.CreateSuccessButton("Enable", "starboard:config:on",
                    DiscordHelper.BallotBoxWithCheck.ToEmoji()).WithDisabled(enabled),
                ButtonBuilder.CreateDangerButton("Disable", "starboard:config:off",
                    DiscordHelper.OctagonalSign.ToEmoji()).WithDisabled(!enabled),
                ButtonBuilder.CreateSecondaryButton("Setup", "starboard:config:setup",
                    DiscordHelper.SpaceInvader.ToEmoji())
            }.Select(x => x.Build());


        public override async Task HandleComponentAsync(MessageComponentContext ctx)
        {
            var messageUpdate = ctx.CreateReplyBuilder().WithEphemeral()
                .WithActionRows(
                    GetConfigCommandButtons(ctx.GuildSettings.Configuration.Starboard.Enabled).AsActionRow());

            if (ctx.Id.Action is "config")
                switch (ctx.Id.Value)
                {
                    case "on":
                        await messageUpdate.WithComponentMessageUpdate(props =>
                        {
                            props.Components = new ComponentBuilder()
                                .AddActionRows(GetConfigCommandButtons(true).AsActionRow()).Build();
                        }).UpdateOrNoopTask;
                        ctx.ModifyGuildSettings(data => data.Configuration.Starboard.Enabled = true);
                        break;
                    case "off":
                        await messageUpdate.WithComponentMessageUpdate(props =>
                        {
                            props.Components = new ComponentBuilder()
                                .AddActionRows(GetConfigCommandButtons(false).AsActionRow()).Build();
                        }).UpdateOrNoopTask;
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
                        await ctx.CreateReplyBuilder().WithEphemeral().WithEmbedFrom(
                                $"Successfully configured the Starboard functionality, and any starred messages will go to {channel.Mention}.")
                            .RespondAsync();
                        break;
                }
        }
    }
}