// ReSharper disable MemberCanBePrivate.Global

namespace Volte.Entities;

/// <summary>
///     The base class for all Command-related Volte EventArgs.
/// </summary>
public abstract class CommandEventArgs : EventArgs
{
    public string Command { get; protected init; }
    public string Arguments { get; protected init; }
    public Stopwatch Stopwatch { get; protected init; }
    public VolteContext Context { get; protected init; }

    public string FormatInvocator(bool whitespace = false) 
        => Fmt(CommandFrom, $"{Context.User} ({Context.User.Id})", whitespace);
    public string FormatTargetCommand() => Fmt(CommandIssued, Context.Command.Name);
    public string FormatInvocationMessage() => Fmt(FullMessage, Context.Message.Content);
    public string FormatSourceGuild() 
        => Fmt(InGuild, $"{Context.Guild.Name} ({Context.Guild.Id})");
    public string FormatSourceChannel() 
        => Fmt(InChannel, $"{Context.Channel.Name} ({Context.Channel.Id})");
    public string FormatTimestamp() 
        => Fmt(TimeIssued, $"{Context.Now.FormatFullTime()}, {Context.Now.FormatDate()}");
    public string FormatTimeTaken() 
        => Fmt(After, $"{Stopwatch.Elapsed.Humanize()}");

    protected const int SpaceCount = 20;
    protected const int HyphenCount = 49;

    protected const string CommandFrom = "Command from user";
    protected const string CommandIssued = "Command Issued";
    protected const string FullMessage = "Full Message";
    protected const string InGuild = "In Guild";
    protected const string InChannel = "In Channel";
    protected const string TimeIssued = "Time Issued";
    protected const string After = "After";
    protected const string ResultMessage = "Result Message";
    protected const string Executed = "Executed";

    public static readonly string Whitespace = string.Intern(new string(' ', SpaceCount));

    public static readonly string Separator = string.Intern(
        new StringBuilder(Whitespace)
            .Append(new string('-', HyphenCount))
            .ToString()
    );

    protected static string Fmt(
        string commandInfoLine,
        string content,
        bool whitespace = true
    ) => new StringBuilder(whitespace ? Whitespace : string.Empty)
        .Append('|')
        .Append(('-' + commandInfoLine).PadLeft(20))
        .Append(": ")
        .Append(content)
        .ToString();
}