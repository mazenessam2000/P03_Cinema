using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using P03_Cinema.DataAccess.Configurations;

namespace P03_Cinema.DataAccess;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Movie> Movies { get; set; }
    public DbSet<Actor> Actors { get; set; }
    public DbSet<Cinema> Cinemas { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<MovieImage> MovieImages { get; set; }
    public DbSet<MovieActor> MovieActors { get; set; }
    public DbSet<MovieCategory> MovieCategories { get; set; }
    public DbSet<ShowTime> ShowTimes { get; set; }
    public DbSet<ApplicationUserOTP> ApplicationUserOTPs { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<Hall> Halls { get; set; }
    public DbSet<Seat> Seats { get; set; }
    public DbSet<ShowTimeSeat> ShowTimeSeats { get; set; }
    public DbSet<Cart> Carts { get; set; }
    public DbSet<CartItem> CartItems { get; set; }
    public DbSet<BookingSeat> BookingSeats { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ActorConfiguration).Assembly);
    }
}