using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace P03_Cinema.DataAccess.Configurations;

public class CinemaConfiguration : IEntityTypeConfiguration<Cinema>
{
    public void Configure(EntityTypeBuilder<Cinema> builder)
    {
        builder.Property(c => c.Name)
               .HasMaxLength(150);

        builder.Property(c => c.Location)
               .HasMaxLength(250);

        builder.Property(c => c.ImageUrl)
               .HasMaxLength(250);

        builder.Property(c => c.Rate)
               .HasColumnType("float");
    }
}
