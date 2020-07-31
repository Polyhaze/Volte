using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

using BrackeysBot.Services;

namespace BrackeysBot
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddBrackeysBotServices(this IServiceCollection col)
        {
            foreach (var service in GetServiceTypes())
            {
                col.TryAddSingleton(service);
            }
            return col;
        }
        public static void InitializeServices(this IServiceProvider provider)
        {
            foreach (var service in GetServiceTypes().Where(t => typeof(IInitializeableService).IsAssignableFrom(t)))
            {
                object instance = provider.GetService(service);
                (instance as IInitializeableService).Initialize();
            }
        }

        private static IEnumerable<Type> GetServiceTypes()
            => Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => !t.HasAttribute<ObsoleteAttribute>() && t.Inherits<BrackeysBotService>() && !t.IsAbstract);
    }
}
