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

        var handlerParams = arg.Method.GetParameters();

        // must take a single argument; 
        if (handlerParams.Length is not 1)
            throw err("Invalid argument count");

        var contextParam = handlerParams[0];

        // that argument must be an implementor of IAuditLogContext (aka, AuditLogContext<TAuditLogData>)
        if (!contextParam.ParameterType.IsAssignableTo(typeof(IAuditLogContext)))
            throw err("Invalid argument type");

        // the type argument to the AuditLogContext must be the same as passed to the attribute.
        if (contextParam.ParameterType.GenericTypeArguments[0] != arg.Attr.DataType)
            throw err("Invalid argument type generic type argument");

        Delegates[arg.Attr.Action] = Wrap(arg.Attr, arg.Method);

        return;

        static string formatExpectedSignature(MethodInfo method, Type argType)
        {
            var sb = new StringBuilder();
            sb.Append(method.ReturnType.AsPrettyString());
            sb.Append(' ');
            sb.Append(method.Name);
            sb.Append('(');
            sb.Append(argType.AsPrettyString());
            sb.Append(')');

            return sb.ToString();
        }

        static string formatEncounteredSignature(MethodInfo method, params ParameterInfo[] infos)
        {
            var sb = new StringBuilder();
            sb.Append(method.ReturnType.AsPrettyString());
            sb.Append(' ');
            sb.Append(method.Name);
            sb.Append('(');

            sb.Append(infos
                .Select(x => x.Name is null 
                    ? x.ParameterType.AsPrettyString() 
                    : $"{x.ParameterType.AsPrettyString()} {x.Name}")
                .JoinToString(", ")
            );
            
            sb.Append(')');

            return sb.ToString();
        }
        
        InvalidOperationException err(string message) => new(
            $"{message} for found handler {arg.Method.Name}; " +
            $"Expected signature of '{formatExpectedSignature(arg.Method, AuditLogContextType.MakeGenericType(arg.Attr.DataType))}'; " +
            $"got '{formatEncounteredSignature(arg.Method, handlerParams)}'"
        );
    }
    
    private static Func<AuditLogCreatedEventArgs, Task> Wrap(AuditLogHandlerAttribute attribute, MethodInfo mi) => args => 
        IAuditLogContext.Create(AuditLogHandlerAttribute.FindConstructorForDataType(attribute), args)
            .Process(mi, (method, ctx) =>
            {
                try
                {
                    if (method.ReturnType == typeof(void))
                    {
                        method.Invoke(null, [ctx]);
                        return Task.CompletedTask;
                    }

                    var returned = method.Invoke(null, [ctx]);

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