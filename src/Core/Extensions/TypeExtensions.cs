using System;
using System.Reflection;

namespace BrackeysBot
{
    public static class TypeExtensions
    {
        public static bool HasAttribute<T>(this Type t) where T : Attribute
            => t.GetCustomAttribute<T>() != null;
        public static bool Inherits<T>(this Type t)
            => typeof(T).IsAssignableFrom(t);
    }
}
