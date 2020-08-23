using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using Gommon;
using Extensions = Gommon.Extensions;

namespace Volte.Commands.Results
{
    public class OkResult : ActionResult
    {
        public OkResult(string text, bool shouldEmbed = true, DiscordEmbedBuilder embed = null,
            Func<DiscordMessage, Task> func = null, bool awaitCallback = true)
        {
            _message = text;
            _shouldEmbed = shouldEmbed;
            _embed = embed;
            _callback = func;
            _runFuncAsync = awaitCallback;
        }

        public OkResult(Func<Task> logic, bool awaitFunc = true)
        {
            _separateLogic = logic;
            _runFuncAsync = awaitFunc;
        }

        private readonly bool _runFuncAsync;

        private readonly string _message;
        private readonly bool _shouldEmbed;
        private readonly Func<DiscordMessage, Task> _callback;
        private readonly Func<Task> _separateLogic;
        private readonly DiscordEmbedBuilder _embed;

        public override async ValueTask<ResultCompletionData> ExecuteResultAsync(VolteContext ctx)
        {
            if (!ctx.Guild.CurrentMember.PermissionsIn(ctx.Channel).HasPermission(Permissions.SendMessages)) return new ResultCompletionData();

            if (_separateLogic is not null)
            {
                if (_runFuncAsync)
                    await Executor.ExecuteAsync(_separateLogic);
                else
                    Executor.Execute(() => _separateLogic());

                return new ResultCompletionData();
            }

            DiscordMessage message;
            if (_embed is null)
            {
                if (_shouldEmbed)
                    message = await ctx.CreateEmbed(_message).SendToAsync(ctx.Channel);
                else
                    message = await ctx.Channel.SendMessageAsync(_message);
            }
            else
                message = await _embed.SendToAsync(ctx.Channel);


            if (_callback is not null)
            {
                if (_runFuncAsync)
                    await Executor.ExecuteAsync(async () => await _callback(message));
                else
                    Executor.Execute(() => _callback(message));
            }


            return new ResultCompletionData(message);
        }
    }
}