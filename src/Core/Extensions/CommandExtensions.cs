using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Qmmands;
using Volte.Commands;
using Volte.Commands.TypeParsers;
using Volte.Services;
using Module = Qmmands.Module;

namespace Gommon
{
    public static partial class Extensions
    {
        public static string SanitizeName(this Module m)
            => m.Name.Replace("Module", string.Empty);

        public static string SanitizeRemarks(this Command c, VolteContext ctx)
        {
            ctx.ServiceProvider.Get<DatabaseService>(out var db);
            var aliases = $"({c.FullAliases.Join('|')})";
            return (c.Remarks ?? "No usage provided")
                .Replace(c.Name.ToLower(), (c.FullAliases.Count > 1 ? aliases : c.Name).ToLower())
                .Replace("|prefix|", db.GetData(ctx.Guild).Configuration.CommandPrefix)
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

                var parser = type.GetConstructor(Type.EmptyTypes)?.Invoke(new object[] { });
                var method = addTypeParserMethod?.MakeGenericMethod(type.BaseType?.GenericTypeArguments[0]);
                method?.Invoke(service, new [] { parser, attr.OverridePrimitive });
                loadedTypes.Add(type);
            }


            return Task.FromResult(loadedTypes);
        }
    }
}