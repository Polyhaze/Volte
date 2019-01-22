using Discord;
using LiteDB;
using SIVA.Core.Files.Objects;
using SIVA.Core.Files.Readers;

namespace SIVA.Core.Services {
    public class DatabaseService {
        public Server GetConfig(IGuild guild) {
            using (var db = new LiteDatabase("data/SIVA.db")) {
                var conf = db.GetCollection<Server>("serverconfigs").FindOne(g => g.ServerId == guild.Id);
                if (conf == null) {
                    var newconf = ServerConfig.Create(guild);
                    db.GetCollection<Server>("serverconfigs").Insert(newconf);
                    db.Dispose();
                }
                    return conf;
                
            }
        }
    }
}