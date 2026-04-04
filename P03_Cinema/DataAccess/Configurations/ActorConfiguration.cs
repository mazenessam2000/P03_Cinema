using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace P03_Cinema.DataAccess.Configurations;

public class ActorConfiguration : IEntityTypeConfiguration<Actor>
{
    public void Configure(EntityTypeBuilder<Actor> builder)
    {
        builder.Property(a => a.FullName)
               .HasMaxLength(150);

        builder.Property(a => a.Bio)
               .HasMaxLength(1000);

        builder.Property(a => a.ImageUrl)
               .HasMaxLength(250);
    }
}