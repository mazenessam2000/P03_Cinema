using Microsoft.EntityFrameworkCore;

namespace P03_Cinema.Repositories;

public class CartItemRepository(ApplicationDbContext context) : ICartItemRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<CartItem?> GetWithCartAndSeatAsync(int cartItemId, string userId, CancellationToken ct = default)
    {
        return await _context.CartItems
            .Include(ci => ci.Cart)
            .Include(ci => ci.ShowTimeSeat)
            .FirstOrDefaultAsync(ci => ci.Id == cartItemId && ci.Cart.UserId == userId, ct);
    }

    public async Task<List<CartItem>> GetCartItemsByUserAndShowTimeAsync(string userId, int showTimeId, CancellationToken ct)
    {
        return await _context.CartItems
            .Where(ci => ci.Cart.UserId == userId && ci.ShowTimeSeat.ShowTimeId == showTimeId)
            .ToListAsync(ct);
    }

    public async Task<HashSet<int>> GetShowTimeSeatIdsByUserAndShowTimeAsync(string userId, int showTimeId, CancellationToken ct = default)
    {
        return await _context.CartItems
            .Where(ci => ci.Cart.UserId == userId && ci.ShowTimeSeat.ShowTimeId == showTimeId)
            .Select(ci => ci.ShowTimeSeatId)
            .ToHashSetAsync(ct);
    }

    public async Task<List<CartItem>> GetByShowTimeSeatIdsAsync(IEnumerable<int> showTimeSeatIds, CancellationToken ct = default)
    {
        return await _context.CartItems
            .Where(ci => showTimeSeatIds.Contains(ci.ShowTimeSeatId))
            .ToListAsync(ct);
    }

    public void Add(CartItem cartItem) => _context.CartItems.Add(cartItem);
    public void Remove(CartItem cartItem) => _context.CartItems.Remove(cartItem);
    public void RemoveRange(IEnumerable<CartItem> cartItems) => _context.CartItems.RemoveRange(cartItems);
}