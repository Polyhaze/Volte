using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Discord.WebSocket;
using Gommon;
using Microsoft.Extensions.DependencyInjection;
using Qmmands;
using Volte.Commands;
using Volte.Core.Models.Guild;
using Volte.Services;

namespace Volte.Core.Models
{
    public sealed class EvalEnvironment
    {

        internal EvalEnvironment() { }

        public VolteContext Context { get; set; }
        public DiscordSocketClient Client { get; set; }
        public GuildData Data { get; set; }
        public LoggingService Logger { get; set; }
        public CommandService CommandService { get; set; }
        public DatabaseService DatabaseService { get; set; }
        public EmojiService EmojiService { get; set; }

        public SocketGuildUser User(ulong id) 
            => Context.Guild.GetUser(id);

        public SocketGuildUser User(string username) 
            => Context.Guild.Users.FirstOrDefault(a => a.Username.EqualsIgnoreCase(username) || (a.Nickname != null && a.Nickname.EqualsIgnoreCase(username)));

        public SocketTextChannel TextChannel(ulong id)
            => Context.Client.GetChannel(id).Cast<SocketTextChannel>();

        public SocketUserMessage Message(ulong id)
            => Context.Channel.GetCachedMessage(id).Cast<SocketUserMessage>() ?? throw new InvalidOperationException($"The ID provided didn't lead to a valid user-created message, it lead to a(n) {Context.Channel.GetCachedMessage(id)?.Source} message.");

        public SocketGuild Guild(ulong id)
            => Context.Client.GetGuild(id);

        public T GetFromProvider<T>()
            => Context.ServiceProvider.GetRequiredService<T>();

        public SocketUserMessage Message(string id)
        {
            if (ulong.TryParse(id, out var ulongId))
            {
                return Message(ulongId);
            }
            throw new ArgumentException($"Method parameter {nameof(id)} is not a valid {typeof(ulong)}.");
        }

        public string Inheritance<T>() 
            => Inheritance(typeof(T));

        public string Inheritance(object obj)
            => Inheritance(obj.GetType());

        public string Inheritance(Type type)
        {
            var baseTypes = new List<Type> {type};
            var latestType = type.BaseType;

            while (latestType != null)
            {
                baseTypes.Add(latestType);
                latestType = latestType.BaseType;
            }

            var sb = new StringBuilder().AppendLine($"Inheritance tree for type [{type.FullName}]").AppendLine();

            foreach (var baseType in baseTypes)
            {
                sb.Append($"[{FormatTypeParameters(baseType)}]");
                IList<Type> inheritors = baseType.GetInterfaces();
                if (baseType.BaseType != null)
                {
                    inheritors = inheritors.ToList();
                    inheritors.Add(baseType.BaseType);
                }
                if (inheritors.Count > 0) sb.Append($": {string.Join(", ", inheritors.Select(FormatTypeParameters))}");

                sb.AppendLine();
            }

            return sb.ToString();
        }

        private string FormatTypeParameters(Type type)
        {
            var vs = $"{type.Namespace}.{type.Name}";

            var t = type.GenericTypeArguments;

            if (t.Length > 0) vs += $"<{string.Join(", ", t.Select(a => a.Name))}>";

            return vs;
        }

        public string Inspect(object obj)
        {
            var type = obj.GetType();

            var inspection = new StringBuilder();
            inspection.Append("<< Inspecting type [").Append(type.Name).AppendLine("] >>");
            inspection.Append("<< String Representation: [").Append(obj).AppendLine("] >>");
            inspection.AppendLine();

            var props = type.GetProperties().Where(a => a.GetIndexParameters().Length == 0)
                .OrderBy(a => a.Name).ToList();

            var fields = type.GetFields().OrderBy(a => a.Name).ToList();

            if (props.Count != 0)
            {
                if (fields.Count != 0) inspection.AppendLine("<< Properties >>");

                var columnWidth = props.Max(a => a.Name.Length) + 5;
                foreach (var prop in props)
                {
                    if (inspection.Length > 1800) break;

                    var sep = new string(' ', columnWidth - prop.Name.Length);

                    inspection.Append(prop.Name).Append(sep).Append(prop.CanRead ? ReadValue(prop, obj) : "Unreadable").AppendLine();
                }
            }

            if (fields.Count != 0)
            {
                if (props.Count != 0)
                {
                    inspection.AppendLine();
                    inspection.AppendLine("<< Fields >>");
                }

                var columnWidth = fields.Max(ab => ab.Name.Length) + 5;
                foreach (var prop in fields)
                {
                    if (inspection.Length > 1800) break;

                    var sep = new string(' ', columnWidth - prop.Name.Length);
                    inspection.Append(prop.Name).Append(":").Append(sep).Append(ReadValue(prop, obj)).AppendLine();
                }
            }

            if (obj is IEnumerable objEnumerable)
            {
                inspection.AppendLine();
                inspection.AppendLine("<< Items >>");
                foreach (var prop in objEnumerable) inspection.Append(" - ").Append(prop).AppendLine();
            }

            return inspection.ToString();
        }

        public object ReadValue(FieldInfo prop, object obj) 
            => ReadValue((object)prop, obj);

        public object ReadValue(PropertyInfo prop, object obj) 
            => ReadValue((object)prop, obj);

        private string ReadValue(object prop, object obj)
        {
            try
            {
                var value = prop switch
                    {
                    PropertyInfo pinfo => pinfo.GetValue(obj),

                    FieldInfo finfo => finfo.GetValue(obj),

                    _ => throw new ArgumentException($"{nameof(prop)} must be PropertyInfo or FieldInfo", nameof(prop)),
                    };

                if (value is null) return "Null";

                if (value is IEnumerable e && !(value is string))
                {
                    var enu = e.Cast<object>().ToList();
                    return $"{enu.Count} [{enu.GetType().Name}]";
                }
                return value + $" [{value.GetType().Name}]";

            }
            catch (Exception e)
            {
                return $"[[{e.GetType().FullName} thrown]]";
            }
        }

    }
}