using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace events_manager_api.Domain.Repositories;

public interface IRepository<T> where T : class, new()
{
    Task<T> GetAsync(String id);
    IQueryable<T> Find(Expression<Func<T, bool>> predicate,params Expression<Func<T, object>>[] includes);
    Task<IEnumerable<T>> GetAllAsync();
    IQueryable<T> GetWhere(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);
    EntityEntry<T> Add(T entity);
    void AddRange(IEnumerable<T> entities);
    void Update(T entity);
    void UpdateRange(IEnumerable<T> entities);
    void Remove(T entity);
    void RemoveRange(IEnumerable<T> entities);

    IEnumerable<T> Include(params Expression<Func<T, object>>[] includes);
}