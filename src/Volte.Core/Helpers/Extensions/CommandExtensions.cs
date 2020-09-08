using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using JetBrains.Annotations;
using Qmmands;
using Qommon.Collections;
using Volte.Commands;
using Volte.Commands.Checks;
using Volte.Core;
using Volte.Commands.TypeParsers;
using Volte.Core.Entities;
using Module = Qmmands.Module;

namespace Gommon
{
    public static partial class Extensions
    {
        public static string SanitizeName(this Module m)
            => m.Name.Replace("Module", string.Empty);
        
        public static string SanitizeParserName(this Type type)
            => type.Name.Replace("Parser", string.Empty);

        public static string GetUsage(this Command c, VolteContext ctx)
            => (c.Remarks ?? "No usage provided")
                .Replace(c.Name.ToLower(), c.AsPrettyString().ToLower())
                .Insert(0, ctx.GuildData.Configuration.CommandPrefix);

        public static string AsPrettyString(this Command c)
            => c.FullAliases.Count > 1 ? $"({c.FullAliases.Join('|')})" : c.Name;

        public static string GenerateHelp(this Command c)
        {
            var sb = new StringBuilder();

            foreach (var arg in c.Parameters)
            {
                var result = arg.Type.AsPrettyString().Replace("Discord", string.Empty);
                var optionalAttr = arg.Attributes.FirstOrDefault(x => x is OptionalArgumentAttribute)
                    .Cast<OptionalArgumentAttribute>();
                var requiredAttr = arg.Attributes.FirstOrDefault(x => x is RequiredArgumentAttribute)
                    .Cast<RequiredArgumentAttribute>();

                if (optionalAttr is not null)
                {
                    sb.Append(optionalAttr.ValidFormat is not null
                        ? $"[{optionalAttr.ValidFormat}]"
                        : $"[{result}]").Append(' ');
                    continue;
                }

                if (requiredAttr is not null)
                {
                    sb.Append(requiredAttr.ValidFormat is not null
                        ? $"{{{requiredAttr.ValidFormat}}}"
                        : $"{{{result}}}").Append(' ');
                    continue;
                }

                sb.Append($"{{{result}}}").Append(' ');
            }

            return sb.ToString();
        }

        [NotNull]
        public static VolteContext AsVolteContext(this CommandContext ctx) =>
            ctx.Cast<VolteContext>() ?? throw new InvalidOperationException($"Cast to {nameof(VolteContext)} from {ctx.GetType().AsPrettyString()} was unsuccessful. " +
                                                                            $"Please make sure the {nameof(CommandContext)} you passed is actually a {nameof(VolteContext)}. Received: {ctx.GetType().AsPrettyString()}");
        
        public static bool IsMod(this Command command)
        {
            if (command is null) return false;
            return command.Attributes.Any(x => x is RequireGuildModeratorAttribute) || command.Module.IsMod();
        }

        public static bool IsMod(this Module module)
        {
            if (module is null) return false;
            return module.Attributes.Any(x => x is RequireGuildModeratorAttribute);
        }

        public static bool IsAdmin(this Command command)
        {
            if (command is null) return false;
            return command.Attributes.Any(x => x is RequireGuildAdminAttribute) || command.Module.IsAdmin();
        }

        public static bool IsAdmin(this Module module)
        {
            if (module is null) return false;
            return module.Attributes.Any(x => x is RequireGuildAdminAttribute);
        }

        public static bool IsBotOwner(this Command command)
        {
            if (command is null) return false;
            return command.Attributes.Any(x => x is RequireBotOwnerAttribute) || command.Module.IsBotOwner();
        }

        public static bool IsBotOwner(this Module module)
        {
            if (module is null) return false;
            return module.Attributes.Any(x => x is RequireBotOwnerAttribute);
        }

        internal static ReadOnlyList<Type> AddTypeParsersAsync(this CommandService service)
        {
            var assembly = typeof(VolteBot).Assembly;
            var meth = typeof(CommandService).GetMethod("AddTypeParser");
            var parsers = assembly.ExportedTypes.Where(x => x.HasAttribute<VolteTypeParserAttribute>()).ToList();

            var loadedTypes = new List<Type>();
            parsers.ForEach(parserType =>
            {
                var attr = parserType.GetCustomAttribute<VolteTypeParserAttribute>();
                var parser = parserType.GetConstructor(Type.EmptyTypes)?.Invoke(Array.Empty<object>());
                var method = meth?.MakeGenericMethod(
                    parserType.BaseType?.GenericTypeArguments[0]
                    ?? throw new FormatException("CommandService#AddTypeParser() values invalid."));
                method?.Invoke(service, new[] {parser, attr?.OverridePrimitive});
                loadedTypes.Add(parserType);
            });

            return new ReadOnlyList<Type>(loadedTypes);
        }

        public static Command GetCommand(this CommandService service, string name)
            => service.GetAllCommands().FirstOrDefault(x => x.FullAliases.ContainsIgnoreCase(name));

        public static int GetTotalTypeParsers(this CommandService _)
        {
            var customParsers = typeof(VolteBot).Assembly.GetTypes()
                .Count(x => x.HasAttribute<VolteTypeParserAttribute>());
            //add the number of primitive TypeParsers (that come with Qmmands), which is 13, minus bool since we override that one, therefore 12.
            return customParsers + (12); 
        }
    }
}