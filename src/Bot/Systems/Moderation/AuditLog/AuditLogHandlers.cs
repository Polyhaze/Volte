namespace Volte.Systems.Moderation;

public static partial class AuditLogHandlers
{
    public static readonly Type ContextType;

    public static readonly Dictionary<ActionType, Func<AuditLogCreatedEventArgs, Task>> Delegates = new();

    static AuditLogHandlers()
    {
        ContextType = typeof(AuditLogHandlers).Assembly.GetExportedTypes()
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

        var sw = Stopwatch.StartNew();

        typeof(AuditLogHandlers).GetMethods(BindingFlags.Static | BindingFlags.NonPublic)
            .Select(x => (Method: x, Attr: x.GetCustomAttribute<AuditLogHandlerAttribute>()))
            .ForEach(Map);

        sw.Stop();

        Info(LogSource.Service, $"Loaded {Delegates.Count} audit log handlers in {sw.ElapsedMilliseconds}ms.");
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
            throw err("Invalid context argument generic type argument");

        Delegates[arg.Attr.Action] = args => arg.Attr.CreateContext(args).Process(arg.Method, ExecuteHandler);

        return;

        InvalidOperationException err(string message) => new(
            $"{message} for found handler {arg.Method.Name}; " +
            $"Expected signature of '{arg.Method.FormatSignatureString(ContextType.MakeGenericType(arg.Attr.DataType))}'; " +
            $"got '{arg.Method.FormatSignatureString()}'"
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