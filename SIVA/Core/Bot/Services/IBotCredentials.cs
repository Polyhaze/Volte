using System.Collections.Immutable;
using Microsoft.Extensions.Primitives;

namespace SIVA.Core.Bot.Services
{
    public interface IBotCredentials
    {
        string Token { get; }
        ulong ClientId { get; }
        ulong OwnerId { get; }
        
        DBConfig Db { get; }
    }

    public class DBConfig
    {
        public DBConfig(string type, string connectionString)
        {
            Type = type;
            ConnectionString = connectionString;
        }
        public string Type { get; }
        public string ConnectionString { get; }
    }
}