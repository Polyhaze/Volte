using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Gommon;
using Microsoft.Extensions.DependencyInjection;
using Qmmands;
using Volte.Commands;
using Volte.Core.Models.Guild;
using Volte.Services;
// ReSharper disable MemberCanBePrivate.Global

namespace Volte.Core.Models
{
    public sealed class EvalEnvironment
    {

        internal EvalEnvironment()
        {
            Environment = this;
        }

        public VolteContext Context { get; set; }
        public DiscordSocketClient Client { get; set; }
        public GuildData Data { get; set; }
        public LoggingService Logger { get; set; }
        public CommandService Commands { get; set; }
        public DatabaseService Database { get; set; }
        public EmojiService Emoji { get; set; }
        public EvalEnvironment Environment { get; set; }

        public SocketGuildUser User(ulong id) 
            => Context.Guild.GetUser(id);

        public SocketGuildUser User(string username) 
            => Context.Guild.Users.FirstOrDefault(a => a.Username.EqualsIgnoreCase(username) || (a.Nickname != null && a.Nickname.EqualsIgnoreCase(username)));

        public SocketTextChannel TextChannel(ulong id)
            => Context.Client.GetChannel(id).Cast<SocketTextChannel>();
        
        public SocketUserMessage Message(ulong id)
            => Context.Channel.GetCachedMessage(id).Cast<SocketUserMessage>() ?? throw new InvalidOperationException($"The ID provided didn't lead to a valid user-created message, it lead to a(n) {Context.Channel.GetCachedMessage(id)?.Source} message.");
        
        public async Task<IUserMessage> MessageAsync(ulong id)
        {
            var m = await Context.Channel.GetMessageAsync(id);
            if (m is IUserMessage userMessage)
            {
                return userMessage;
            }
            throw new InvalidOperationException($"The ID provided didn't lead to a valid user-created message, it lead to a(n) {m.Source} message.");
        }
        
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
            throw new ArgumentException($"Method parameter {nameof(id)} is not a valid {typeof(ulong).FullName}.");
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
                sb.Append($"[{FormatType(baseType)}]");
                var inheritors = baseType.GetInterfaces().ToList();
                if (baseType.BaseType != null)
                {
                    inheritors = inheritors.ToList();
                    inheritors.Add(baseType.BaseType);
                }
                if (inheritors.Count > 0) sb.Append($": {string.Join(", ", inheritors.Select(FormatType))}");

                sb.AppendLine();
            }

            return sb.ToString();
        }

        public string Inspect(object obj)
        {
            var type = obj.GetType();

            var inspection = new StringBuilder();
            inspection.Append("<< Inspecting type [").Append(FormatType(type)).AppendLine("] >>");
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
                var arr = objEnumerable as object[] ?? objEnumerable.Cast<object>().ToArray();
                if (arr.IsEmpty()) return inspection.ToString();
                inspection.AppendLine();
                inspection.AppendLine("<< Items >>");
                foreach (var prop in arr) inspection.Append(" - ").Append(prop).AppendLine();
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

                    _ => throw new ArgumentException($"{nameof(prop)} must be PropertyInfo or FieldInfo. Any other type cannot be read.", nameof(prop)),
                    };

                if (value is null) return "Null";

                if (value is IEnumerable e && !(value is string))
                {
                    var enu = e.Cast<object>().ToList();
                    return $"{enu.Count} [{enu.GetType().Name}]";
                }
                return value + $" [{FormatType(value.GetType())}]";

            }
            catch (Exception e)
            {
                return $"[[{e.GetType().Name} thrown, message: \"{e.Message}\"]]";
            }
        }
        
        public void Throw()
        {
            throw new Exception("Test exception.");
        }

        public string FormatType(Type type)
        {
            var t = type.GenericTypeArguments;
            var vs = type.Name.Replace($"`{t.Length}", ""); //thanks .NET for putting an annoying ass backtick and number at the end of type names.

            if (!t.IsEmpty()) vs += $"<{t.Select(a => a.Name).Join(", ")}>";

            return vs;
        }

    }
}