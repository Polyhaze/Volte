namespace Volte.Core.Data.Models.BotConfig
{
    public sealed class JoinLeaveLog
    {
        internal JoinLeaveLog()
        {
            Enabled = false;
            GuildId = ulong.MinValue;
            ChannelId = ulong.MinValue;
        }

        public bool Enabled { get; set; }
        public ulong GuildId { get; set; }
        public ulong ChannelId { get; set; }
    }
}