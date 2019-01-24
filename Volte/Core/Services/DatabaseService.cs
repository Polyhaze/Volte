using Discord;
using LiteDB;
using Volte.Core.Discord;
using Volte.Core.Files.Objects;
using Volte.Core.Files.Readers;

namespace Volte.Core.Services {
    public class DatabaseService {
        public Server GetConfig(IGuild guild) {
            return GetConfig(guild.Id);
        }
        public Server GetConfig(ulong id) {
            using (var db = new LiteDatabase("data/SIVA.db")) {
                var conf = db.GetCollection<Server>("serverconfigs").FindOne(g => g.ServerId == id);
                if (conf == null) {
                    var newconf = ServerConfig.Create(VolteBot.Client.GetGuild(id));
                    db.GetCollection<Server>("serverconfigs").Insert(newconf);
                    db.Dispose();
                }
                return conf;
                
            }
        }
        
        //public void InsertIntoConfig(ulong id, )
    }
}