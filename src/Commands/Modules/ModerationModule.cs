using System;
using System.Collections.Generic;
using System.Text;

using BrackeysBot.Services;

using Discord.Commands;

namespace BrackeysBot.Commands
{
    [Summary("Provides tools to moderate the server.")]
    [ModuleColor(0xd95d5d)]
    public partial class ModerationModule : BrackeysBotModule
    {
        public ModerationService Moderation { get; set; }

        public const string DefaultReason = "Unspecified";
    }
}
