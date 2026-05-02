namespace P03_Cinema.Interfaces.Repositories;

public interface IShowTimeRepository
{
    Task<ShowTime?> GetWithSeatMapAsync(int showTimeId, CancellationToken ct = default);
}