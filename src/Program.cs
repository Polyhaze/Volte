using System.Threading.Tasks;
using Volte.Core;

namespace Volte
{
    public static class Program
    {
        internal static async Task Main()
        {
            await VolteBot.StartAsync();
        }
    }
}