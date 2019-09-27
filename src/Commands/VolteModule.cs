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
        public EmojiService EmojiService { get; set; }
        public LoggingService Logger { get; set; }


        protected ActionResult Ok(string text, Func<IUserMessage, Task> afterCompletion = null,
            bool shouldEmbed = true) 
            => new OkResult(text, shouldEmbed, null, afterCompletion);

        protected ActionResult Ok(Func<Task> logic, bool awaitLogic = true) 
            => new OkResult(logic, awaitLogic);


        protected ActionResult Ok(EmbedBuilder embed, Func<IUserMessage, Task> afterCompletion = null) 
            => new OkResult(null, true, embed, afterCompletion);

        protected ActionResult Ok(string text) 
            => new OkResult(text);

        protected ActionResult Ok(EmbedBuilder embed) 
            => new OkResult(null, true, embed);

        protected ActionResult BadRequest(string reason) 
            => new BadRequestResult(reason);

        protected ActionResult None(Func<Task> afterCompletion = null, bool awaitCallback = true) 
            => new NoResult(afterCompletion, awaitCallback);
    }
}