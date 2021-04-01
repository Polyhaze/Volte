using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Qmmands;
using Volte.Commands;
using Volte.Core;
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

        internal static IEnumerable<Type> AddTypeParsers(this CommandService service)
        {
            var assembly = typeof(VolteBot).Assembly;
            var meth = typeof(CommandService).GetMethod("AddTypeParser");
            var parsers = assembly.ExportedTypes.Where(x => x.HasAttribute<VolteTypeParserAttribute>()).ToList();
            
            foreach (var parser in parsers)
            {
                var attr = parser.GetCustomAttribute<VolteTypeParserAttribute>();
                var parserObj = parser.GetConstructor(Type.EmptyTypes)?.Invoke(Array.Empty<object>());
                var method = meth?.MakeGenericMethod(
                    parser.BaseType?.GenericTypeArguments[0]
                    ?? throw new FormatException("CommandService#AddTypeParser() values invalid."));
                method?.Invoke(service, new[] {parserObj, attr?.OverridePrimitive});
                yield return parser;
            }
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