using System;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using Qmmands;
using Volte.Commands.Results;
using Volte.Core.Models.Guild;
using Volte.Services;

namespace Volte.Commands
{
    public abstract class VolteModule : ModuleBase<VolteContext>
    {
        public DatabaseService Db { get; set; }
        public EventService EventService { get; set; }
        public ModLogService ModLogService { get; set; }
        public CommandService CommandService { get; set; }
        public LoggingService Logger { get; set; }
        public CancellationTokenSource Cts { get; set; }
        public new VolteContext Context => base.Context;

        public void ModifyData(Func<GuildData, GuildData> func)
        {
            Db.ModifyAndSaveData(Context.Guild.Id, func);
        }

        public Task ModifyDataAsync(Func<GuildData, Task<GuildData>> func)
        {
            return Db.ModifyAndSaveDataAsync(Context.Guild.Id, func);
        }


        protected ActionResult Ok(
            string text, 
            Func<DiscordMessage, Task> callback = null,
            bool shouldEmbed = true, bool awaitCallback = true) 
            => new OkResult(text, shouldEmbed, null, callback, awaitCallback);

        protected ActionResult Ok(
            Func<Task> logic, 
            bool awaitLogic = true) 
            => new OkResult(logic, awaitLogic);


        protected ActionResult Ok(
            DiscordEmbedBuilder embed, 
            Func<DiscordMessage, Task> callback = null, bool awaitCallback = true) 
            => new OkResult(null, true, embed, callback);

        protected ActionResult Ok(string text) 
            => new OkResult(text);

        protected ActionResult Ok(DiscordEmbedBuilder embed) 
            => new OkResult(null, true, embed);

        protected ActionResult BadRequest(string reason) 
            => new BadRequestResult(reason);

        protected ActionResult None(
            Func<Task> afterCompletion = null, 
            bool awaitCallback = true) 
            => new NoResult(afterCompletion, awaitCallback);
    }
}