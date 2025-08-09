using JetBrains.Annotations;
using Volte.Systems.Database;
using Volte.Systems.Database.EntitiesV2;

namespace Volte.Systems.Moderation;

public interface IAuditLogContext
{
    public SocketTextChannel Channel { get; }
    
    public AuditLogArchiveSettings GuildSettings { get; }
    
    public SocketAuditLogEntry Entry { get; }
    public SocketGuild Guild { get; }
    
    public AuditLogArchiveMessageBuilder Message { get; }

    [ItemCanBeNull]
    public async Task<IUserMessage> Process(Func<Task> handler)
    {
        if (Channel is null || !GuildSettings.IsEnabled(Entry.Action))
            return null;

        var handlerTask = handler();
        await handlerTask
            .ContinueWith(task =>
            {
                if (task.Exception != null)
                    throw task.Exception.GetBaseException();
            });


        return await Message.SendAsync();
    }

    public static IAuditLogContext Create(ConstructorInfo ctor, AuditLogCreatedEventArgs args)
    {
        if (ctor.DeclaringType?.IsAssignableFrom(typeof(IAuditLogContext)) ?? false)
            throw new InvalidOperationException($"provided constructor was not for an object implementing {nameof(IAuditLogContext)}");

        return  ctor.Invoke([args, VolteBot.Services.Get<DatabaseService>()]).HardCast<IAuditLogContext>();
    }
}

public class AuditLogContext<TAuditLogData> : IAuditLogContext where TAuditLogData : class, ISocketAuditLogData
{
    public TAuditLogData Data { get; }
    public SocketTextChannel Channel { get; }
    
    public AuditLogArchiveSettings GuildSettings { get; }

    private readonly AuditLogCreatedEventArgs _eventArgs;
    public SocketAuditLogEntry Entry => _eventArgs.Entry;
    public SocketGuild Guild => _eventArgs.Guild;

    private readonly Action<AuditLogContext<TAuditLogData>> _save;

    public void SaveGuildSettings() => _save(this);
    
    public AuditLogArchiveMessageBuilder Message { get; }
        
    public AuditLogContext(AuditLogCreatedEventArgs args, DatabaseService db)
    {
        _eventArgs = args;
        _save = ctx => db.Modify(ctx.Guild.Id, it => it.Settings.AuditLog = ctx.GuildSettings);

        GuildSettings = db[args.Guild.Id].Settings.AuditLog;
        Data = args.Entry.Data.Cast<TAuditLogData>() ?? throw new InvalidOperationException();
        Channel = args.Guild.GetTextChannel(GuildSettings.Channel);
        Message = new AuditLogArchiveMessageBuilder(Entry.User, Channel);
    }
}