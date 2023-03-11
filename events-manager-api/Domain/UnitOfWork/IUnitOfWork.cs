using events_manager_api.Domain.Repositories;
using events_manager_api.Domain.Entities;

namespace events_manager_api.Domain.UnitOfWork;

public interface IUnitOfWork : IDisposable
{
    IRepository<DeveloperEntity> DeveloperRepository { get; }
    IRepository<EventEntity> EventRepository { get; }
    IRepository<InviteEntity> InviteRepository { get; }
    Task<bool> Complete();
}