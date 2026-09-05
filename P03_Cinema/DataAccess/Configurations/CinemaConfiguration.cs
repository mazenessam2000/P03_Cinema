using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

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
               .HasColumnType("decimal(3,1)");

        builder.HasMany(c => c.Halls)
               .WithOne(h => h.Cinema)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.ShowTimes)
               .WithOne(s => s.Cinema)
               .HasForeignKey(s => s.CinemaId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
