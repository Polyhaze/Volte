namespace Volte.Entities;

public sealed class UserLeftEventArgs : EventArgs
{
    public SocketUser User { get; }
    public SocketGuild Guild { get; }

    public UserLeftEventArgs(SocketGuild guild, SocketUser user)
    {
        User = user;
        Guild = guild;
    }
}