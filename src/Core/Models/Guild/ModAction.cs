using System;

namespace Volte.Core.Models.Guild
{
    public class ModAction
    {
        public ModActionType Type { get; set; }
        public ulong Moderator { get; set; }
        public string Reason { get; set; }
        public DateTimeOffset Time { get; set; }
    }
}