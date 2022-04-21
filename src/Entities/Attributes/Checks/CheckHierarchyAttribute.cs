﻿using System;
using System.Threading.Tasks;
using Discord.WebSocket;
using Gommon;
using Qmmands;
using Volte.Commands;
using Volte.Helpers;

namespace Volte.Entities
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class CheckHierarchyAttribute : ParameterCheckAttribute
    {
        public override ValueTask<CheckResult> CheckAsync(object argument, CommandContext context)
        {
            var u = argument.Cast<SocketGuildUser>() ?? throw new ArgumentException(
                $"Cannot use the CheckHierarchy attribute on a type that isn't {typeof(SocketGuildUser)}.");
            var ctx = context.Cast<VolteContext>();

            return ctx.IsAdmin(u)
                ? CheckResult.Failed("Cannot ban someone with the configured Admin role.")
                : ctx.User.Hierarchy > u.Hierarchy
                    ? CheckResult.Successful
                    : CheckResult.Failed("Cannot ban someone in a higher, or equal, hierarchy position than yourself.");
        }
    }
}