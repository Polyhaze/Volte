using System.Threading.Tasks;

namespace Volte
{
    internal static class Program
    {
        private static async Task Main()
        {
            await Core.VolteBot.StartAsync();
        }
    }
}