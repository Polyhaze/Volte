using System.Threading.Tasks;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using Volte.Core.Services;

namespace Volte.Core.Modules.Owner {
    public class DatabaseCommand : VolteCommand {
        
        [Command("dbtest")]
        public async Task DbTest() {
            await Reply(Context.Channel, Db.GetConfig(Context.Guild).ServerId.ToString());
        }
        
    }
}