using System;
using System.ComponentModel.DataAnnotations;

namespace SIVA.Core.Bot.Services.Database.DbTypes
{
    public class DatabaseEntity
    {
        [Key] 
        public int Id { get; set; }
        public DateTime? DateAdded { get; set; } = DateTime.UtcNow;
    }
}