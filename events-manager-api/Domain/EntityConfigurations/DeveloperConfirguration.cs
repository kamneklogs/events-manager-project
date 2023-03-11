using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using events_manager_api.Domain.Entities;

namespace events_manager_api.Domain.EntityConfigurations;

public class DeveloperConfiguration : IEntityTypeConfiguration<DeveloperEntity>
{
    public void Configure(EntityTypeBuilder<DeveloperEntity> builder)
    {
        builder.HasKey(e => e.Email);
    }
}
