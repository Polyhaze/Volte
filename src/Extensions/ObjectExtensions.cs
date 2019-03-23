namespace Volte.Extensions
{
    public static class ObjectExtensions
    {
        public static T Cast<T>(this object obj) => obj is T o ? o : default(T);
    }
}