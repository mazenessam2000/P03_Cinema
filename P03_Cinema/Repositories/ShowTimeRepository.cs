using Microsoft.EntityFrameworkCore;

namespace P03_Cinema.Repositories;

public class ShowTimeRepository(ApplicationDbContext context) : IShowTimeRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<ShowTime?> GetWithSeatMapAsync(int showTimeId, CancellationToken ct = default)
    {
        return await _context.ShowTimes
            .Include(st => st.Movie)
            .Include(st => st.Cinema)
            .Include(st => st.Hall)
                .ThenInclude(h => h.Seats.Where(s => s.IsActive).OrderBy(s => s.RowNumber).ThenBy(s => s.SeatNumber))
            .Include(st => st.ShowTimeSeats)
                .ThenInclude(ss => ss.Seat)
            .FirstOrDefaultAsync(st => st.Id == showTimeId, ct);
    }
}