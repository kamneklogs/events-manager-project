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

    public T Add(T entity) => _context.Set<T>().Add(entity).Entity;

    public IQueryable<T> FindWhere(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
    {
        var query = _context.Set<T>().Where(predicate);
        return includes.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
    }

    public T? Get(string id) =>  _context.FindAsync<T>(id).Result;

    public async Task<IEnumerable<T>> GetAllAsync() => await _context.Set<T>().ToListAsync();

    public void Update(T entity) => _context.Set<T>().Update(entity);

}