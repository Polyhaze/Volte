using System.Threading.Tasks;
using Discord;
using Volte.Discord;

namespace Volte
{
    internal static class Program
    {
        private static async Task Main()
        {
            await VolteBot.StartAsync();
        }
    }
}