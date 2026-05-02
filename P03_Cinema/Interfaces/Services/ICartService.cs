namespace P03_Cinema.Interfaces.Services;

public interface ICartService
{
    /// <summary>Get or create the cart for the logged-in user.</summary>
    Task<CartVM> GetCartAsync(string userId, CancellationToken ct);

    /// <summary>Add a seat to the user's cart and temporarily reserve it.</summary>
    Task AddSeatAsync(string userId, int showTimeSeatId, CancellationToken ct);

    /// <summary>Remove a seat from the cart and release the reservation.</summary>
    Task RemoveSeatAsync(string userId, int cartItemId, CancellationToken ct);

    /// <summary>Clear the entire cart and release all reservations.</summary>
    Task ClearCartAsync(string userId, CancellationToken ct);

    Task<string> CreateCheckoutSessionAsync(string userId, string baseUrl, CancellationToken ct);

    Task ReleaseExpiredReservationsAsync(CancellationToken ct);

    Task<int> GetCartCountAsync(string userId, CancellationToken ct = default);

    Task<BookingConfirmationVM> FinalizeBookingAsync(string userId, CancellationToken ct);

}
