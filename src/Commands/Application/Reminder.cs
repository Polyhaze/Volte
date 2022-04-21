using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Discord;
using Gommon;
using Volte.Entities;
using Volte.Helpers;
using Volte.Interactions;
using Volte.Services;

namespace Volte.Commands.Application
{
    public sealed class ReminderCommand : ApplicationCommand
    {
        public ReminderCommand() : base("reminder", "Create, view & delete reminders.") => Signature(o =>
        {
            o.Subcommand("view", "View and/or delete reminders; selectable via dropdown menu.");

            o.Subcommand("create", "Create a reminder.", x =>
            {
                x.RequiredString("time-from-now",
                    "When do you want to be reminded? i.e. 2d6h2s, 2 days 6 hours 2 seconds.");
                x.RequiredString("content", "What do you want to be reminded of?");
            });
        });

        public override async Task HandleSlashCommandAsync(SlashCommandContext ctx)
        {
            var reply = ctx.CreateReplyBuilder(true);
            var subcommand = ctx.Options.First().Value;
            var timeFromNow = subcommand.GetOption("time-from-now")?.GetAsString();
            var reminderContent = subcommand.GetOption("content")?.GetAsString();
            if (subcommand.Name is "create")
            {
                var reminders = ctx.Db.GetReminders(ctx.User);
                if (reminders.Count is 25)
                    reply.WithEmbed(eb => eb.WithTitle("You may not have more than 25 reminders."));
                else
                {
                    var timeSpanRes = await ctx.Services.Get<CommandsService>()
                        .GetTypeParser<TimeSpan>().ParseAsync(timeFromNow);
                    if (timeSpanRes.IsSuccessful)
                    {
                        var end = DateTime.Now.Add(timeSpanRes.Value);
                        ctx.Db.CreateReminder(Reminder.CreateFrom(ctx, end, reminderContent));
                        reply.WithEmbeds(ctx
                            .CreateEmbedBuilder($"I'll remind you about {Format.Code(reminderContent)}.")
                            .WithTitle($"{end.GetDiscordTimestamp(TimestampType.Relative)},"));
                    }
                    else
                        reply.WithEmbed(eb => eb.WithTitle(timeSpanRes.FailureReason));
                }
            }
            else
            {
                var reminders = ctx.Db.GetReminders(ctx.User.Id);
                if (reminders.IsEmpty())
                    reply.WithEmbed(e => e.WithTitle("You don't have any reminders."));
                else
                    reply.WithEmbed(e => e.WithTitle("Choose a reminder below to proceed."))
                        .WithSelectMenu(_getReminderMenu(reminders));
            }

            await reply.RespondAsync();
        }

        private readonly ButtonBuilder _deleteButton = ButtonBuilder.CreatePrimaryButton("Delete reminder",
            "reminder:delete", DiscordHelper.X.ToEmoji());

        private readonly Func<IEnumerable<Reminder>, SelectMenuBuilder> _getReminderMenu = rs
            => new SelectMenuBuilder()
                .WithCustomId("reminder:menu")
                .WithOptions(rs.Take(25)
                    .Select(r
                        => new SelectMenuOptionBuilder()
                            .WithLabel(
                                $"{r.Id}: {(r.ReminderText.Length > 25 ? $"{r.ReminderText.Take(25).Select(x => x.ToString()).Join("")}..." : r.ReminderText)}")
                            .WithValue(r.Id.ToString()))
                    .ToList())
                .WithPlaceholder("Choose a reminder...");

        public override async Task HandleComponentAsync(MessageComponentContext ctx)
        {
            switch (ctx.Id.Action)
            {
                case "delete":
                    var reply = ctx.CreateReplyBuilder(true);
                    var fields = ctx.Interaction.Message.Embeds.First().Fields;
                    if (fields.IsEmpty())
                        reply.WithEmbed(x => x.WithTitle("Please select a reminder from the list!"));
                    else
                    {
                        var reminderId = long.Parse(
                            ctx.Interaction.Message.Embeds.First().Fields.First(f => f.Name is "Unique ID").Value);
                        var targetReminder = ctx.Db.GetReminder(reminderId);

                        if (ctx.Db.TryDeleteReminder(targetReminder))
                        {
                            var reminders = ctx.Db.GetReminders(ctx.User.Id);
                            await ctx.UpdateAsync(x =>
                            {
                                x.Components = reminders.IsEmpty()
                                    ? new ComponentBuilder().Build()
                                    : new ComponentBuilder()
                                        .WithSelectMenu(_getReminderMenu(reminders))
                                        .Build();

                                x.Embed = ctx.CreateEmbedBuilder(reminders.IsEmpty()
                                        ? "You've deleted all of your reminders."
                                        : "Please select a reminder from the list.")
                                    .Build();
                            });
                            return;
                        }

                        reply.WithEmbedFrom("Reminder couldn't be deleted.");

                        return;
                    }

                    await reply.RespondAsync();
                    break;
                case "menu":
                    var reminder = ctx.Db.GetReminder(long.Parse(ctx.SelectedMenuOptions.First()));
                    await ctx.Interaction.UpdateAsync(x =>
                    {
                        x.Embed = ctx.CreateEmbedBuilder()
                            .WithTitle(reminder.TargetTime.GetDiscordTimestamp(TimestampType.Relative))
                            .AddField("Unique ID", reminder.Id)
                            .AddField("Reminder", Format.Code(reminder.ReminderText))
                            .AddField("Created", reminder.CreationTime.GetDiscordTimestamp(TimestampType.LongDateTime))
                            .Build();
                        x.Components = new ComponentBuilder()
                            .WithButton(_deleteButton)
                            .WithSelectMenu(_getReminderMenu(ctx.Db.GetReminders(ctx.User.Id)))
                            .Build();
                    });
                    break;
            }
        }
    }
}