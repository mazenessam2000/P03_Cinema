using Microsoft.EntityFrameworkCore;
using P03_Cinema.DataAccess.Configurations;

namespace P03_Cinema.DataAccess;

public class ApplicationDbContext : DbContext
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
    public DbSet<CinemaMovie> CinemaMovies { get; set; }
    //public DbSet<ShowTime> ShowTimes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ActorConfiguration).Assembly);
    }
}