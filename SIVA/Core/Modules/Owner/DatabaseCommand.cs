using System.Threading.Tasks;
using Discord.Commands;
using SIVA.Core.Services;

namespace SIVA.Core.Modules.Owner {
    public class DatabaseCommand : SIVACommand {

        public DatabaseService DbService { get; }
        
        [Command("dbtest")]
        public async Task DbTest() {
            await Reply(Context.Channel, DbService.GetConfig(Context.Guild).ToString());
        }
        
    }
}