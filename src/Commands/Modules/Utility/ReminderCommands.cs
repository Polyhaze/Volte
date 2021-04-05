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
            [Description("The time, from now, to set the reminder for.")] TimeSpan time, 
            [Description("What you wanted to be reminded of."), Remainder] string reminder)
        {
            var end = Context.Now.Add(time);
            Db.CreateReminder(Reminder.FromContext(Context, end, reminder));
            return Ok($"I'll remind you {end.FormatPrettyString()} ({end.Humanize(dateToCompareAgainst: Context.Now)}).");
        }

        [Command("Reminds", "Reminders")]
        public Task<ActionResult> RemindersAsync()
        {
            var pages = Db.GetAllReminders().Where(x => x.CreatorId == Context.User.Id && x.GuildId == Context.Guild.Id)
                .Select(x => Context.CreateEmbedBuilder()
                    .WithTitle(x.TargetTime.Humanize(dateToCompareAgainst: Context.Now))
                    .AddField("Reminder", Format.Code(x.Value))
                    .AddField("Created at", x.CreationTime.FormatPrettyString())
                    .AddField("Channel", Context.Guild.GetTextChannel(x.ChannelId).Mention))
                .ToList();
            if (pages.IsEmpty()) return Ok("You currently have no reminders set in this guild.");
            if (pages.Count is 1) return Ok(pages.First());
            return Ok(pages);
        }
    }
}