using events_manager_api.Domain.Repositories;
using events_manager_api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using events_manager_api.Domain.Repositories.Impl;

namespace events_manager_api.Domain.UnitOfWork.Impl;

public class UnitOfWork : IUnitOfWork
{

    private readonly DbContext _context;

    private IRepository<DeveloperEntity>? _developerRepository;
    public IRepository<DeveloperEntity> DeveloperRepository => _developerRepository ??= new Repository<DeveloperEntity>(_context);

    private IRepository<EventEntity>? _eventRepository;
    public IRepository<EventEntity> EventRepository => _eventRepository ??= new Repository<EventEntity>(_context);


    private IRepository<InviteEntity>? _inviteRepository;
    public IRepository<InviteEntity> InviteRepository => _inviteRepository ??= new Repository<InviteEntity>(_context);

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Complete() => await _context.SaveChangesAsync() > 0;

    public void Dispose() => _context.Dispose();
}