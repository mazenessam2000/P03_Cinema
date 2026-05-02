using Microsoft.EntityFrameworkCore;

namespace P03_Cinema.Repositories;

public class CartRepository(ApplicationDbContext context) : ICartRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<Cart?> GetWithFullDetailsAsync(string userId, CancellationToken ct = default)
    {
        return await _context.Carts
            .Include(c => c.CartItems)
                .ThenInclude(ci => ci.ShowTimeSeat)
                    .ThenInclude(ss => ss.Seat)
                        .ThenInclude(s => s.Hall)
                            .ThenInclude(h => h.Cinema)
            .Include(c => c.CartItems)
                .ThenInclude(ci => ci.ShowTimeSeat)
                    .ThenInclude(ss => ss.ShowTime)
                        .ThenInclude(st => st.Movie)
            .FirstOrDefaultAsync(c => c.UserId == userId, ct);
    }

    public async Task<int> GetCartCountAsync(string userId, CancellationToken ct = default)
    {
        return await _context.CartItems
            .Where(ci => ci.Cart.UserId == userId)
            .CountAsync(ct);
    }

    public async Task<Cart?> GetWithItemsAndSeatsAsync(string userId, CancellationToken ct = default)
    {
        return await _context.Carts
            .Include(c => c.CartItems)
                .ThenInclude(ci => ci.ShowTimeSeat)
            .FirstOrDefaultAsync(c => c.UserId == userId, ct);
    }

    public async Task<Cart?> GetWithItemsAsync(string userId, CancellationToken ct = default)
    {
        return await _context.Carts
            .Include(c => c.CartItems)
            .FirstOrDefaultAsync(c => c.UserId == userId, ct);
    }

    public void Add(Cart cart) => _context.Carts.Add(cart);
}