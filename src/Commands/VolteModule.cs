using System;
using System.Threading.Tasks;
using Discord;
using Qmmands;
using Volte.Commands.Results;
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


        protected VolteCommandResult Ok(string text, Func<IUserMessage, Task> afterCompletion = null,
            bool shouldEmbed = true)
        {
            return new OkResult(text, shouldEmbed, null, afterCompletion);
        }

        protected VolteCommandResult Ok(Func<Task> logic)
        {
            return new OkResult(logic);
        }

        protected VolteCommandResult Ok(EmbedBuilder embed, Func<IUserMessage, Task> afterCompletion = null)
        {
            return new OkResult(null, true, embed, afterCompletion);
        }

        protected VolteCommandResult Ok(string text)
        {
            return new OkResult(text);
        }

        protected VolteCommandResult Ok(EmbedBuilder embed)
        {
            return new OkResult(null, true, embed);
        }

        protected VolteCommandResult BadRequest(string reason)
        {
            return new BadRequestResult(reason);
        }

        protected VolteCommandResult None()
        {
            return new NoResult();
        }
    }
}