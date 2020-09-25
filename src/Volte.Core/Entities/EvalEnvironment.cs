using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using Gommon;
using Microsoft.Extensions.DependencyInjection;
using Qmmands;
using Volte.Commands;
using Volte.Services;
// ReSharper disable MemberCanBePrivate.Global

namespace Volte.Core.Entities
{
    public sealed class EvalEnvironment
    {

        internal EvalEnvironment()
        {
            Environment = this;
        }

        public static EvalEnvironment From(VolteContext ctx)
        {
            var shardId = ctx.Client.GetShardId(ctx.Guild.Id);
            var e = new EvalEnvironment 
            {
                Context = ctx,
                Client = ctx.Client.ShardClients[shardId],
                Data = ctx.ServiceProvider.Get<DatabaseService>().GetData(ctx.Guild),
                Logger = ctx.ServiceProvider.Get<LoggingService>(),
                Commands = ctx.ServiceProvider.Get<CommandService>(),
                Database = ctx.ServiceProvider.Get<DatabaseService>()
            };
            return e;

        }

        public VolteContext Context { get; set; }
        public DiscordClient Client { get; set; }
        public GuildData Data { get; set; }
        public LoggingService Logger { get; set; }
        public CommandService Commands { get; set; }
        public DatabaseService Database { get; set; }
        public EvalEnvironment Environment { get; set; }

        public DiscordMember Member(ulong id) => Context.Guild.Members[id];
        public DiscordMember Member(string username) => Context.Guild.Members.Values.FirstOrDefault(a => a.Username.EqualsIgnoreCase(username) || 
            (a.Nickname is not null && a.Nickname.EqualsIgnoreCase(username)));
        public DiscordChannel TextChannel(ulong id) => Context.Client.FindFirstChannel(id);
        public Task<DiscordMessage> MessageAsync(ulong id) => Context.Channel.GetMessageAsync(id);

        public DiscordGuild Guild(ulong id) => Context.Client.GetGuild(id);
        public T GetService<T>() => Context.ServiceProvider.GetRequiredService<T>();

        public Task<DiscordMessage> MessageAsync(string id)
        {
            if (ulong.TryParse(id, out var ulongId))
            {
                return MessageAsync(ulongId);
            }
            throw new ArgumentException($"Method parameter {nameof(id)} is not a valid {typeof(ulong).AsPrettyString()}.");
        }

        public string Inheritance<T>() => InheritanceInternal(typeof(T));
        public string Inheritance(object obj) => InheritanceInternal(obj is Type type ? type : obj.GetType());
        private string InheritanceInternal(Type type)
        {
            var baseTypes = new List<Type> {type};
            var latestType = type.BaseType;

            while (latestType is not null)
            {
                baseTypes.Add(latestType);
                latestType = latestType.BaseType;
            }

            var sb = new StringBuilder().AppendLine($"Inheritance tree for type [{type.AsPrettyString()}]").AppendLine();

            foreach (var baseType in baseTypes)
            {
                sb.Append($"[{baseType.AsPrettyString()}]");
                var inheritors = baseType.GetInterfaces().ToList();
                if (baseType.BaseType is not null)
                {
                    inheritors.Add(baseType.BaseType);
                }
                if (inheritors.Count > 0) sb.Append($": {inheritors.Select(x => x.AsPrettyString()).Join(", ")}");

                sb.AppendLine();
            }

            return sb.ToString();
        }

        public string Inspect(object obj)
        {
            var type = obj.GetType();

            var inspection = new StringBuilder();
            inspection.AppendLine($"<< Inspecting object of type [{type.AsPrettyString()}] >>").AppendLine();

            var props = type.GetProperties().Where(a => a.GetIndexParameters().Length == 0)
                .OrderBy(a => a.Name).ToList();

            var fields = type.GetFields().OrderBy(a => a.Name).ToList();

            if (props.Count is not 0)
            {
                if (fields.Count is not 0) inspection.AppendLine("<< Properties >>");

                var columnWidth = props.Max(a => a.Name.Length) + 5;
                foreach (var prop in props)
                {
                    if (inspection.Length > 1800) break;

                    var sep = new string(' ', columnWidth - prop.Name.Length);

                    inspection.Append(prop.Name).Append(sep).Append(prop.CanRead ? ReadValue(prop, obj) : "Unreadable").AppendLine();
                }
            }

            if (fields.Count is not 0)
            {
                if (props.Count is not 0)
                {
                    inspection.AppendLine();
                    inspection.AppendLine("<< Fields >>");
                }

                var columnWidth = fields.Max(ab => ab.Name.Length) + 5;
                foreach (var prop in fields)
                {
                    if (inspection.Length > 1800) break;

                    var sep = new string(' ', columnWidth - prop.Name.Length);
                    inspection.Append(prop.Name).Append(':').Append(sep).Append(ReadValue(prop, obj)).AppendLine();
                }
            }
            
            if (obj is IEnumerable objEnumerable)
            {
                var arr = objEnumerable as object[] ?? objEnumerable.Cast<object>().ToArray();
                if (arr.IsEmpty()) return inspection.ToString();
                inspection.AppendLine();
                inspection.AppendLine("<< Items >>");
                foreach (var prop in arr) inspection.Append(" - ").Append(prop).AppendLine();
            }

            return inspection.ToString();
        }

        public object ReadValue(FieldInfo prop, object obj) => ReadValue((object)prop, obj);

        public object ReadValue(PropertyInfo prop, object obj) => ReadValue((object)prop, obj);

        private string ReadValue(object prop, object obj)
        {
            try
            {
                var value = prop switch
                    {
                    PropertyInfo pi => pi.GetValue(obj),

                    FieldInfo fi => fi.GetValue(obj),

                    _ => throw new ArgumentException($"{nameof(prop)} must be {typeof(PropertyInfo).AsPrettyString()} or {typeof(FieldInfo).AsPrettyString()}. Any other type cannot be read.", nameof(prop)),
                    };

                switch (value)
                {
                    case null:
                        return "Null";
                    case IEnumerable e when value is not string:
                    {
                        var enu = e.Cast<object>().ToList();
                        return $"{enu.Count} [{enu.GetType().AsPrettyString()}]";
                    }
                    default:
                        return value + $" [{value.GetType().AsPrettyString()}]";
                }
            }
            catch (Exception e)
            {
                return $"[[{e.GetType().AsPrettyString()} thrown, message: \"{e.Message}\"]]";
            }
        }

        public void Throw() => throw new Exception("Test exception.");

    }
}