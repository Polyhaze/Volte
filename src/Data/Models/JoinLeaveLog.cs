namespace Volte.Data.Models
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