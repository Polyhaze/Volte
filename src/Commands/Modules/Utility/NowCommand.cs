using System;
using System.Threading.Tasks;
using Gommon;
using Qmmands;
using Volte.Extensions;

namespace Volte.Commands.Modules.Utility
{
    public partial class UtilityModule : VolteModule
    {
        [Command("Now")]
        [Description("Shows the current date and time, in UTC.")]
        [Remarks("Usage: |prefix|now")]
        public async Task NowAsync()
        {
            await Context.CreateEmbed($"**Date**: {DateTimeOffset.UtcNow.FormatDate()}\n" +
                                      $"**Time**: {DateTimeOffset.UtcNow.FormatFullTime()}")
                .SendToAsync(Context.Channel);
        }
    }
}