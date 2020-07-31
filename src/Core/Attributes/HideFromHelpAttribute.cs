using System;

namespace BrackeysBot
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public sealed class HideFromHelpAttribute : Attribute
    {
    }
}
