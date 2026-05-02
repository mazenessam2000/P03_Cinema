namespace P03_Cinema.Services;

public class SeatMapService(
    IShowTimeRepository showTimeRepo,
    ICartItemRepository cartItemRepo,
    IUnitOfWork uow) : ISeatMapService
{
    public async Task<SeatMapVM> GetSeatMapAsync(int showTimeId, string userId, CancellationToken ct)
    {
        var showTime = await showTimeRepo.GetWithSeatMapAsync(showTimeId, ct)
            ?? throw new KeyNotFoundException($"ShowTime {showTimeId} not found.");

        // ✅ Get FULL cart items (not just IDs)
        var userCartItems = await cartItemRepo
            .GetCartItemsByUserAndShowTimeAsync(userId, showTimeId, ct);

        // ✅ Release expired reservations
        var expiredLocks = showTime.ShowTimeSeats
            .Where(ss => ss.Status == SeatStatus.Reserved && ss.IsLockExpired)
            .ToList();

        foreach (var ss in expiredLocks)
        {
            ss.Status = SeatStatus.Available;
            ss.ReservedUntil = null;
            ss.ReservedByUserId = null;
        }

        if (expiredLocks.Count > 0)
            await uow.SaveChangesAsync(ct);

        // ✅ Map ShowTimeSeat by SeatId
        var seatStatusMap = showTime.ShowTimeSeats
            .ToDictionary(ss => ss.SeatId, ss => ss);

        var rows = showTime.Hall.Seats
            .GroupBy(s => s.RowLabel)
            .OrderBy(g => g.Key)
            .Select(g => new SeatRowVM
            {
                RowLabel = g.Key,

                Seats = g
                    .OrderBy(s => s.SeatNumber)
                    .Select(s =>
                    {
                        seatStatusMap.TryGetValue(s.Id, out var showTimeSeat);

                        var cartItem = showTimeSeat == null
                            ? null
                            : userCartItems.FirstOrDefault(ci => ci.ShowTimeSeatId == showTimeSeat.Id);

                        return new SeatCellVM
                        {
                            ShowTimeSeatId = showTimeSeat?.Id ?? 0,
                            SeatId = s.Id,

                            Label = $"{s.RowLabel}{s.SeatNumber}",

                            Type = s.Type,
                            Status = showTimeSeat?.Status ?? SeatStatus.Available,

                            IsInMyCart = cartItem != null,
                            CartItemId = cartItem?.Id,

                            IsReservedByMe = showTimeSeat?.ReservedByUserId == userId
                        };
                    })
                    .ToList()
            })
            .ToList();

        return new SeatMapVM
        {
            ShowTimeId = showTimeId,
            MovieName = showTime.Movie.Name,
            CinemaName = showTime.Cinema.Name,
            HallName = showTime.Hall.Name,
            StartTime = showTime.StartTime,
            PricePerSeat = showTime.Movie.Price,
            Rows = rows
        };
    }
}