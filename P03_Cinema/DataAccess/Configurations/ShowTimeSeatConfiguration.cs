using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace P03_Cinema.DataAccess.Configurations;

public class ShowTimeSeatConfiguration : IEntityTypeConfiguration<ShowTimeSeat>
{
    public void Configure(EntityTypeBuilder<ShowTimeSeat> builder)
    {
        builder.HasKey(ss => ss.Id);

        builder.Property(ss => ss.Status)
            .HasConversion<string>()
            .HasDefaultValue(SeatStatus.Available);

        builder.HasIndex(ss => new { ss.ShowTimeId, ss.SeatId })
            .IsUnique();

        builder.HasOne(ss => ss.ShowTime)
            .WithMany(st => st.ShowTimeSeats)
            .HasForeignKey(ss => ss.ShowTimeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ss => ss.Seat)
            .WithMany(s => s.ShowTimeSeats)
            .HasForeignKey(ss => ss.SeatId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(ss => ss.ReservedByUser)
            .WithMany()
            .HasForeignKey(ss => ss.ReservedByUserId)
            .OnDelete(DeleteBehavior.SetNull)
            .IsRequired(false);
    }
}