using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace P03_Cinema.Utility.DbInitializers;

public class DbInitializer : IDbInitializer
{
    private readonly ApplicationDbContext _context;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<DbInitializer> _logger;

    public DbInitializer(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, ApplicationDbContext context, ILogger<DbInitializer> logger)
    {
        _roleManager = roleManager;
        _userManager = userManager;
        _context = context;
        _logger = logger;
    }

    public async Task Initialize()
    {
        try
        {
            if (_context.Database.GetPendingMigrations().Any())
                _context.Database.Migrate();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
        }

        // ================= ROLES =================
        if (!_roleManager.Roles.Any())
        {
            await _roleManager.CreateAsync(new IdentityRole(SD.SUPER_ADMIN_ROLE));
            await _roleManager.CreateAsync(new IdentityRole(SD.ADMIN_ROLE));
            await _roleManager.CreateAsync(new IdentityRole(SD.EMPLOYEE_ROLE));
            await _roleManager.CreateAsync(new IdentityRole(SD.CUSTOMER_ROLE));
        }

        // ================= SUPER ADMIN =================
        if (await _userManager.FindByEmailAsync("SuperAdmin@cinemaverse.com") == null)
        {
            var admin = new ApplicationUser
            {
                Email = "SuperAdmin@cinemaverse.com",
                UserName = "SuperAdmin",
                FirstName = "Super",
                LastName = "Admin",
                EmailConfirmed = true
            };

            await _userManager.CreateAsync(admin, "Admin123$");
            await _userManager.AddToRoleAsync(admin, SD.SUPER_ADMIN_ROLE);
        }

        // Prevent reseeding
        if (_context.Cinemas.Any())
            return;

        // ================= CATEGORIES =================
        var categories = new List<Category>
    {
        new() { Name = "Action", ImageUrl = "action.jpg", IsActive = true },
        new() { Name = "Comedy", ImageUrl = "comedy.jpg", IsActive = true },
        new() { Name = "Drama", ImageUrl = "drama.jpg", IsActive = true }
    };
        _context.Categories.AddRange(categories);

        // ================= ACTORS =================
        var actors = new List<Actor>
    {
        new() { FullName = "Leonardo DiCaprio", ImageUrl = "leo.jpg" },
        new() { FullName = "Tom Cruise", ImageUrl = "tom.jpg" },
        new() { FullName = "Scarlett Johansson", ImageUrl = "scarlett.jpg" }
    };
        _context.Actors.AddRange(actors);

        // ================= MOVIES =================
        var movies = new List<Movie>
    {
        new()
        {
            Name = "Inception",
            Price = 50,
            DurationMinutes = 148,
            ReleaseDate = DateTime.UtcNow.AddYears(-10),
            Status = "Now Showing",
            MainImage = "inception.jpg"
        },
        new()
        {
            Name = "Top Gun Maverick",
            Price = 60,
            DurationMinutes = 130,
            ReleaseDate = DateTime.UtcNow.AddYears(-2),
            Status = "Now Showing",
            MainImage = "topgun.jpg"
        }
    };
        _context.Movies.AddRange(movies);

        // ================= CINEMAS =================
        var cinema = new Cinema
        {
            Name = "CinemaVerse Downtown",
            Location = "Main Street",
            ImageUrl = "cinema1.jpg",
            Rate = 8.5,
            IsActive = true
        };
        _context.Cinemas.Add(cinema);

        await _context.SaveChangesAsync();

        // ================= HALLS =================
        var halls = new List<Hall>
    {
        new() { Name = "Hall 1", CinemaId = cinema.Id, TotalRows = 5, SeatsPerRow = 10 },
        new() { Name = "Hall 2", CinemaId = cinema.Id, TotalRows = 6, SeatsPerRow = 8 }
    };
        _context.Halls.AddRange(halls);

        await _context.SaveChangesAsync();

        // ================= SEATS =================
        var seats = new List<Seat>();

        foreach (var hall in halls)
        {
            for (int r = 1; r <= hall.TotalRows; r++)
            {
                for (int s = 1; s <= hall.SeatsPerRow; s++)
                {
                    seats.Add(new Seat
                    {
                        HallId = hall.Id,
                        RowNumber = r,
                        RowLabel = $"{(char)(64 + r)}{s}",
                        SeatNumber = s,
                        Type = s % 5 == 0 ? SeatType.VIP : SeatType.Regular
                    });
                }
            }
        }

        _context.Seats.AddRange(seats);
        await _context.SaveChangesAsync();

        // ================= SHOWTIMES =================
        var showTimes = new List<ShowTime>();

        foreach (var hall in halls)
        {
            foreach (var movie in movies)
            {
                showTimes.Add(new ShowTime
                {
                    MovieId = movie.Id,
                    CinemaId = cinema.Id,
                    HallId = hall.Id,
                    StartTime = DateTime.UtcNow.AddHours(2),
                    AvailableSeats = hall.TotalSeats
                });
            }
        }

        _context.ShowTimes.AddRange(showTimes);
        await _context.SaveChangesAsync();

        // ================= SHOWTIME SEATS =================
        var showTimeSeats = new List<ShowTimeSeat>();

        foreach (var st in showTimes)
        {
            var hallSeats = seats.Where(s => s.HallId == st.HallId);

            foreach (var seat in hallSeats)
            {
                showTimeSeats.Add(new ShowTimeSeat
                {
                    ShowTimeId = st.Id,
                    SeatId = seat.Id
                });
            }
        }

        _context.ShowTimeSeats.AddRange(showTimeSeats);
        await _context.SaveChangesAsync();
    }
}