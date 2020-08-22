using System;
using System.Globalization;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using Qmmands;

namespace Volte.Commands.TypeParsers
{
    [VolteTypeParser]
    public sealed class ColorParser : TypeParser<DiscordColor>
    {
        public override ValueTask<TypeParserResult<DiscordColor>> ParseAsync(
            Parameter _, 
            string value, 
            CommandContext __)
        {
            DiscordColor? c = null;

            if (uint.TryParse(value.StartsWith("#") ? value.Substring(1) : value, NumberStyles.HexNumber, CultureInfo.CurrentCulture, out var colorInt))
                c = new DiscordColor((int) colorInt);

            if (c is null)
            {
                try
                {
                    var val = value.Split(" ");
                    var r = int.Parse(val[0]);
                    var g = int.Parse(val[1]);
                    var b = int.Parse(val[2]);

                    if (r > 255 || g > 255 || b > 255)
                    {
                        return TypeParserResult<DiscordColor>.Unsuccessful(
                            "A value in an RGB sequence may not be over the value of 255.");
                    }

                    c = new DiscordColor(r, g, b);
                }
                catch (Exception)
                { 
                    c = null;
                }
            }

            return c is null
                ? TypeParserResult<DiscordColor>.Unsuccessful("A color could not be determined from your input text. Try using a hex value.")
                : TypeParserResult<DiscordColor>.Successful(c.Value);
        }
    }
}
