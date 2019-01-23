using System.Threading.Tasks;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using SIVA.Core.Services;

namespace SIVA.Core.Modules.Owner {
    public class DatabaseCommand : SIVACommand {

        public DatabaseService DbService { get; set; }
        
        [Command("dbtest")]
        public async Task DbTest() {
            await Reply(Context.Channel, DbService.GetConfig(Context.Guild).ServerId.ToString());
        }
        
    }
}