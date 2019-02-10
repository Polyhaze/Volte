namespace Volte.Core.Data.Objects {
    public class JoinLeaveLog {
        public bool Enabled { get; set; }
        public ulong GuildId { get; set; }
        public ulong ChannelId { get; set; }
        

        public JoinLeaveLog() {
            Enabled = false;
            GuildId = ulong.MinValue;
            ChannelId = ulong.MinValue;
        }
    }
}