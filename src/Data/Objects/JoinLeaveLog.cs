namespace Volte.Data.Objects
{
    public sealed class JoinLeaveLog
    {
        public JoinLeaveLog()
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