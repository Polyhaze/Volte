using System;
using static System.Linq.Expressions.Expression;

namespace Volte.Core.Helpers
{
    public static class ExpressionHelper
    {
        public static Func<T, TResult> GetMemberInstance<T, TResult>(string name)
        {
            var instance = Parameter(typeof(T), "instance");
            
            return Lambda<Func<T, TResult>>(
                PropertyOrField(instance, name) // return instance.name
                , instance // (instance) => ...
            ).Compile();
        }

        public static Func<T> Constructor0Args<T>()
        {
            return Lambda<Func<T>>(
                New(typeof(T)) // return new T()
            ).Compile(); // () => ...
        }
        
        public static Action<T, TValue> SetMemberInstance<T, TValue>(string name) where T : class
        {
            var instance = Parameter(typeof(T), "instance");
            var value = Parameter(typeof(TValue), "value");

            return Lambda<Action<T, TValue>>(
                Assign(PropertyOrField(instance, name), value) // instance.name = value
                , instance, value).Compile(); // (instance, value) => ...
        }

    }
}