using Microsoft.EntityFrameworkCore;
using SIVA.Core.Bot.Services.Database.DbTypes;
using System.Collections.Generic;
using System.Linq;

namespace SIVA.Core.Bot.Services.Database.Repositories.Impl
{
    public class Repository<T> : IRepository<T> where T : DatabaseEntity
    {
        protected DbContext _context;
        protected DbSet<T> _set;

        public Repository(DbContext context)
        {
            _context = context;
            _set = context.Set<T>();
        }

        public void Add(T obj) =>
            _set.Add(obj);

        public void AddRange(params T[] objs) =>
            _set.AddRange(objs);

        public T Get(int id) =>
            _set.FirstOrDefault(e => e.Id == id);

        public IEnumerable<T> GetAll() =>
            _set.ToList();

        public void Remove(int id) =>
            _set.Remove(Get(id));

        public void Remove(T obj) =>
            _set.Remove(obj);

        public void RemoveRange(params T[] objs) =>
            _set.RemoveRange(objs);

        public void Update(T obj) =>
            _set.Update(obj);

        public void UpdateRange(params T[] objs) =>
            _set.UpdateRange(objs);
    }
}