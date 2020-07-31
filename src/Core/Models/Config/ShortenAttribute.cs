using System;

namespace BrackeysBot
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class ShortenAttribute : Attribute
    {
        public int Length { get; }
        
        public ShortenAttribute(int length)
        {
            Length = length;
        }
    }
}