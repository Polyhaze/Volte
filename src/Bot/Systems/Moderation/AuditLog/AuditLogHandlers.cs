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
            .Select(x => (Method: x, Attr: x.GetCustomAttribute<AuditLogHandlerAttribute>()))
            .ForEach(Map);
        
        Info(LogSource.Service, $"Loaded {Delegates.Count} audit log handlers.");
    }

    private static void Map((MethodInfo Method, AuditLogHandlerAttribute Attr) arg)
    {
        if (arg.Attr is null) return;
        
        Delegates[arg.Attr.Action] = Wrap(arg.Attr, arg.Method);
    }
    
    private static Func<AuditLogCreatedEventArgs, Task> Wrap(AuditLogHandlerAttribute attribute, MethodInfo mi) => 
        args =>
        {
            var context = IAuditLogContext.Create(AuditLogHandlerAttribute.FindConstructorForDataType(attribute), args);

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

                    return returned switch
                    {
                        Task task => task,
                        _ => Task.FromResult(returned)
                    };
                }
                catch (Exception e)
                {
                    return Task.FromException(e);
                }
            });
        };
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

    public static ConstructorInfo FindConstructorForDataType(AuditLogHandlerAttribute attr)
    {
        if (ActionsToContextCtor[attr.Action] is not { } contextCtor)
        {
            var contextType = AuditLogHandlers.AuditLogContextType.MakeGenericType(attr.DataType);

            contextCtor = ActionsToContextCtor[attr.Action] 
                = contextType.GetConstructor([typeof(AuditLogCreatedEventArgs), typeof(DatabaseService)]) 
                  ?? throw new InvalidOperationException($"AuditLogContext constructor taking ({nameof(AuditLogCreatedEventArgs)}, {nameof(DatabaseService)}) not found");
        }

        return contextCtor;
    }
}