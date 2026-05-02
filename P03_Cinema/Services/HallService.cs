using Microsoft.EntityFrameworkCore;

namespace P03_Cinema.Services;

public class HallService(
    IHallRepository hallRepo,
    IRepository<Cinema> cinemaRepo,
    IUnitOfWork uow) : IHallService
{
    public async Task<HallIndexVM> GetHallsPageAsync(int page, int pageSize, string q, CancellationToken ct)
    {
        var (halls, total) = await hallRepo.GetPageAsync(page, pageSize, q, ct);

        return new HallIndexVM
        {
            Halls = halls,
            CurrentPage = page,
            TotalPages = (int)Math.Ceiling(total / (double)pageSize),
            Query = q
        };
    }

    public async Task<HallCreateVM> GetCreateVMAsync(CancellationToken ct)
    {
        return new HallCreateVM
        {
            Cinemas = await cinemaRepo.Get()
                .Where(c => c.IsActive)
                .OrderBy(c => c.Name)
                .ToListAsync(ct)
        };
    }

    public async Task<HallUpdateVM?> GetUpdateVMAsync(int id, CancellationToken ct)
    {
        var hall = await hallRepo.GetByIdAsync(id, ct);
        if (hall == null) return null;

        return new HallUpdateVM
        {
            Id = hall.Id,
            Name = hall.Name,
            CinemaId = hall.CinemaId,
            TotalRows = hall.TotalRows,
            SeatsPerRow = hall.SeatsPerRow,
            IsActive = hall.IsActive,
            Cinemas = await cinemaRepo.Get()
                .Where(c => c.IsActive)
                .OrderBy(c => c.Name)
                .ToListAsync(ct)
        };
    }

    public async Task CreateAsync(HallCreateVM vm, CancellationToken ct)
    {
        var hall = new Hall
        {
            Name = vm.Name,
            CinemaId = vm.CinemaId,
            TotalRows = vm.TotalRows,
            SeatsPerRow = vm.SeatsPerRow,
            IsActive = vm.IsActive
        };

        hallRepo.Add(hall);
        await uow.SaveChangesAsync(ct);

        await GenerateSeatsAsync(hall, ct);
    }

    public async Task UpdateAsync(HallUpdateVM vm, CancellationToken ct)
    {
        var hall = await hallRepo.GetByIdAsync(vm.Id, ct)
            ?? throw new KeyNotFoundException($"Hall {vm.Id} not found.");

        var rowsChanged = hall.TotalRows != vm.TotalRows || hall.SeatsPerRow != vm.SeatsPerRow;

        hall.Name = vm.Name;
        hall.CinemaId = vm.CinemaId;
        hall.TotalRows = vm.TotalRows;
        hall.SeatsPerRow = vm.SeatsPerRow;
        hall.IsActive = vm.IsActive;

        if (rowsChanged)
        {
            if (await hallRepo.HasActiveSeatsAsync(hall.Id, ct))
                throw new InvalidOperationException("Cannot resize hall with active/booked seats.");

            // Remove ShowTimeSeats first to avoid orphaned FK records, then seats
            var oldSeatIds = await hallRepo.GetSeatIdsAsync(hall.Id, ct);
            hallRepo.RemoveShowTimeSeats(oldSeatIds);
            hallRepo.RemoveSeats(hall.Id);

            await uow.SaveChangesAsync(ct);
            await GenerateSeatsAsync(hall, ct);
        }
        else
        {
            await uow.SaveChangesAsync(ct);
        }
    }

    public async Task DeleteAsync(int id, CancellationToken ct)
    {
        var hall = await hallRepo.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException($"Hall {id} not found.");

        hallRepo.Remove(hall);
        await uow.SaveChangesAsync(ct);
    }

    public async Task<List<Hall>> GetByCinemaAsync(int cinemaId, CancellationToken ct)
        => await hallRepo.GetByCinemaAsync(cinemaId, ct);

    // ─── Private helpers ─────────────────────────────────────────────────────

    private async Task GenerateSeatsAsync(Hall hall, CancellationToken ct)
    {
        var seats = new List<Seat>();
        for (int row = 1; row <= hall.TotalRows; row++)
        {
            string rowLabel = GetRowLabel(row);
            for (int seatNum = 1; seatNum <= hall.SeatsPerRow; seatNum++)
            {
                seats.Add(new Seat
                {
                    HallId = hall.Id,
                    RowNumber = row,
                    RowLabel = rowLabel,
                    SeatNumber = seatNum,
                    Type = SeatType.Regular,
                    IsActive = true
                });
            }
        }

        hallRepo.AddSeats(seats);
        await uow.SaveChangesAsync(ct);
    }

    private static string GetRowLabel(int rowNumber)
    {
        string label = string.Empty;
        while (rowNumber > 0)
        {
            rowNumber--;
            label = (char)('A' + rowNumber % 26) + label;
            rowNumber /= 26;
        }
        return label;
    }
}