using System.Collections;

// ReSharper disable MemberCanBePrivate.Global

namespace Volte.Entities;

public sealed class EvalEnvironment
{
    internal EvalEnvironment()
    {
        Environment = this;
    }

    public required VolteContext Context { get; init; }
    public required DiscordSocketClient Client { get; init; }
    public required GuildData Data { get; init; }
    public required CommandService Commands { get; init; }
    public required DatabaseService Database { get; init; }
    public EvalEnvironment Environment { get; }

    public SocketGuildUser Member(ulong id) => Context.Guild.GetUser(id);
    public SocketUser User(ulong id) => Context.Client.GetUser(id);

    public SocketGuildUser Member(string username) => Context.Guild.Users.FirstOrDefault(a =>
        a.Username.EqualsIgnoreCase(username) || (a.Nickname != null && a.Nickname.EqualsIgnoreCase(username)));

    public SocketTextChannel TextChannel(ulong id) => Context.Client.GetChannel(id).Cast<SocketTextChannel>();
    public SocketVoiceChannel VoiceChannel(ulong id) => Context.Client.GetChannel(id).Cast<SocketVoiceChannel>();
    public SocketNewsChannel NewsChannel(ulong id) => Context.Client.GetChannel(id).Cast<SocketNewsChannel>();
    public SocketDMChannel DmChannel(ulong id) => Context.Client.GetDMChannelAsync(id).Cast<SocketDMChannel>();
    public SocketRole Role(ulong id) => Context.Guild.GetRole(id);
    public SocketSelfUser SelfUser() => Context.Client.CurrentUser;

    public SocketSystemMessage SystemMessage(ulong id) =>
        Context.Channel.CachedMessages.TryGetFirst(m => m.Id == id, out var message) && message is SocketSystemMessage
            ? message.Cast<SocketSystemMessage>()
            : throw new InvalidOperationException(
                $"The ID provided didn't lead to a valid system message, it lead to a(n) {message?.Source} message.");

    public SocketUserMessage Message(ulong id) =>
        Context.Channel.CachedMessages.TryGetFirst(m => m.Id == id, out var message) && message is SocketUserMessage
            ? message.Cast<SocketUserMessage>()
            : throw new InvalidOperationException(
                $"The ID provided didn't lead to a valid user-created message, it lead to a(n) {message?.Source} message.");

    public async ValueTask<IUserMessage> MessageAsync(ulong id)
    {
        var m = await Context.Channel.GetMessageAsync(id);

        return m as IUserMessage ?? throw new InvalidOperationException(
            $"The ID provided didn't lead to a valid user-created message, it lead to a(n) {m.Source} message.");
    }

    public SocketGuild Guild(ulong id) => Context.Client.GetGuild(id);
    public T Service<T>() where T : notnull => Context.Services.GetRequiredService<T>();

    public SocketUserMessage Message(string id)
        => id.TryParse<ulong>(out var ulongId)
            ? Message(ulongId)
            : throw new ArgumentException(
                $"Method parameter {nameof(id)} is not a valid {typeof(ulong).FullName}.");

    public async ValueTask<IUserMessage> ReplyAsync(string content) =>
        await Context.Channel.SendMessageAsync(content);

    public async ValueTask<IUserMessage> ReplyAsync(Embed embed) => await embed.SendToAsync(Context.Channel);

    public async ValueTask<IUserMessage> ReplyAsync(EmbedBuilder embed) => await embed.SendToAsync(Context.Channel);

    public Task ReactAsync(string unicode) => Context.Message.AddReactionAsync(new Emoji(unicode));
    public Task ReactAsync(Emoji emoji) => Context.Message.AddReactionAsync(emoji);

    public string Inheritance<T>() => Inheritance(typeof(T));
    public string Inheritance(object obj) => Inheritance(obj.GetType());

