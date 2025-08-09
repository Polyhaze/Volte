using Volte.Systems.Database;

namespace Volte.Systems.Moderation;

public static partial class AuditLogHandlers
{
    public static Type AuditLogContextType { get; }

    public static readonly Dictionary<ActionType, Func<AuditLogCreatedEventArgs, Task>> Delegates = new();

    static AuditLogHandlers()
    {
        AuditLogContextType = typeof(AuditLogHandlers).Assembly.GetExportedTypes()
            .FindFirst(t => t.Inherits<IAuditLogContext>() && !t.IsAbstract)
            .OrThrow(() => new InvalidOperationException("context type not found"));
    }

    public static Task HandleAsync(AuditLogCreatedEventArgs args)
    {
        if (!Delegates.TryGetValue(args.Entry.Action, out var func))
        {
            Debug(LogSource.Service, $"AuditLogHandlers missing definition for: {args.Entry.Action}");
            return Task.CompletedTask;
        }

        return func(args);
    }

    public static void Initialize()
    {
        Delegates.Clear();

        typeof(AuditLogHandlers).GetMethods(BindingFlags.Static | BindingFlags.NonPublic)
            .Select(x => x.GetCustomAttribute<AuditLogHandlerAttribute>()?.WithMethod(x))
            .Where(x => x != null)
            .ForEach(Map);
        
        Info(LogSource.Service, $"Loaded {Delegates.Count} audit log handlers.");
    }
    
    private static void Map(AuditLogHandlerAttribute attr) => Delegates[attr.Action] = attr.Wrap(attr.Method);
}

[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public sealed class AuditLogHandlerAttribute<TAuditLogData> : AuditLogHandlerAttribute where TAuditLogData : class, ISocketAuditLogData
{
    public AuditLogHandlerAttribute(ActionType actionType) : base(actionType, typeof(TAuditLogData))
    {
    }
}

[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public class AuditLogHandlerAttribute : Attribute
{
    private static readonly SafeDictionary<ActionType, ConstructorInfo> ActionsToContextCtor = new();
    
    public ActionType Action { get; }
    public Type DataType { get; }

    public AuditLogHandlerAttribute(ActionType action, Type dataType)
    {
        Action = action;
        DataType = dataType;
    }

    public MethodInfo Method { get; private set; }

    public AuditLogHandlerAttribute WithMethod(MethodInfo method)
    {
        Method = method;
        return this;
    }

    public ConstructorInfo GetOrAdd(ActionType action)
    {
        if (ActionsToContextCtor[action] is not { } contextCtor)
        {
            var contextType = AuditLogHandlers.AuditLogContextType.MakeGenericType(DataType);

            contextCtor = ActionsToContextCtor[action] 
                = contextType.GetConstructor([typeof(AuditLogCreatedEventArgs), typeof(DatabaseService)]) 
                  ?? throw new InvalidOperationException($"AuditLogContext constructor taking ({nameof(AuditLogCreatedEventArgs)}, {nameof(DatabaseService)}) not found");
        }

        return contextCtor;
    }

    public Func<AuditLogCreatedEventArgs, Task> Wrap(MethodInfo mi) => 
        args =>
        {
            var context = GetOrAdd(args.Entry.Action).Invoke([args, VolteBot.Services.Get<DatabaseService>()]).HardCast<IAuditLogContext>();

            return context.Process(() =>
            {
                try
                {
                    if (mi.ReturnType == typeof(void))
                    {
                        mi.Invoke(null, [context]);
                        return Task.CompletedTask;
                    }

                    var returned = mi.Invoke(null, [context]);

                    switch (returned)
                    {
                        case Task task:
                            return task;
                        default:
                            return Task.FromResult(returned);
                    }
                }
                catch (Exception e)
                {
                    return Task.FromException(e);
                }
            });
        };
}