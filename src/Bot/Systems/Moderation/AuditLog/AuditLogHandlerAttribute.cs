using Volte.Systems.Database;

namespace Volte.Systems.Moderation;

[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public sealed class AuditLogHandlerAttribute<TAuditLogData> : AuditLogHandlerAttribute where TAuditLogData : class, ISocketAuditLogData
{
    public AuditLogHandlerAttribute(ActionType actionType) : base(actionType, typeof(TAuditLogData))
    {
    }
}

public abstract class AuditLogHandlerAttribute : Attribute
{
    private static readonly SafeDictionary<ActionType, ConstructorInfo> ActionsToContextCtor = new();

    public ActionType Action { get; }
    public Type DataType { get; }

    protected AuditLogHandlerAttribute(ActionType action, Type dataType)
    {
        Action = action;
        DataType = dataType;
    }

    public ConstructorInfo FindConstructor()
    {
        if (ActionsToContextCtor[Action] is not { } contextCtor)
        {
            var contextType = AuditLogHandlers.ContextType.MakeGenericType(DataType);

            contextCtor = ActionsToContextCtor[Action]
                = contextType.GetConstructor([typeof(AuditLogCreatedEventArgs), typeof(DatabaseService)]) 
                  ?? throw new InvalidOperationException($"AuditLogContext constructor taking ({nameof(AuditLogCreatedEventArgs)}, {nameof(DatabaseService)}) not found");
        }

        return contextCtor;
    }
    
    public IAuditLogContext CreateContext(AuditLogCreatedEventArgs args) 
        => IAuditLogContext.Create(FindConstructor(), args);
}