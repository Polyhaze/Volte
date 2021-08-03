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
    [Group("Remind", "RemindMe", "Reminder")]
    public sealed class ReminderModule : VolteModule
    {
        [Command]
        [ShowTimeFormatInHelp, ShowSubcommandsInHelpOverride]
        [Description("Creates a reminder.")]
        public Task<ActionResult> BaseAsync(
            [Description("The time, from now, to set the reminder for.")]
            TimeSpan timeFromNow,
            [Description("What you wanted to be reminded of."), Remainder]
            string reminder)
        {
            var end = Context.Now.Add(timeFromNow);
            Db.CreateReminder(Reminder.CreateFrom(Context, end, reminder));
            return Ok(
                $"I'll remind you {end.FormatBoldString()} ({end.Humanize(dateToCompareAgainst: Context.Now)}).");
        }

        [Command("List", "Ls")]
        [Description("Lists all of your reminders.")]
        public Task<ActionResult> ListAsync(
            [Description(
                "Whether or not to only include reminders made in this current guild; or all of your reminders bot-wide.")]
            bool onlyCurrentGuild = true)
        {
            var pages = Db.GetReminders(Context.User.Id, onlyCurrentGuild ? Context.Guild.Id : 0)
                .Select(x => Context.CreateEmbedBuilder()
                    .WithTitle(x.TargetTime.Humanize(false, Context.Now))
                    .AddField("Internal ID", x.Id)
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

        [Command("Delete")]
        [Description("Deletes a reminder by its internal ID.")]
        public Task<ActionResult> DeleteAsync(
            [Description(
                "Internal Volte ID for the reminder you want to delete. You can find the ID via the Reminder list command.")]
            long internalId)
        {
            var reminder = Db.GetAllReminders()
                .FirstOrDefault(x => x.Id == internalId && x.CreatorId == Context.User.Id);
            return reminder != null && Db.TryDeleteReminder(reminder)
                ? Ok($"Deleted reminder #{internalId}; {Format.Code(reminder.ReminderText)}.")
                : BadRequest("Reminder couldn't be deleted.");
        }
    }
}