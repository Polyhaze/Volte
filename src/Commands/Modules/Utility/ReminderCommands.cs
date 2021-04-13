using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Gommon;
using Humanizer;
using Qmmands;
using Volte.Core.Entities;

namespace Volte.Commands.Modules
{
    public partial class UtilityModule
    {
        [Command("Remind", "RemindMe", "Reminder")]
        [Description("Creates a reminder.")]
        [ShowTimeFormatInHelp]
        public Task<ActionResult> ReminderAsync(
            [Description("The time, from now, to set the reminder for.")]
            TimeSpan time,
            [Description("What you wanted to be reminded of."), Remainder]
            string reminder)
        {
            var end = Context.Now.Add(time);
            Db.CreateReminder(Reminder.FromContext(Context, end, reminder));
            return Ok(
                $"I'll remind you {end.FormatBoldString()} ({end.Humanize(dateToCompareAgainst: Context.Now)}).");
        }

        [Command("Reminds", "Reminders")]
        [Description("Lists all of your reminders.")]
        public Task<ActionResult> RemindersAsync(
            [Description(
                "Whether or not to only include reminders made in this current guild; or all of your reminders bot-wide.")]
            bool onlyCurrentGuild = true)
        {
            var pages = (onlyCurrentGuild
                    ? Db.GetReminders(Context.User.Id, Context.Guild.Id)
                    : Db.GetReminders(Context.User)
                ).Select(x => Context.CreateEmbedBuilder()
                    .WithTitle(x.TargetTime.Humanize(false, Context.Now))
                    .AddField("Reminder", Format.Code(x.ReminderText))
                    .AddField("Created", x.CreationTime.FormatBoldString())
                    .AddField("Channel", MentionUtils.MentionChannel(x.ChannelId)))
                .ToList();
            if (pages.IsEmpty())
                return Ok(
                    $"You currently have no reminders set{(onlyCurrentGuild ? " in this guild" : string.Empty)}.");
            if (pages.Count is 1) return Ok(pages.First());
            return Ok(pages);
        }
    }
}