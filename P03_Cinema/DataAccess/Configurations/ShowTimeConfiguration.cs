using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace P03_Cinema.DataAccess.Configurations;

public class ShowTimeConfiguration : IEntityTypeConfiguration<ShowTime>
{
    public void Configure(EntityTypeBuilder<ShowTime> builder)
    {
        builder.HasKey(st => st.Id);

        builder.HasOne(st => st.Movie)
            .WithMany(m => m.ShowTimes)
            .HasForeignKey(st => st.MovieId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(st => st.Cinema)
            .WithMany(c => c.ShowTimes)
            .HasForeignKey(st => st.CinemaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(st => st.Hall)
            .WithMany(h => h.ShowTimes)
            .HasForeignKey(st => st.HallId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(st => st.ShowTimeSeats)
            .WithOne(ss => ss.ShowTime)
            .HasForeignKey(ss => ss.ShowTimeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}