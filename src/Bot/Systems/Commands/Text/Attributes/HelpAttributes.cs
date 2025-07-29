namespace Volte.Systems.Commands.Text;

public enum VolteUnixCommand
{
    Announce,
    Zalgo,
    UnixBan
}

/// <summary>
///     Marker attribute for the base command of a group to act as a help page for that group.
///     Don't use this attribute for normal commands, it is used in the Help command logic for displaying groups properly.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class DummyCommandAttribute : Attribute;

/// <summary>
///     Signals the <see cref="HelpModule"/> to list all provided Unix-style arguments for commands that use them.
///     Don't use this on any commands unless they take a <see cref="Dictionary{String,String}"/> as its only argument.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class ShowUnixArgumentsInHelpAttribute : Attribute
{
    public VolteUnixCommand VolteUnixCommand { get; }

    public ShowUnixArgumentsInHelpAttribute(VolteUnixCommand command) => VolteUnixCommand = command;
}

/// <summary>
///     Signals the <see cref="HelpModule"/> to list all available placeholders for welcome messages.
///     Don't use this on any other commands, unless they use the same placeholders.
///     Placeholders are defined in <see cref="WelcomeOptions"/> as a static property.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class ShowPlaceholdersInHelpAttribute : Attribute;

/// <summary>
///     Signals the <see cref="HelpModule"/> to show example time formats for use in Reminders.
///     Don't use this on any commands; unless they have a parameter of type <see cref="TimeSpan"/>.
///     Formats can be viewed in a nerdy fashion in the file for <see cref="TimeSpanParser"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class ShowTimeFormatInHelpAttribute : Attribute;
    
/// <summary>
///     Signals the <see cref="HelpModule"/> to show the parent module's subcommands inside the help embed of a command.
///     Don't use on any commands unless they are in a module with subcommands, and it makes sense to use it.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class ShowSubcommandsInHelpOverrideAttribute : Attribute;