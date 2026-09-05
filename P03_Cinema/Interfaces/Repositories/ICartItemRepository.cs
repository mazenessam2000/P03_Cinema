namespace P03_Cinema.Interfaces.Repositories;

public interface ICartItemRepository
{
    Task<CartItem?> GetWithCartAndSeatAsync(int cartItemId, string userId, CancellationToken ct = default);

    Task<HashSet<int>> GetShowTimeSeatIdsByUserAndShowTimeAsync(string userId, int showTimeId, CancellationToken ct = default);

    Task<List<CartItem>> GetByShowTimeSeatIdsAsync(IEnumerable<int> showTimeSeatIds, CancellationToken ct = default);

    Task<List<CartItem>> GetCartItemsByUserAndShowTimeAsync(string userId, int showTimeId, CancellationToken ct);

    void Add(CartItem cartItem);
    void Remove(CartItem cartItem);
    void RemoveRange(IEnumerable<CartItem> cartItems);
}