    public static string Inheritance(Type type)
    {
        var baseTypes = new List<Type> {type};
        var latestType = type.BaseType;

        while (latestType != null)
        {
            baseTypes.Add(latestType);
            latestType = latestType.BaseType;
        }

        return String(sb =>
        {
            sb.AppendLine($"Inheritance tree for type [{type.FullName}]").AppendLine();

            foreach (var baseType in baseTypes)
            {
                var inheritors = baseType.GetInterfaces().ToList();
                if (baseType.BaseType != null)
                    inheritors = inheritors.Prepend(baseType.BaseType).ToList();
                
                sb.Append($"[{baseType.AsPrettyString()}]");

                if (inheritors.Count > 0)
                    sb.Append($": {inheritors.Select(x => x.AsPrettyString()).JoinToString(", ")}");

                sb.AppendLine();
            }
        });
    }

    public string Inspect(object obj, bool discordSizeCap = true)
    {
        var type = obj.GetType();

        var inspection = new StringBuilder();
        inspection.Append("<< Inspecting type [").Append(type.AsPrettyString()).AppendLine("] >>");
        inspection.AppendLine();

        var props = type.GetProperties()
            .Where(a => a.GetIndexParameters().Length == 0)
            .OrderBy(a => a.Name).ToList();

        var fields = type.GetFields().OrderBy(a => a.Name).ToList();

        if (props.Count != 0)
        {
            if (fields.Count != 0) inspection.AppendLine("<< Properties >>");

            var columnWidth = props.Max(a => a.Name.Length) + 5;
            foreach (var prop in props)
            {
                if (inspection.Length > 1800 && discordSizeCap) break;

                var sep = new string(' ', columnWidth - prop.Name.Length);

                inspection.Append(prop.Name)
                    .Append(sep).Append(prop.CanRead ? ReadValue(prop, obj) : "Unreadable")
                    .AppendLine();
            }
        }

        if (fields.Count != 0)
        {
            if (props.Count != 0)
                inspection.AppendLine().AppendLine("<< Fields >>");
                

            var columnWidth = fields.Max(ab => ab.Name.Length) + 5;
            foreach (var prop in fields)
            {
                if (inspection.Length > 1800 && discordSizeCap) break;

                var sep = new string(' ', columnWidth - prop.Name.Length);
                inspection.Append(prop.Name).Append(':').Append(sep).Append(ReadValue(prop, obj)).AppendLine();
            }
        }

        if (obj is IEnumerable objEnumerable)
        {
            var arr = objEnumerable as object[] ?? objEnumerable.Cast<object>().ToArray();
            if (arr.None()) return inspection.ToString();
            inspection.AppendLine();
            inspection.AppendLine("<< Items >>");
            foreach (var prop in arr)
                inspection.AppendLine($" - {prop}");
        }

        return inspection.ToString();
    }

    public string ReadValue(FieldInfo prop, object obj) => ReadValue(prop.Cast<object>(), obj);

    public string ReadValue(PropertyInfo prop, object obj) => ReadValue(prop.Cast<object>(), obj);

    private static string ReadValue(object prop, object obj)
        => TryCatch(() =>
        {
            var value = prop switch
            {
                PropertyInfo pInfo => pInfo.GetValue(obj),

                FieldInfo fInfo => fInfo.GetValue(obj),

                _ => throw new ArgumentException(
                    $"{nameof(prop)} must be PropertyInfo or FieldInfo. Any other type cannot be read.")
            };

            return value switch
            {
                null => "Null",
                IEnumerable e and not string => getEnumerableStr(e),
                _ => $"{value} [{value.GetType().AsPrettyString()}]"
            };
            
            static string getEnumerableStr(IEnumerable e)
            {
                var enu = e.Cast<object>().ToList();
                return $"{enu.Count} [{enu.GetType().AsPrettyString()}]";
            }
        }, e => $"[[{e.GetType().Name} thrown, message: \"{e.Message}\"]]");

    public void Throw<TException>() where TException : Exception
    {
        var ctor = typeof(TException).GetConstructors()
                       .FirstOrDefault(x => x.GetParameters().None())
                   ?? throw new InvalidOperationException(
                       "Specified exception type didn't have a discoverable zero-parameter constructor.");
        throw ctor.Invoke([]).Cast<TException>();
    }
}