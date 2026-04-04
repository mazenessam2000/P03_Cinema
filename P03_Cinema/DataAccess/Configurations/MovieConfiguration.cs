using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace P03_Cinema.DataAccess.Configurations;

public class MovieConfiguration : IEntityTypeConfiguration<Movie>
{
    public void Configure(EntityTypeBuilder<Movie> builder)
    {
        builder.Property(m => m.Name)
               .HasMaxLength(200);

        builder.Property(m => m.Description)
               .HasMaxLength(1000);

        builder.Property(m => m.Price)
               .HasColumnType("decimal(10,2)");

        builder.Property(m => m.MainImage)
               .HasMaxLength(250);

        builder.Property(m => m.Status)
               .HasMaxLength(50);
    }
}
