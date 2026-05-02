using Microsoft.EntityFrameworkCore;

namespace P03_Cinema.Repositories;

public class HallRepository(ApplicationDbContext context) : IHallRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<Hall?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _context.Halls.FindAsync([id], ct);

    public async Task<(List<HallRowVM> Items, int Total)> GetPageAsync(
        int page, int pageSize, string? q, CancellationToken ct = default)
    {
        var query = _context.Halls
            .Include(h => h.Cinema)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(q))
            query = query.Where(h => h.Name.Contains(q) || h.Cinema.Name.Contains(q));

        var total = await query.CountAsync(ct);

        var items = await query
            .OrderBy(h => h.Cinema.Name).ThenBy(h => h.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(h => new HallRowVM
            {
                Id = h.Id,
                Name = h.Name,
                CinemaName = h.Cinema.Name,
                TotalRows = h.TotalRows,
                SeatsPerRow = h.SeatsPerRow,
                TotalSeats = h.TotalRows * h.SeatsPerRow,
                IsActive = h.IsActive
            })
            .ToListAsync(ct);

        return (items, total);
    }

    public async Task<List<Hall>> GetByCinemaAsync(int cinemaId, CancellationToken ct = default)
        => await _context.Halls
            .Where(h => h.CinemaId == cinemaId && h.IsActive)
            .OrderBy(h => h.Name)
            .ToListAsync(ct);

    public async Task<bool> HasActiveSeatsAsync(int hallId, CancellationToken ct = default)
        => await _context.ShowTimeSeats
            .AnyAsync(ss => ss.Seat.HallId == hallId && ss.Status != SeatStatus.Available, ct);

    public async Task<List<int>> GetSeatIdsAsync(int hallId, CancellationToken ct = default)
        => await _context.Seats
            .Where(s => s.HallId == hallId)
            .Select(s => s.Id)
            .ToListAsync(ct);

    public void Add(Hall hall) => _context.Halls.Add(hall);
    public void Remove(Hall hall) => _context.Halls.Remove(hall);

    public void AddSeats(IEnumerable<Seat> seats) => _context.Seats.AddRange(seats);

    public void RemoveShowTimeSeats(IEnumerable<int> seatIds)
    {
        var orphaned = _context.ShowTimeSeats.Where(ss => seatIds.Contains(ss.SeatId));
        _context.ShowTimeSeats.RemoveRange(orphaned);
    }

    public void RemoveSeats(int hallId)
    {
        var seats = _context.Seats.Where(s => s.HallId == hallId);
        _context.Seats.RemoveRange(seats);
    }
}