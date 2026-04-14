using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace P03_Cinema.DataAccess.Configurations;

public class ShowTimeConfiguration : IEntityTypeConfiguration<ShowTime>
{
    public void Configure(EntityTypeBuilder<ShowTime> builder)
    {
        builder.HasKey(st => st.Id);

        // ================= MOVIE =================
        builder.HasOne(st => st.Movie)
            .WithMany(m => m.ShowTimes)
            .HasForeignKey(st => st.MovieId)
            .OnDelete(DeleteBehavior.Restrict);

        // ================= CINEMA =================
        builder.HasOne(st => st.Cinema)
            .WithMany(c => c.ShowTimes)
            .HasForeignKey(st => st.CinemaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.MovieId, x.CinemaId, x.StartTime })
       .IsUnique();
    }
}