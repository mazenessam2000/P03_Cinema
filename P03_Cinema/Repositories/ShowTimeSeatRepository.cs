using Microsoft.EntityFrameworkCore;

namespace P03_Cinema.Repositories;

public class ShowTimeSeatRepository(ApplicationDbContext context) : IShowTimeSeatRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<ShowTimeSeat?> GetWithShowTimeAndMovieAsync(int showTimeSeatId, CancellationToken ct = default)
    {
        return await _context.ShowTimeSeats
            .Include(ss => ss.ShowTime)
                .ThenInclude(st => st.Movie)
            .FirstOrDefaultAsync(ss => ss.Id == showTimeSeatId, ct);
    }

    public async Task<List<ShowTimeSeat>> GetExpiredReservationsAsync(CancellationToken ct = default)
    {
        return await _context.ShowTimeSeats
            .Where(ss => ss.Status == SeatStatus.Reserved &&
                         ss.ReservedUntil.HasValue &&
                         ss.ReservedUntil < DateTime.UtcNow)
            .ToListAsync(ct);
    }

    public void Add(BookingSeat bookingSeat) => _context.BookingSeats.Add(bookingSeat);
}