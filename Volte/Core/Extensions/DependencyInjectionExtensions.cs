using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Volte.Core.Services;

namespace Volte.Core.Extensions
{
    public static class DependencyInjectionExtensions
    {

        public static IServiceCollection AddVolteServices(this IServiceCollection provider)
        {
            foreach (var service in Assembly.GetEntryAssembly().GetTypes()
                .Where(t => typeof(IService).IsAssignableFrom(t) && !t.IsInterface))
            {
                provider.AddSingleton(service);
            }

            return provider;
        }

    }
}
