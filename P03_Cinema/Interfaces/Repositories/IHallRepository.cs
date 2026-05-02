namespace P03_Cinema.Repositories;

public interface IHallRepository
{
    Task<Hall?> GetByIdAsync(int id, CancellationToken ct = default);

    Task<(List<HallRowVM> Items, int Total)> GetPageAsync(
        int page, int pageSize, string? q, CancellationToken ct = default);

    Task<List<Hall>> GetByCinemaAsync(int cinemaId, CancellationToken ct = default);

    Task<bool> HasActiveSeatsAsync(int hallId, CancellationToken ct = default);

    Task<List<int>> GetSeatIdsAsync(int hallId, CancellationToken ct = default);

    void Add(Hall hall);
    void Remove(Hall hall);
    void AddSeats(IEnumerable<Seat> seats);
    void RemoveShowTimeSeats(IEnumerable<int> seatIds);
    void RemoveSeats(int hallId);
}