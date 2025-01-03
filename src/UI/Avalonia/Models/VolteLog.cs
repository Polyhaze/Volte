using System.Text;
using Discord;
using Gommon;
using Volte.Entities;
using Volte.Helpers;

// ReSharper disable MemberCanBePrivate.Global

namespace Volte.UI.Avalonia.Models;

public record struct VolteLog
{
    private static readonly int LongestSeverity = Enum
        .GetNames<LogSeverity>()
        .Max(sev => sev.Length);

    private static readonly int LongestSource = Enum
        .GetNames<LogSource>()
        .Max(src => src.Length);

    private static readonly int SeverityPadding = (int)(LongestSeverity * 1.33);
    private static readonly int SourcePadding = (int)(LongestSource * 1.85);

    public VolteLog(VolteLogEventArgs eventData)
    {
        Severity = eventData.Severity;
        Source = eventData.Source;
        Message = eventData.Message;
        Error = eventData.Error;
    }

    public DateTime Date { get; } = DateTime.Now;

    public LogSeverity Severity { get; }
    public LogSource Source { get; }
    public string? Message { get; }
    public Exception? Error { get; }

    public string? StrippedMessage => Message?.Replace(CommandEventArgs.Whitespace, string.Empty);
    public string SeverityName => Enum.GetName(Severity)!.ToUpper();
    public string SourceName => Enum.GetName(Source)!.ToUpper();

    public string FormattedSeverityName => $"{SeverityName}:".P(SeverityPadding);

    public string FormattedSourceName =>
        $"[{SourceName}] ->".P(SourcePadding + 3); //+3 accounts for the space and arrow 

    public string FormattedMessage
    {
        get
        {
            if (Message is not null)
                return StrippedMessage!;

            return new StringBuilder($"{Error!.GetType().AsFullNamePrettyString()}: ")
                .AppendLine(Error.Message.IsNullOrEmpty() ? "No message provided" : Error.Message)
                .Append(Error.StackTrace)
                .ToString();
        }
    }

    private StringBuilder? _formatted = null;

    public string FormattedString => (
        _formatted ??= new StringBuilder(FormattedSeverityName)
            .Append($"[{SourceName}]".P(SourcePadding))
            .Append(FormattedMessage)
    ).ToString();


    public string Markdown =>
        $"""
         `[{Date.FormatDate()} @ {Date.FormatFullTime()}]`
         `[{SourceName}]` `[{SeverityName}]` 

         {Format.Code(StrippedMessage, string.Empty)}
         """;
}