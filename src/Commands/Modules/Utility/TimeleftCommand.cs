using System;
using System.Text;
using System.Linq;
using System.Globalization;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

using Humanizer;
using Humanizer.Localisation;

namespace BrackeysBot.Commands
{
    public partial class UtilityModule : BrackeysBotModule
    {
        private const string DateTimeOutputFormat = @"dd/MM/yyyy HH:mm:ss \U\T\C";
        private const string DateTimeInputFormat = @"dd/MM/yyyy-HH:mm:ss";

        [Command("timeleft"), Alias("gamejamtime")]
        [Summary("Displays when the jam starts or ends.")]
        public async Task ShowGamejamTimeLeftAsync()
        {
            EmbedBuilder reply = GetDefaultBuilder();

            if (Data.Configuration.GamejamTimestamps == null || Data.Configuration.GamejamTimestamps.Length == 0)
            {
                reply.WithDescription("No jams are currently scheduled!");
            }
            else
            {
                DateTimeOffset[] times = Data.Configuration.GamejamTimestamps.Select(t => DateTimeOffset.FromUnixTimeSeconds(t)).ToArray();
                DateTimeOffset now = DateTimeOffset.UtcNow;

                bool CheckTime(DateTimeOffset time, string message)
                {
                    if (now < time)
                    {
                        TimeSpan diff = time - now;
                        reply.WithDescription(string.Format(message, diff.Humanize(7, minUnit: TimeUnit.Second)));

                        return true;
                    }
                    return false;
                }

                if (!CheckTime(times[0], "The jam will start in {0}."))
                    if (!CheckTime(times[1], "The jam has started! It will end in {0}."))
                        if (!CheckTime(times[2], "The jam has ended! The voting period will end in {0}."))
                            reply.WithDescription("No jams are currently scheduled!");
            }

            await reply.Build().SendToChannel(Context.Channel);
        }

        [Command("gamejaminfo")]
        [Summary("Displays time information about an upcoming or ongoing jam!")]
        public async Task ShowGamejamInfoAsync()
        {
            await GetDefaultBuilder()
                .WithTitle("Game Jam Information")
                .WithDescription(GetJamConfigurationResponse())
                .Build()
                .SendToChannel(Context.Channel);
        }

        [Command("settimeleft"), Alias("setgamejamtime")]
        [Summary("Sets the time configuration for a jam, with a format of '" + DateTimeInputFormat + "'.")]
        [Remarks("settimeleft <start-date> <end-date> <voting-end>")]
        [RequireModerator]
        public async Task SetGamejamTimesAsync([Summary("The dates to outline the jam times.")] params string[] dates)
        {
            if (dates.Length != 3)
                throw new ArgumentException($"Invalid time configuration. (Got {dates.Length}, expected 3)");

            Data.Configuration.GamejamTimestamps = dates.Select(t =>
            {
                if (!DateTimeOffset.TryParseExact(t, DateTimeInputFormat, CultureInfo.InvariantCulture.DateTimeFormat, DateTimeStyles.AssumeUniversal, out DateTimeOffset result))
                    throw new ArgumentException($"The date _{t}_ does not meet the required format of _{DateTimeInputFormat}.");
                return result.ToUnixTimeSeconds();
            }).ToArray();
            Data.SaveConfiguration();

            var configDates = Data.Configuration.GamejamTimestamps.Select(l => DateTimeOffset.FromUnixTimeSeconds(l)).ToArray();
            await GetDefaultBuilder()
                .WithTitle("Jam times set!")
                .WithDescription(GetJamConfigurationResponse())
                .Build()
                .SendToChannel(Context.Channel);
        }

        private string GetJamConfigurationResponse()
        {
            // Display that no jam is scheduled if there are no timestamps or the last timestamp is behind the current time
            if (Data.Configuration.GamejamTimestamps?.Length == 0
                || Data.Configuration.GamejamTimestamps.Last() < DateTimeOffset.UtcNow.ToUnixTimeSeconds())
                return "No jam is currently scheduled!";

            // Convert the unix timestamps into DateTimeOffsets, and nicely format them
            var configDates = Data.Configuration.GamejamTimestamps.Select(l => DateTimeOffset.FromUnixTimeSeconds(l)).ToArray();
            return new StringBuilder()
                .AppendLine($"The current configuration is that the jam:")
                .AppendLine($"... begins on {configDates[0].ToString(DateTimeOutputFormat)}")
                .AppendLine($"... ends on {configDates[1].ToString(DateTimeOutputFormat)}.")
                .AppendLine($"... has the voting closed at {configDates[2].ToString(DateTimeOutputFormat)}.")
                .ToString();
        }
    }
}
