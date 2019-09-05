using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Qmmands;
using Volte.Commands;
using Volte.Commands.TypeParsers;
using Volte.Core;
using Volte.Services;
using Module = Qmmands.Module;

namespace Gommon
{
    public static partial class Extensions
    {
        public static string SanitizeName(this Module m)
            => m.Name.Replace("Module", string.Empty);

        public static string GetUsage(this Command c, VolteContext ctx)
        {
            var aliases = $"({c.FullAliases.Join('|')})";
            return (c.Remarks ?? "No usage provided")
                .Replace(c.Name.ToLower(), (c.FullAliases.Count > 1 ? aliases : c.Name).ToLower())
                .Replace("|prefix|", ctx.GuildData.Configuration.CommandPrefix)
                .Replace("Usage: ", string.Empty);
        }

        internal static Task<List<Type>> AddTypeParsersAsync(this CommandService service)
        {
            var currentAssembly = Assembly.GetExecutingAssembly();
            var addTypeParserMethod = typeof(CommandService).GetMethod("AddTypeParser");

            var loadedTypes = new List<Type>();

            foreach (var type in currentAssembly.ExportedTypes)
            {
                if (!(type.GetCustomAttributes().FirstOrDefault(a => a is VolteTypeParserAttribute) is VolteTypeParserAttribute attr)) continue;

                var parser = type.GetConstructor(Type.EmptyTypes)?.Invoke(Array.Empty<object>());
                var method = addTypeParserMethod?.MakeGenericMethod(type.BaseType?.GenericTypeArguments[0]);
                method?.Invoke(service, new [] { parser, attr.OverridePrimitive });
                loadedTypes.Add(type);
            }


            return Task.FromResult(loadedTypes);
        }

        public static Command GetCommand(this CommandService service, string name) 
            => service.GetAllCommands().FirstOrDefault(x => x.FullAliases.ContainsIgnoreCase(name));

        public static int GetTotalTypeParsers(this CommandService _)
        {
            var customParsers = typeof(VolteBot).Assembly.GetTypes().Count(x => x.HasAttribute<VolteTypeParserAttribute>());
            return customParsers + 12; //add the number of primitive typeparsers (that come with Qmmands), minus bool since we override that one.
        }
    }
}