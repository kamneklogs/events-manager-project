using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace events_manager_api.Domain.Repositories;

public interface IRepository<T> where T : class, new()
{
    T? Get(String id);
    IQueryable<T> FindWhere(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);
    Task<IEnumerable<T>> GetAllAsync();
    T Add(T entity);
    void Update(T entity);
}