using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Qmmands;
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
                var parserObj = parser.GetConstructor(Type.EmptyTypes)?.Invoke(Array.Empty<object>());
                var method = meth?.MakeGenericMethod(
                    parser.BaseType?.GenericTypeArguments[0]
                    ?? throw new FormatException("CommandService#AddTypeParser() values invalid."));
                // ReSharper disable once PossibleNullReferenceException
                // cant happen
                method?.Invoke(service, new[] {parserObj, parser.GetCustomAttribute<VolteTypeParserAttribute>().OverridePrimitive});
                yield return parser;
            }
        }

        public static Command GetCommand(this CommandService service, string name)
            => service.GetAllCommands().FirstOrDefault(x => x.FullAliases.ContainsIgnoreCase(name));

        public static int GetTotalTypeParsers(this CommandService cs)
        {
            var customParsers = typeof(VolteBot).Assembly.GetTypes()
                .Count(x => x.HasAttribute<VolteTypeParserAttribute>());
            //add the number of primitive TypeParsers (that come with Qmmands) obtained from the private field _primitiveTypeParsers's Count
            // ReSharper disable twice PossibleNullReferenceException
            var primitiveTypeParsers = cs.GetType()
                .GetField("_primitiveTypeParsers", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(cs)
                .Cast<IDictionary>();
            return customParsers + primitiveTypeParsers.Count;
        }
    }
}