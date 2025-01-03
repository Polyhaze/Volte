using System.Collections.Immutable;
using System.Globalization;
using Avalonia.Data.Converters;
using Gommon;

// ReSharper disable InconsistentNaming

namespace Volte.UI.Converters;

public abstract class BasicValueConverter<TFrom, TTo, TConverter> : IValueConverter where TConverter : IValueConverter, new()
{
    private static readonly Lazy<TConverter> _shared = new(() => new());
    public static TConverter Shared => _shared.Value;

    protected Optional<ImmutableArray<(TFrom From, TTo To)>> Definitions;

    public object Convert(object? value, Type _, object? __, CultureInfo ___) =>
        Definitions.OrDefault()
            .FindFirst(x => x.From!.Equals(value))
            .Convert(x => x.To);

    public object ConvertBack(object? value, Type _, object? __, CultureInfo ___) =>
        Definitions.OrDefault()
            .FindFirst(x => x.To!.Equals(value))
            .Convert(x => x.From);
}