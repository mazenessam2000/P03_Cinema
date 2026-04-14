using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace P03_Cinema.DataAccess.Configurations;

public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {

        builder.HasKey(b => b.Id);

        builder.HasOne(b => b.ShowTime)
            .WithMany(st => st.Bookings)
            .HasForeignKey(b => b.ShowTimeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
