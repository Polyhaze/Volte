using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Gommon;
using Volte.Entities;
using Volte.Helpers;
using Volte.Interactions;
using Volte.Services;

namespace Volte.Commands.Application
{
    public class ModerationCommand : ApplicationCommand
    {
        public ModerationCommand() : base("mod", "Volte's moderation command suite.", true) => Signature(o =>
        {
            o.Subcommand("ban", "Ban someone.", x =>
            {
                x.RequiredUser("target", "The target of your fury.");
                x.OptionalString("reason", "Why are you banning this person?");

                x.OptionalInteger("delete-days", "The amount of days of messages to delete.",
                    choices: new Choices(
                        ("None", 0), ("1", 1),
                        ("2", 2), ("3", 3),
                        ("4", 4), ("5", 5),
                        ("6", 6), ("7", 7)
                    ));
            });
            o.Subcommand("ban-by-id",
                "Ban someone by their Snowflake ID, allowing the ability to ban people who are not in your guild.",
                x =>
                {
                    x.RequiredString("target", "The ID of the user to ban.");
                    x.OptionalString("reason", "Why are you banning this person?");
                });
        });

        public override Task<bool> RunSlashChecksAsync(SlashCommandContext ctx) =>
            Task.FromResult(ctx.IsModerator(ctx.GuildUser));

        public override async Task HandleSlashCommandAsync(SlashCommandContext ctx)
        {
            var reply = ctx.CreateReplyBuilder(true);
            var subcommand = ctx.Options.First().Value;

            if (!await RunSlashChecksAsync(ctx))
            {
                await reply.WithEmbed(x => x.WithTitle("You are not a server moderator.").WithErrorColor())
                    .RespondAsync();
                return;
            }

            switch (subcommand.Name)
            {
                case "ban":
                    var target = subcommand.GetOption("target").GetAsGuildUser();
                    var daysToDelete = subcommand.GetOption("delete-days").GetAsInteger();
                    var reason = subcommand.GetOption("reason").GetAsString() ?? "Banned by a moderator.";

                    var e = ctx.CreateEmbedBuilder(
                            $"You've been banned from {Format.Bold(ctx.Guild.Name)} for {Format.Bold(reason)}.")
                        .ApplyConfig(ctx.GuildSettings);

                    if (!await target.TrySendMessageAsync(embed: e.Build()))
                        Logger.Warn(LogSource.Module, $"encountered a 403 when trying to message {target}!");

                    try
                    {
                        await target.BanAsync(daysToDelete, reason);
                        reply.WithEmbed(x => x.WithTitle($"Successfully banned {target}."))
                            .WithButtons(ButtonBuilder.CreateDangerButton("Undo", "mod:ban:undoBan",
                                DiscordHelper.ArrowBackwards.ToEmoji()));


                        await ctx.Services.Get<ModerationService>().OnModActionCompleteAsync(ModActionEventArgs.New
                            .WithDefaultsFromContext(ctx)
                            .WithActionType(ModActionType.Ban)
                            .WithTarget(target)
                            .WithReason(reason), ctx.CreateEmbedBuilder(), ctx.GuildSettings, ctx.TextChannel);
                    }
                    catch
                    {
                        reply.WithEmbeds(ctx.CreateEmbedBuilder()
                            .WithTitle("Something happened.")
                            .WithDescription(
                                "I couldn't ban that person. Am I lacking permission, or are they in a higher position in the role hierarchy?"));
                    }

                    break;
            }

            await reply.RespondAsync();
        }
    }
}