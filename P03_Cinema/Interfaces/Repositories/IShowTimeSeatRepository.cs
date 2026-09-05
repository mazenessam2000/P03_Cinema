namespace P03_Cinema.Interfaces.Repositories;

public interface IShowTimeSeatRepository
{
    Task<ShowTimeSeat?> GetWithShowTimeAndMovieAsync(int showTimeSeatId, CancellationToken ct = default);

    Task<List<ShowTimeSeat>> GetExpiredReservationsAsync(CancellationToken ct = default);

    void Add(BookingSeat bookingSeat);
}