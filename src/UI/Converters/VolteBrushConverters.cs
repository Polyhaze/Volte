using System.Collections.Immutable;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;
using Discord;
using Gommon;
using Volte.Entities;
using Color = System.Drawing.Color;

namespace Volte.UI.Converters;

public class LogSeverityToBrush : VolteBrushConverter<LogSeverity, LogSeverityToBrush>
{
    public LogSeverityToBrush() =>
        SetBrushDefinitions(
            (LogSeverity.Critical, Color.Maroon),
            (LogSeverity.Error, Color.DarkRed),
            (LogSeverity.Warning, Color.Yellow),
            (LogSeverity.Info, Color.SpringGreen),
            (LogSeverity.Verbose, Color.Pink),
            (LogSeverity.Debug, Color.SandyBrown)
        );
}

public class LogSourceToBrush : VolteBrushConverter<LogSource, LogSourceToBrush>
{
    public LogSourceToBrush() =>
        SetBrushDefinitions(
            (LogSource.Volte, Color.LawnGreen),
            (LogSource.Discord, Color.RoyalBlue),
            (LogSource.Gateway, Color.RoyalBlue),
            (LogSource.Service, Color.Gold),
            (LogSource.Module, Color.LimeGreen),
            (LogSource.Rest, Color.Red),
            (LogSource.Sentry, Color.Chartreuse),
            (LogSource.UI, Color.Crimson),
            (LogSource.Unknown, Color.Fuchsia)
        );
}

public abstract class VolteBrushConverter<TSource, TConverter> : BasicValueConverter<TSource, IBrush, TConverter> 
    where TConverter : IValueConverter, new()
{
    protected void SetBrushDefinitions(params (TSource Raw, Color Color)[] definitions) =>
        Definitions = definitions
            .Select(x => (x.Raw, Brush.Parse(x.Color.ToHexadecimalString())))
            .ToImmutableArray();
}