using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace P03_Cinema.DataAccess.Configurations;

public class HallConfiguration : IEntityTypeConfiguration<Hall>
{
    public void Configure(EntityTypeBuilder<Hall> builder)
    {
        builder.HasKey(h => h.Id);

        builder.Property(h => h.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasOne(h => h.Cinema)
            .WithMany(c => c.Halls)
            .HasForeignKey(h => h.CinemaId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(h => h.Seats)
            .WithOne(s => s.Hall)
            .HasForeignKey(s => s.HallId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(h => h.ShowTimes)
            .WithOne(st => st.Hall)
            .HasForeignKey(st => st.HallId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Ignore(h => h.TotalSeats); // computed property
    }
}
