using Discord;
using Qmmands;
using Volte.Data;
using Volte.Data.Models.Results;
using Volte.Services;

namespace Volte.Commands
{
    public abstract class VolteModule : ModuleBase<VolteContext>
    {
        public DatabaseService Db { get; set; }
        public EventService EventService { get; set; }
        public ModLogService ModLogService { get; set; }
        public CommandService CommandService { get; set; }
        public BinService BinService { get; set; }
        public EmojiService EmojiService { get; set; }
        public LoggingService Logger { get; set; }

        protected BaseResult Ok(string embedContent)
        {
            return new OkResult(Context.CreateEmbedBuilder(embedContent));
        }

        protected BaseResult Ok(EmbedBuilder embed)
        {
            return new OkResult(embed);
        }

        protected BaseResult BadRequest(string reason)
        {
            return new BadRequestResult(reason);
        }

        protected BaseResult None()
        {
            return new NoResult();
        }
    }
}