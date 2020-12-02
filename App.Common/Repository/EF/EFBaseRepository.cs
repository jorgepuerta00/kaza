using App.Common.Repository.Abstractions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace App.Common.Repository.EF
{
    /// <summary>
    /// https://www.c-sharpcorner.com/article/net-entity-framework-core-generic-async-operations-with-unit-of-work-generic-re/
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EFBaseRepository<T> : IRepository<T> where T : BaseEntity
    {
        private readonly DbContext _context;
        private readonly DbSet<T> _entities = null;

        public EFBaseRepository(DbContext context)
        {
            _context = context;
            _entities = _context.Set<T>();
        }

        public void Add(T entity)
        {
            _context.Add(entity);
        }

        public async Task<T> FindAsync(Guid Id)
        {
            return await _entities.FindAsync(Id);
        }
        public void Update(T entity)
        {
            _entities.Update(entity);
        }
    }
}
