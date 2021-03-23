using System;

namespace Volte.Core.Entities
{
    /// <summary>
    ///     Used on a base command of a command group; for Help command usage. Don't use this attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class DummyCommandAttribute : Attribute { }
}