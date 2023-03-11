using events_manager_api.Domain.Entities;
using events_manager_api.Domain.EntityConfigurations;
using Microsoft.EntityFrameworkCore;

namespace events_manager_api.Domain.Repositories;

public class ApplicationDbContext : DbContext
{

    private readonly IConfiguration Configuration;

    public DbSet<DeveloperEntity> Developers { get; set; } = default!;
    public DbSet<EventEntity> Events { get; set; } = default!;
    public DbSet<InviteEntity> Invites { get; set; } = default!;

    public ApplicationDbContext(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfiguration(new DeveloperConfiguration());
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(Configuration.GetConnectionString("sqlConnection"));
    }
}