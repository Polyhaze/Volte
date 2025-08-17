using JetBrains.Annotations;
using Volte.Systems.Database;
using Volte.Systems.Database.EntitiesV2;

namespace Volte.Systems.Moderation;

public interface IAuditLogContext
{
    
    public static readonly Type ImplementationType;

    static IAuditLogContext()
    {
        ImplementationType = typeof(AuditLogHandlers).Assembly.GetExportedTypes()
            .FindFirst(t => t.Inherits<IAuditLogContext>() && !t.IsAbstract)
            .OrThrow(() => new InvalidOperationException("context type not found"));
    }
    
    public SocketTextChannel Channel { get; }

    public AuditLogArchiveSettings GuildSettings { get; }

    public SocketAuditLogEntry Entry { get; }
    public SocketGuild Guild { get; }

    public AuditLogArchiveMessageBuilder Message { get; }

    [ItemCanBeNull]
    public async Task<IUserMessage> Process(MethodInfo methodInfo, Func<MethodInfo, IAuditLogContext, Task> handler)
    {
        if (Channel is null || !GuildSettings.IsEnabled(Entry.Action))
        {
            Debug(LogSource.Service, $"Audit log handler does not have a channel to post to or is of a disabled action type ({Entry.Action}); aborting.");
            return null;
        }

        var sw = Stopwatch.StartNew();

        var handlerTask = handler(methodInfo, this);
        await handlerTask.ContinueWith(task =>
        {
            if (task.IsFaulted)
                throw task.Exception.GetBaseException();
        });

        sw.Stop();

        if (!Message.IsModified)
        {
            Debug(LogSource.Service, 
                $"Audit log handler for type {Entry.Action} (method name: {methodInfo.Name}, in type: {methodInfo.DeclaringType?.AsPrettyString() ?? "null"}) did not configure the archive embed.");
            return null;
        }

        Debug(LogSource.Service, $"Audit log handler for type {Entry.Action} executed in {sw.ElapsedMilliseconds}ms.");

        return await Message.SendAsync();
    }

    public static IAuditLogContext Create(ConstructorInfo ctor, AuditLogCreatedEventArgs args)
    {
        if (ctor.DeclaringType?.IsAssignableFrom(typeof(IAuditLogContext)) ?? false)
            throw new InvalidOperationException($"provided constructor was not for an object implementing {nameof(IAuditLogContext)}");

        return ctor.Invoke([args, VolteBot.Services.Get<DatabaseService>()]).HardCast<IAuditLogContext>();
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