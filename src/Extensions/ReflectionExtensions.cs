using System;
using System.Reflection;
using RestSharp.Extensions;

namespace Volte.Extensions
{
    public static class ReflectionExtensions
    {

        public static bool HasAttribute<T>(this TypeInfo type) where T : Attribute
            => type.GetAttribute<T>() != null;

        public static bool HasAttribute<T>(this Type type) where T : Attribute
            => type.GetAttribute<T>() != null;

    }
}
