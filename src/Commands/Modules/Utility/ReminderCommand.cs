using System;
using System.Threading.Tasks;
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
            [Description("The time, from now, to set the reminder for.")] TimeSpan time, 
            [Description("What you wanted to be reminded of."), Remainder] string reminder)
        {
            var end = Context.Now.Add(time);
            Db.CreateReminder(Reminder.FromContext(Context, end, reminder));
            return Ok($"I'll remind you in {end.Humanize(dateToCompareAgainst: Context.Now)}.");
        }
    }
}