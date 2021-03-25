using System;
using Volte.Commands.Modules;

namespace Volte.Core.Entities
{
    /// <summary>
    ///     Signals the <see cref="HelpModule"/> to list all available placeholders for welcome messages.
    ///     Don't use this on any other commands, unless they use the same placeholders.
    ///     Placeholders are defined in <see cref="WelcomeOptions"/>> as a static property.
    /// </summary>
    public class ShowPlaceholdersInHelpAttribute : Attribute { }
}