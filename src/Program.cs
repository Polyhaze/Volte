using System.Threading.Tasks;
using Discord;
 
using Volte.Core;

namespace Volte
{
    internal static class Program
    {
        private static Task Main()
        {
            return VolteBot.StartAsync();
        }
    }
}