using System;

namespace Gommon
{
    public static partial class Extensions
    {
        //an extension on all `Type` instances, it's only applicable on typeparsers.
        public static string SanitizeParserName(this Type type)
            => type.Name.Replace("Parser", string.Empty);

    }
}
