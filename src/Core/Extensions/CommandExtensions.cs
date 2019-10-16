using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Qmmands;
using Volte.Commands;
using Volte.Commands.TypeParsers;
using Volte.Core;
using Module = Qmmands.Module;

namespace Gommon
{
    public static partial class Extensions
    {
        public static string SanitizeName(this Module m)
            => m.Name.Replace("Module", string.Empty);

        public static string GetUsage(this Command c, VolteContext ctx)
            => (c.Remarks ?? "No usage provided")
                .Replace(c.Name.ToLower(), (c.FullAliases.Count > 1 ? $"({c.FullAliases.Join('|')})" : c.Name).ToLower())
                .Insert(0, ctx.GuildData.Configuration.CommandPrefix);

        internal static Task<List<Type>> AddTypeParsersAsync(this CommandService service)
        {
            var currentAssembly = Assembly.GetExecutingAssembly();
            var addTypeParserMethod = typeof(CommandService).GetMethod("AddTypeParser");
            var parsers = currentAssembly.ExportedTypes.Where(x => x.HasAttribute<VolteTypeParserAttribute>());

            var loadedTypes = new List<Type>();

            foreach (var parserType in parsers)
            {
                var attr = parserType.GetCustomAttribute<VolteTypeParserAttribute>();
                var parser = parserType.GetConstructor(Type.EmptyTypes)?.Invoke(Array.Empty<object>());
                var method = addTypeParserMethod?.MakeGenericMethod(parserType.BaseType?.GenericTypeArguments[0]);
                method?.Invoke(service, new[] {parser, attr.OverridePrimitive});
                loadedTypes.Add(parserType);
            }


            return Task.FromResult(loadedTypes);
        }

        public static Command GetCommand(this CommandService service, string name)
            => service.GetAllCommands().FirstOrDefault(x => x.FullAliases.ContainsIgnoreCase(name));

        public static int GetTotalTypeParsers(this CommandService _)
        {
            var customParsers = typeof(VolteBot).Assembly.GetTypes()
                .Count(x => x.HasAttribute<VolteTypeParserAttribute>());
            return customParsers + (13 - 1); //add the number of primitive TypeParsers (that come with Qmmands), minus bool since we override that one.
        }
    }
}