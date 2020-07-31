using System.IO;
using System.Threading.Tasks;

using Discord.Commands;

using BrackeysBot.Services;

namespace BrackeysBot.Commands
{
    public partial class UtilityModule : BrackeysBotModule
    {
        public LoggingService Logging { get; set; }

        [Command("logfile"), Alias("log")]
        [Summary("Provides today's logfile.")]
        [RequireModerator]
        public async Task GetLogfileAsync()
        {
            string file = Logging.LogFile;
            if (!File.Exists(file))
                throw new FileNotFoundException();

            await Context.Channel.SendFileAsync(file);
        }
    }
}
