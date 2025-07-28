using System.Globalization;
using Discord.Interactions;

namespace Volte.Interactions.Commands.TypeConverters;

public class ColorTypeConverter : TypeConverter<Color>
{
    public override ApplicationCommandOptionType GetDiscordType() => ApplicationCommandOptionType.String;

    public override Task<TypeConverterResult> ReadAsync(
        IInteractionContext context, IApplicationCommandInteractionDataOption opt, IServiceProvider services)
    {
        var option = opt.Value.ToString()!;
        
        Color? c = null;

        if (uint.TryParse(option.StartsWith('#') ? option[1..] : option, NumberStyles.HexNumber,
                CultureInfo.CurrentCulture, out var colorInt))
            c = new(colorInt);

        if (c is null)
        {
            try
            {
                var val = option.Split(" ");

                var r = val[0].Parse<int>();
                var g = val[1].Parse<int>();
                var b = val[2].Parse<int>();

                if (r > 255 || g > 255 || b > 255)
                {
                    return Task.FromResult(TypeConverterResult.FromError(InteractionCommandError.ConvertFailed,
                        "A value in an RGB sequence may not be over the value of 255."));
                }

                c = new(r, g, b);
            }
            catch
            {
                // ignored
            }
        }

        return Task.FromResult(c is null
            ? TypeConverterResult.FromError(InteractionCommandError.ConvertFailed,
                "A color could not be determined from your input text. Try using a hex value.")
            : TypeConverterResult.FromSuccess(c.Value)
        );
    }
}