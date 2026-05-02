namespace P03_Cinema.Interfaces.Services;

public interface ISeatMapService
{
    Task<SeatMapVM> GetSeatMapAsync(int showTimeId, string userId, CancellationToken ct);
}
