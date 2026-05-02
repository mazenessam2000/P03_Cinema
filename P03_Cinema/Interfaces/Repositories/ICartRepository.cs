namespace P03_Cinema.Interfaces.Repositories;

public interface ICartRepository
{
    Task<Cart?> GetWithFullDetailsAsync(string userId, CancellationToken ct = default);

    Task<Cart?> GetWithItemsAndSeatsAsync(string userId, CancellationToken ct = default);

    Task<Cart?> GetWithItemsAsync(string userId, CancellationToken ct = default);

    Task<int> GetCartCountAsync(string userId, CancellationToken ct = default);
    void Add(Cart cart);
}