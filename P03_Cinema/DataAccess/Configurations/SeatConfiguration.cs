using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace P03_Cinema.DataAccess.Configurations;

public class SeatConfiguration : IEntityTypeConfiguration<Seat>
{
    public void Configure(EntityTypeBuilder<Seat> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.RowLabel)
            .IsRequired()
            .HasMaxLength(5);

        builder.Property(s => s.Type)
            .HasConversion<string>();

        builder.HasIndex(s => new { s.HallId, s.RowNumber, s.SeatNumber })
            .IsUnique();

        builder.HasOne(s => s.Hall)
            .WithMany(h => h.Seats)
            .HasForeignKey(s => s.HallId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}