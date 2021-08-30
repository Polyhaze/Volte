using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Gommon;
using Qmmands;
using Volte.Entities;
using Volte.Helpers;

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
                $"I'll remind you {end.GetDiscordTimestamp(TimestampType.LongDateTime)} ({end.GetDiscordTimestamp(TimestampType.Relative)}).");
        }

        [Command("List", "Ls")]
        [Description("Lists all of your reminders.")]
        public Task<ActionResult> ListAsync(
            [Description(
                "Whether or not to only include reminders made in this current guild; or all of your reminders bot-wide.")]
            bool onlyCurrentGuild = true)
        {
            var pages = Db.GetReminders(Context.User.Id)
                .Select(x => Context.CreateEmbedBuilder()
                    .WithTitle(x.TargetTime.GetDiscordTimestamp(TimestampType.Relative))
                    .AddField("Unique ID", x.Id)
                    .AddField("Reminder", Format.Code(x.ReminderText))
                    .AddField("Created", x.CreationTime.GetDiscordTimestamp(TimestampType.LongDateTime)))
                .ToList();
            if (pages.IsEmpty())
                return Ok(
                    $"You currently have no reminders set{(onlyCurrentGuild ? " in this guild" : string.Empty)}.");
            if (pages.Count is 1) return Ok(pages.First());
            return Ok(pages);
        }

        [Command("Delete")]
        [Description("Deletes a reminder by its internal ID.")]
        [Remarks("You may only delete reminders you made. Obviously.")]
        public Task<ActionResult> DeleteAsync(
            [Description(
                "Reminder's Unique ID for the reminder you want to delete. You can find the ID via the Reminder list command.")]
            long uniqueId)
        {
            var reminder = Db.AllReminders.FirstOrDefault(x => x.Id == uniqueId && x.CreatorId == Context.User.Id);
            return reminder != null && Db.TryDeleteReminder(reminder)
                ? Ok($"Deleted reminder #{uniqueId}; {Format.Code(reminder.ReminderText)}.")
                : BadRequest("Reminder couldn't be deleted.");
        }
    }
}