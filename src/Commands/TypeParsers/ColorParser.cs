using System;
using System.Globalization;
using System.Threading.Tasks;
using Discord;
using Gommon;
using Qmmands;

namespace Volte.Commands.TypeParsers
{
    [VolteTypeParser]
    public sealed class ColorParser : TypeParser<Color>
    {
        public override ValueTask<TypeParserResult<Color>> ParseAsync(
            Parameter parameter, 
            string value, 
            CommandContext context, 
            IServiceProvider provider)
        {
            Color? c = null;

            if (uint.TryParse(value.StartsWith("#") ? value.Substring(1) : value, NumberStyles.HexNumber, CultureInfo.CurrentCulture, out var colorInt))
                c = new Color(colorInt);

            if (c is null)
            {
                try
                {
                    var r = int.Parse(value.Split(" ")[0]);
                    var g = int.Parse(value.Split(" ")[1]);
                    var b = int.Parse(value.Split(" ")[2]);

                    if (r > 255 || g > 255 || b > 255)
                    {
                        return TypeParserResult<Color>.Unsuccessful(
                            "A value in an RGB sequence may not be over the value of 255.");
                    }

                    c = new Color(r, g, b);
                }
                catch (Exception)
                { 
                    c = null;
                }
            }

            return c is null
                ? TypeParserResult<Color>.Unsuccessful("A color could not be determined from your input text. Try using a hex value.")
                : TypeParserResult<Color>.Successful(c.Value);
        }
    }
}
