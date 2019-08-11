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
            Color? color = null;
            var colorString = value.StartsWith("#") ? value.Substring(1) : value;

            if (int.TryParse(colorString, NumberStyles.HexNumber, CultureInfo.CurrentCulture, out var colorInt))
                color = new Color(colorInt.Cast<uint>());

            if (color is null)
            {
                try
                {
                    var r = int.Parse(value.Split(" ")[0]);
                    var g = int.Parse(value.Split(" ")[1]);
                    var b = int.Parse(value.Split(" ")[2]);

                    if (r > 255 || g > 255 || b > 255)
                    {
                        return new ValueTask<TypeParserResult<Color>>(TypeParserResult<Color>.Unsuccessful(
                            "A value in an RGB sequence may not be over the value of 255."));
                    }

                    color = new Color(r, g, b);
                }
                catch (IndexOutOfRangeException)
                {
                    color = null;
                }
                catch (FormatException)
                {
                    color = null;
                }
            }

            return new ValueTask<TypeParserResult<Color>>(color is null
                ? TypeParserResult<Color>.Unsuccessful("A color could not be determined from your input text. Try using a hex value.")
                : TypeParserResult<Color>.Successful(color.Value));
        }
    }
}
