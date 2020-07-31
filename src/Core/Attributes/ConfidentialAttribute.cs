using System;

namespace BrackeysBot
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public sealed class ConfidentialAttribute : Attribute
    {
    }
}