using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace P03_Cinema.DataAccess.Configurations;

public class BookingSeatConfiguration : IEntityTypeConfiguration<BookingSeat>
{
    public void Configure(EntityTypeBuilder<BookingSeat> builder)
    {
        builder.HasKey(bs => bs.Id);

        builder.Property(bs => bs.PricePaid)
            .HasColumnType("decimal(18,2)");

        builder.HasOne(bs => bs.Booking)
            .WithMany(b => b.BookingSeats)
            .HasForeignKey(bs => bs.BookingId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(bs => bs.ShowTimeSeat)
            .WithMany(ss => ss.BookingSeats)
            .HasForeignKey(bs => bs.ShowTimeSeatId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
