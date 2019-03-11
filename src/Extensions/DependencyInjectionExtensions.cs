using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using RestSharp.Extensions;
using Volte.Services;

namespace Volte.Extensions
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddVolteServices(this IServiceCollection provider)
        {
            //get all the classes that implement the IService interface, arent an interface themselves, and haven't been marked as Obsolete like the GitHubService.
            foreach (var service in Assembly.GetEntryAssembly().GetTypes()
                .Where(t => typeof(IService).IsAssignableFrom(t) && 
                            !t.IsInterface &&
                            t.GetAttribute<ObsoleteAttribute>() == null))
            {
                provider.AddSingleton(service);
            }

            return provider;
        }
    }
}