using System;
using System.Linq.Expressions;

namespace Volte.Core.Helpers
{
    public static class ExpressionHelper
    {
        public static Func<T, TResult> MemberInstance<T, TResult>(string name)
        {
            var instance = Expression.Parameter(typeof(T), "instance");
            
            return Expression.Lambda<Func<T, TResult>>(
                Expression.PropertyOrField(instance, name) // return instance.name
                , instance // (instance) => ...
            ).Compile();
        }
    }
}