using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Gommon;
using Qmmands;
using Volte.Commands;
using Volte.Core.Entities;
using Volte.Interactive;
using Volte.Services;

namespace Volte.Commands
{
    public abstract class VolteModule : ModuleBase<VolteContext>
    {
        public DatabaseService Db { get; set; }
        public ModerationService ModerationService { get; set; }
        public CommandService CommandService { get; set; }
        
        protected ActionResult Ok(
            string text, 
            MessageCallback afterCompletion = null,
            bool shouldEmbed = true) 
            => new OkResult(text, shouldEmbed, null, afterCompletion);

        protected ActionResult Ok(
            AsyncFunction logic, 
            bool awaitLogic = true) 
            => new OkResult(logic, awaitLogic);

        protected ActionResult Ok(Action<StringBuilder> textBuilder, MessageCallback messageCallback = null,
            bool shouldEmbed = true)
            => Ok(new StringBuilder().Apply(textBuilder), messageCallback, shouldEmbed);

        protected ActionResult Ok(StringBuilder text, MessageCallback messageCallback = null, bool shouldEmbed = true)
            => Ok(text.ToString(), messageCallback, shouldEmbed);
        protected ActionResult Ok(PaginatedMessage.Builder pager) => new OkResult(pager);
        protected ActionResult Ok(IEnumerable<EmbedBuilder> embeds) => new OkResult(embeds);

        protected ActionResult Ok(PollInfo pollInfo) => new OkResult(pollInfo);

        protected ActionResult Ok(
            EmbedBuilder embed, 
            MessageCallback afterCompletion = null) 
            => new OkResult(null, true, embed, afterCompletion);

        protected ActionResult Ok(string text) 
            => new OkResult(text);

        protected ActionResult Ok(EmbedBuilder embed) 
            => new OkResult(null, true, embed);

        protected ActionResult BadRequest(string reason) 
            => new BadRequestResult(reason);

        protected ActionResult None(
            AsyncFunction afterCompletion = null, 
            bool awaitCallback = true) 
            => new NoResult(afterCompletion, awaitCallback);
    }
}