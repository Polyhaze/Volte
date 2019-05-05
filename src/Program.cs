using System.Threading.Tasks;

namespace Volte
{
    internal static class Program
    {
        private static async Task Main()
        {
            await Discord.VolteBot.StartAsync();
        }
    }
}