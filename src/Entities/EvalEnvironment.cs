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
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.DependencyInjection;
using Qmmands;
using Volte.Commands;
using Volte.Interactions;
using Volte.Entities;
using Volte.Helpers;
using Volte.Services;

// ReSharper disable MemberCanBePrivate.Global

namespace Volte.Entities
{

    public sealed class EvalEnvironment
    {
        public class InteractionBased
        {
            public MessageCommandContext Context { get; set; }
            public DiscordSocketClient Client { get; set; }
            public GuildData Data { get; set; }
            public InteractionService Interactions { get; set; }
            public DatabaseService Database { get; set; }
            public InteractionBased Environment => this;
            
            public string Inheritance<T>() => Inheritance(typeof(T));
            public string Inheritance(object obj) => Inheritance(obj.GetType());
            public string Inheritance(Type type) => InheritanceInternal(type);
            public string Inspect(object obj) => InspectInternal(obj);

            public T Service<T>() where T : notnull => Context.Services.GetRequiredService<T>();
        }

        public VolteContext Context { get; set; }
        public DiscordSocketClient Client { get; set; }
        public GuildData Data { get; set; }
        public CommandService Commands { get; set; }
        public DatabaseService Database { get; set; }
        public EvalEnvironment Environment => this;

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
            Context.Channel.CachedMessages.AnyGet(m => m.Id == id && m is SocketSystemMessage, out var message)
                ? message.Cast<SocketSystemMessage>()
                : throw new InvalidOperationException(
                    $"The ID provided didn't lead to a valid system message, it lead to a(n) {Context.Channel.GetCachedMessage(id)?.Source} message.");

        public SocketUserMessage Message(ulong id) =>
            Context.Channel.CachedMessages.AnyGet(m => m.Id == id && m is SocketUserMessage, out var message)
                ? message.Cast<SocketUserMessage>()
                : throw new InvalidOperationException(
                    $"The ID provided didn't lead to a valid user-created message, it lead to a(n) {Context.Channel.GetCachedMessage(id)?.Source} message.");

        public async ValueTask<IUserMessage> MessageAsync(ulong id)
        {
            var m = await Context.Channel.GetMessageAsync(id);

            return m is IUserMessage result
                ? result
                : throw new InvalidOperationException(
                    $"The ID provided didn't lead to a valid user-created message, it lead to a(n) {m.Source} message.");
        }

        public SocketGuild Guild(ulong id) => Context.Client.GetGuild(id);
        public T Service<T>() where T : notnull => Context.Services.GetRequiredService<T>();

        public SocketUserMessage Message(string id)
            => ulong.TryParse(id, out var ulongId)
                ? Message(ulongId)
                : throw new ArgumentException(
                    $"Method parameter {nameof(id)} is not a valid {typeof(ulong).FullName}.");

        public async ValueTask<IUserMessage> ReplyAsync(string content) =>
            await Context.Channel.SendMessageAsync(content);

        public async ValueTask<IUserMessage> ReplyAsync(Embed embed) => await embed.SendToAsync(Context.Channel);

        public async ValueTask<IUserMessage> ReplyAsync(EmbedBuilder embed) => await embed.SendToAsync(Context.Channel);

        public Task ReactAsync(string unicode) => Context.Message.AddReactionAsync(new Emoji(unicode));

        public string Inheritance<T>() => Inheritance(typeof(T));
        public string Inheritance(object obj) => Inheritance(obj.GetType());
        public string Inheritance(Type type) => InheritanceInternal(type);
        
        internal static string InheritanceInternal(Type type)
        {
            var baseTypes = new List<Type> {type};
            var latestType = type.BaseType;

            while (latestType != null)
            {
                baseTypes.Add(latestType);
                latestType = latestType.BaseType;
            }

            var sb = new StringBuilder().AppendLine($"Inheritance tree for type [{type.FullName}]").AppendLine();

            baseTypes.ForEach(baseType =>
            {
                sb.Append($"[{baseType.AsPrettyString()}]");
                var inheritors = baseType.GetInterfaces().ToList();
                if (baseType.BaseType != null)
                {
                    inheritors = inheritors.ToList();
                    inheritors.Add(baseType.BaseType);
                }

                if (inheritors.Count > 0)
                    sb.Append($": {string.Join(", ", inheritors.Select(x => x.AsPrettyString()))}");

                sb.AppendLine();
            });

            return sb.ToString();
        }
        
        public string Inspect(object obj) => InspectInternal(obj);

        private static string InspectInternal(object obj)
        {
            var type = obj.GetType();

            var inspection = new StringBuilder();
            inspection.Append("<< Inspecting type [").Append(type.AsPrettyString()).AppendLine("] >>");
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

                    inspection.Append(prop.Name)
                        .Append(sep).Append(prop.CanRead ? ReadValue(prop, obj) : "Unreadable")
                        .AppendLine();
                }
            }

            if (fields.Count != 0)
            {
                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                // same here wtf
                if (props.Count != 0)
                    inspection.AppendLine().AppendLine("<< Fields >>");
                

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
                arr.ForEach(prop => inspection.Append(" - ").Append(prop).AppendLine());
            }

            return inspection.ToString();
        }

        public static string ReadValue(FieldInfo prop, object obj) => ReadValue(prop.Cast<object>(), obj);

        public static string ReadValue(PropertyInfo prop, object obj) => ReadValue(prop.Cast<object>(), obj);

        private static string ReadValue(object prop, object obj)
            => Lambda.TryCatch(() =>
            {
                var value = prop switch
                {
                    PropertyInfo pinfo => pinfo.GetValue(obj),

                    FieldInfo finfo => finfo.GetValue(obj),

                    _ => throw new ArgumentException(
                        $"{nameof(prop)} must be PropertyInfo or FieldInfo. Any other type cannot be read.")
                };

                static string GetEnumerableStr(IEnumerable e)
                {
                    var enu = e.Cast<object>().ToList();
                    return $"{enu.Count} [{enu.GetType().AsPrettyString()}]";
                }

                return value switch
                {
                    null => "Null",
                    IEnumerable e when !(value is string) => GetEnumerableStr(e),
                    _ => value + $" [{value.GetType().AsPrettyString()}]"
                };
            }, e => $"[[{e.GetType().Name} thrown, message: \"{e.Message}\"]]");

        public static void Throw<TException>() where TException : Exception
        {
            var ctor = typeof(TException).GetConstructors()
                           .FirstOrDefault(x => x.GetParameters().IsEmpty())
                       ?? throw new InvalidOperationException(
                           "Specified exception type didn't have a discoverable zero-parameter constructor.");
            throw ctor.Invoke(Array.Empty<object>()).Cast<TException>();
        }
    }
}