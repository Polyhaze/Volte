namespace Volte.Systems.Moderation;

public static partial class AuditLogHandlers
{
    private static readonly Dictionary<ActionType, Func<AuditLogCreatedEventArgs, Task>> Delegates = new();

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

        var sw = Stopwatch.StartNew();

        typeof(AuditLogHandlers)
            .GetMethods(BindingFlags.Static | BindingFlags.NonPublic)
            .ForEach(Map);

        sw.Stop();

        Info(LogSource.Service, $"Loaded {Delegates.Count} audit log handlers in {sw.ElapsedMilliseconds}ms.");
    }

    private static void Map(MethodInfo method)
    {
        if (method.GetCustomAttribute<AuditLogHandlerAttribute>() is not { } attr)
            return;

        var handlerParams = method.GetParameters();

        // must take a single argument; 
        if (handlerParams.Length is not 1)
            throw err("Invalid argument count");

        var contextParam = handlerParams[0];

        // that argument must not be the base IAuditLogContext interface;
        if (contextParam.ParameterType == typeof(IAuditLogContext))
            throw err($"Argument type cannot be the {nameof(IAuditLogContext)} interface");

        // that argument must be an implementor of IAuditLogContext (aka, AuditLogContext<TAuditLogData>);
        if (!contextParam.ParameterType.IsAssignableTo(typeof(IAuditLogContext)))
            throw err("Invalid argument type");

        // the type argument to the AuditLogContext must be the same as passed to the attribute.
        if (contextParam.ParameterType.GenericTypeArguments[0] != attr.DataType)
            throw err("Invalid context argument generic type argument");

        Delegates[attr.Action] = args => attr.CreateContext(args).Process(method, ExecuteHandler);

        return;

        InvalidOperationException err(string message) => new(
            $"{message} for found handler {method.Name}; " +
            $"Expected signature of '{method.FormatSignatureString(IAuditLogContext.ImplementationType.MakeGenericType(attr.DataType))}'; " +
            $"got '{method.FormatSignatureString()}'"
        );
    }

    private static Task ExecuteHandler(MethodInfo method, IAuditLogContext ctx)
    {
        try
        {
            Debug(LogSource.Service, $"Invoking {method.ReturnType.AsPrettyString()}-returning registered delegate for {ctx.Entry.Action}");

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
            Debug(LogSource.Service, $"Error of type {e.GetType().FullName} occurred in registered delegate for {ctx.Entry.Action}; returning faulted task to the caller");
            return Task.FromException(e);
        }
    }
}