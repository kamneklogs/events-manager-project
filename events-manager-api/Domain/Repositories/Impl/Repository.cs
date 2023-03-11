using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace events_manager_api.Domain.Repositories.Impl;
public class Repository<T> : IRepository<T> where T : class, new()
{

    protected readonly DbContext _context;

    public Repository(DbContext context)
    {
        _context = context;
    }

    public EntityEntry<T> Add(T entity) => _context.Set<T>().Add(entity);

    public void AddRange(IEnumerable<T> entities) => _context.Set<T>().AddRange(entities);

    public IQueryable<T> Find(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes) {
//=> await _context.Set<T>().FirstOrDefaultAsync(predicate) ?? Task.FromResult(new T()).Result;
        var query = _context.Set<T>().Where(predicate);
        return includes.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
    }

    public async Task<T> GetAsync(string id) => await _context.FindAsync<T>(id) ?? Task.FromResult(new T()).Result;

    public async Task<IEnumerable<T>> GetAllAsync() => await _context.Set<T>().ToListAsync();

    public IQueryable<T> GetWhere(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
    {
        var query = _context.Set<T>().Where(predicate);
        return includes.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
    }

    public void Remove(T entity) => _context.Set<T>().Remove(entity);

    public void RemoveRange(IEnumerable<T> entities) => _context.Set<T>().RemoveRange(entities);

    public void Update(T entity) => _context.Set<T>().Update(entity);

    public void UpdateRange(IEnumerable<T> entities) => _context.Set<T>().UpdateRange(entities);

    public IEnumerable<T> Include(params Expression<Func<T, object>>[] includes)
    {
        DbSet<T> dbSet = _context.Set<T>();

        IEnumerable<T> query = default!;
        foreach (var include in includes)
        {
            query = dbSet.Include(include);
        }

        return query ?? dbSet;
    }
}