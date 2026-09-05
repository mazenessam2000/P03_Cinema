using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace P03_Cinema.Services;

public class DashboardService : IDashboardService
{
    private readonly IRepository<Booking> _bookingRepo;
    private readonly IRepository<ShowTimeSeat> _seatRepo;
    private readonly IRepository<ShowTime> _showRepo;

    public DashboardService(
        IRepository<Booking> bookingRepo,
        IRepository<ShowTimeSeat> seatRepo,
        IRepository<ShowTime> showRepo)
    {
        _bookingRepo = bookingRepo;
        _seatRepo = seatRepo;
        _showRepo = showRepo;
    }

    public async Task<DashboardVM> GetDashboardAsync(CancellationToken ct)
    {
        var bookings = await _bookingRepo.Get()
            .Include(b => b.BookingSeats)
                .ThenInclude(bs => bs.ShowTimeSeat)
                    .ThenInclude(st => st.ShowTime)
                        .ThenInclude(s => s.Movie)
            .ToListAsync(ct);

        var seats = await _seatRepo.Get().ToListAsync(ct);

        var showtimes = await _showRepo.Get()
            .Include(s => s.Movie)
            .Include(s => s.Hall)
            .Include(s => s.ShowTimeSeats)
            .ToListAsync(ct);

        // ── Tickets & Revenue ─────────────────────────────
        var ticketsSold = bookings.Sum(b => b.BookingSeats.Count);
        var revenue = bookings.Sum(b => b.TotalPrice);

        // ── Seats ─────────────────────────────
        var availableSeats = seats.Count(s => s.Status == SeatStatus.Available);

        // ── Movies ─────────────────────────────
        var activeMovies = showtimes.Select(s => s.MovieId).Distinct().Count();

        // ── Weekly Sales ─────────────────────────────
        var weeklyGroups = bookings
            .Where(b => b.BookingDate >= DateTime.UtcNow.AddDays(-7))
            .GroupBy(b => b.BookingDate.DayOfWeek)
            .Select(g => new
            {
                Day = g.Key.ToString(),
                Count = g.Sum(x => x.BookingSeats.Count)
            })
            .ToList();

        var weeklyLabels = weeklyGroups.Select(x => x.Day).ToList();
        var weeklySales = weeklyGroups.Select(x => x.Count).ToList();

        // ── Movie Stats ─────────────────────────────
        var movieGroups = bookings
            .SelectMany(b => b.BookingSeats)
            .GroupBy(bs => bs.ShowTimeSeat.ShowTime.Movie.Name)
            .Select(g => new
            {
                Movie = g.Key,
                Count = g.Count()
            })
            .ToList();

        var movieNames = movieGroups.Select(x => x.Movie).ToList();
        var movieSales = movieGroups.Select(x => x.Count).ToList();

        // ── Showtimes ─────────────────────────────
        var showtimeVMs = showtimes.Select(s => new ShowTimeDashboardVM
        {
            ShowTimeId = s.Id,
            MovieName = s.Movie?.Name ?? "N/A",
            HallName = s.Hall?.Name ?? "N/A",

            StartTime = s.StartTime,
            EndTime = s.StartTime.AddHours(2), // fallback if you don't store EndTime

            Status = GetStatus(s.StartTime),
            Progress = CalculateProgress(s),

            TotalSeats = s.ShowTimeSeats.Count,
            BookedSeats = s.ShowTimeSeats.Count(x => x.Status == SeatStatus.Booked),

            Revenue = bookings
                .Where(b => b.BookingSeats.Any(bs => bs.ShowTimeSeat.ShowTimeId == s.Id))
                .Sum(b => b.TotalPrice)
        }).ToList();

        return new DashboardVM
        {
            TicketsSold = ticketsSold,
            Revenue = revenue,
            AvailableSeats = availableSeats,
            ActiveMovies = activeMovies,

            WeeklyLabels = weeklyLabels,
            WeeklySales = weeklySales,

            MovieNames = movieNames,
            MovieSales = movieSales,

            Showtimes = showtimeVMs,
            Alerts = new List<string>(),
            Activities = new List<ActivityVM>()
        };
    }

    private int CalculateProgress(ShowTime s)
    {
        var now = DateTime.UtcNow;

        if (now < s.StartTime) return 0;

        var total = s.Movie.DurationMinutes;

        var passed = (now - s.StartTime).TotalMinutes;

        return (int)Math.Clamp((passed / total) * 100, 0, 100);
    }

    private string GetStatus(DateTime start)
    {
        var now = DateTime.UtcNow;

        if (now < start) return "Upcoming";
        if (now > start.AddHours(2)) return "Finished";
        return "Running";
    }
}