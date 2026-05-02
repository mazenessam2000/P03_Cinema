using Stripe.Checkout;

namespace P03_Cinema.Services;

public class CartService(
    ICartRepository cartRepo,
    ICartItemRepository cartItemRepo,
    IShowTimeSeatRepository showTimeSeatRepo,
    IBookingRepository bookingRepo,
    IUnitOfWork uow) : ICartService
{
    private static readonly TimeSpan ReservationWindow = TimeSpan.FromMinutes(10);
    private readonly ICartRepository _cartRepo = cartRepo;
    private readonly ICartItemRepository _cartItemRepo = cartItemRepo;
    private readonly IShowTimeSeatRepository _showTimeSeatRepo = showTimeSeatRepo;
    private readonly IBookingRepository _bookingRepo = bookingRepo;
    private readonly IUnitOfWork _uow = uow;

    public async Task<CartVM> GetCartAsync(string userId, CancellationToken ct)
    {
        var cart = await _cartRepo.GetWithFullDetailsAsync(userId, ct);

        if (cart == null)
            return new CartVM { Items = [] };

        return new CartVM
        {
            CartId = cart.Id,
            Items = cart.CartItems.Select(ci => new CartItemVM
            {
                CartItemId = ci.Id,
                ShowTimeSeatId = ci.ShowTimeSeatId,
                MovieName = ci.ShowTimeSeat.ShowTime.Movie.Name,
                CinemaName = ci.ShowTimeSeat.Seat.Hall.Cinema.Name,
                HallName = ci.ShowTimeSeat.Seat.Hall.Name,
                SeatLabel = ci.ShowTimeSeat.Seat.RowLabel,
                SeatType = ci.ShowTimeSeat.Seat.Type,
                ShowTime = ci.ShowTimeSeat.ShowTime.StartTime,
                Price = ci.Price
            }).ToList()
        };
    }

    public async Task<int> GetCartCountAsync(string userId, CancellationToken ct = default)
    {
        return await _cartRepo.GetCartCountAsync(userId, ct);
    }

    public async Task AddSeatAsync(string userId, int showTimeSeatId, CancellationToken ct)
    {
        var showTimeSeat = await _showTimeSeatRepo.GetWithShowTimeAndMovieAsync(showTimeSeatId, ct)
            ?? throw new KeyNotFoundException("Seat not found.");

        // Release expired lock first
        if (showTimeSeat.IsLockExpired)
        {
            showTimeSeat.Status = SeatStatus.Available;
            showTimeSeat.ReservedUntil = null;
            showTimeSeat.ReservedByUserId = null;
        }

        if (showTimeSeat.Status != SeatStatus.Available)
            throw new InvalidOperationException("This seat is no longer available.");

        // Get or create cart
        var cart = await _cartRepo.GetWithItemsAsync(userId, ct);
        if (cart == null)
        {
            cart = new Cart { UserId = userId };
            _cartRepo.Add(cart);
            await _uow.SaveChangesAsync(ct);
        }

        // Check not already in cart
        if (cart.CartItems.Any(ci => ci.ShowTimeSeatId == showTimeSeatId))
            throw new InvalidOperationException("Seat already in cart.");

        // Reserve the seat
        showTimeSeat.Status = SeatStatus.Reserved;
        showTimeSeat.ReservedUntil = DateTime.UtcNow.Add(ReservationWindow);
        showTimeSeat.ReservedByUserId = userId;

        _cartItemRepo.Add(new CartItem
        {
            CartId = cart.Id,
            ShowTimeSeatId = showTimeSeatId,
            Price = showTimeSeat.ShowTime.Movie.Price
        });

        await _uow.SaveChangesAsync(ct);
    }

    public async Task RemoveSeatAsync(string userId, int cartItemId, CancellationToken ct)
    {
        var cartItem = await _cartItemRepo.GetWithCartAndSeatAsync(cartItemId, userId, ct)
            ?? throw new KeyNotFoundException("Cart item not found.");

        var seat = cartItem.ShowTimeSeat;
        if (seat.ReservedByUserId == userId)
        {
            seat.Status = SeatStatus.Available;
            seat.ReservedUntil = null;
            seat.ReservedByUserId = null;
        }

        _cartItemRepo.Remove(cartItem);
        await _uow.SaveChangesAsync(ct);
    }

    public async Task ClearCartAsync(string userId, CancellationToken ct)
    {
        var cart = await _cartRepo.GetWithItemsAndSeatsAsync(userId, ct);
        if (cart == null) return;

        foreach (var item in cart.CartItems)
        {
            var seat = item.ShowTimeSeat;
            if (seat.ReservedByUserId == userId)
            {
                seat.Status = SeatStatus.Available;
                seat.ReservedUntil = null;
                seat.ReservedByUserId = null;
            }
        }

        _cartItemRepo.RemoveRange(cart.CartItems);
        await _uow.SaveChangesAsync(ct);
    }

    public async Task<string> CreateCheckoutSessionAsync(string userId, string baseUrl, CancellationToken ct)
    {
        var cart = await _cartRepo.GetWithFullDetailsAsync(userId, ct)
            ?? throw new InvalidOperationException("Cart is empty.");

        if (!cart.CartItems.Any())
            throw new InvalidOperationException("Cart is empty.");

        // Ensure all seats belong to same showtime
        var showTimeIds = cart.CartItems
            .Select(ci => ci.ShowTimeSeat.ShowTimeId)
            .Distinct()
            .ToList();

        if (showTimeIds.Count > 1)
            throw new InvalidOperationException("All seats must belong to the same showtime.");

        // Validate reservation still valid
        foreach (var item in cart.CartItems)
        {
            var ss = item.ShowTimeSeat;

            if (ss.Status != SeatStatus.Reserved ||
                ss.ReservedByUserId != userId ||
                ss.IsLockExpired)
            {
                throw new InvalidOperationException($"Seat {ss.Seat.RowLabel} reservation expired.");
            }
        }

        var showTimeId = showTimeIds[0];
        var total = cart.CartItems.Sum(i => i.Price);

        var options = new SessionCreateOptions
        {
            PaymentMethodTypes = new List<string> { "card" },
            Mode = "payment",

            LineItems = cart.CartItems.Select(item => new SessionLineItemOptions
            {
                Quantity = 1,
                PriceData = new SessionLineItemPriceDataOptions
                {
                    Currency = "usd", // or model-based
                    UnitAmount = (long)(item.Price * 100), // Stripe expects cents
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = $"Seat {item.ShowTimeSeat.Seat.RowLabel}{item.ShowTimeSeat.Seat.RowNumber}",
                        Description = item.ShowTimeSeat.ShowTime.Movie.Name
                    }
                }
            }).ToList(),

            SuccessUrl = $"{baseUrl}/checkout/success",
            CancelUrl = $"{baseUrl}/checkout/cancel",
        };

        var service = new SessionService();
        var session = service.Create(options);

        return session.Url;
    }

    public async Task ReleaseExpiredReservationsAsync(CancellationToken ct)
    {
        var expired = await _showTimeSeatRepo.GetExpiredReservationsAsync(ct);

        foreach (var ss in expired)
        {
            ss.Status = SeatStatus.Available;
            ss.ReservedUntil = null;
            ss.ReservedByUserId = null;
        }

        var expiredSeatIds = expired.Select(ss => ss.Id).ToList();
        var orphanedCartItems = await _cartItemRepo.GetByShowTimeSeatIdsAsync(expiredSeatIds, ct);
        _cartItemRepo.RemoveRange(orphanedCartItems);

        await _uow.SaveChangesAsync(ct);
    }
    public async Task<BookingConfirmationVM> FinalizeBookingAsync(string userId, CancellationToken ct)
    {
        var cart = await _cartRepo.GetWithFullDetailsAsync(userId, ct);

        if (cart == null || !cart.CartItems.Any())
            throw new InvalidOperationException("Cart is empty or already processed.");

        var firstItem = cart.CartItems.FirstOrDefault();

        if (firstItem == null)
            throw new InvalidOperationException("No items found in cart.");

        var showTimeId = firstItem.ShowTimeSeat.ShowTimeId;
        var total = cart.CartItems.Sum(i => i.Price);

        var booking = new Booking
        {
            ShowTimeId = showTimeId,
            UserId = userId,
            TotalPrice = total,
            BookingDate = DateTime.UtcNow
        };

        _bookingRepo.Add(booking);
        await _uow.SaveChangesAsync(ct);

        var seatLabels = new List<string>();

        foreach (var item in cart.CartItems)
        {
            var ss = item.ShowTimeSeat;

            ss.Status = SeatStatus.Booked;
            ss.ReservedUntil = null;
            ss.ReservedByUserId = null;

            _bookingRepo.AddSeat(new BookingSeat
            {
                BookingId = booking.Id,
                ShowTimeSeatId = ss.Id,
                PricePaid = item.Price
            });

            seatLabels.Add(ss.Seat.RowLabel);
        }

        _cartItemRepo.RemoveRange(cart.CartItems);
        await _uow.SaveChangesAsync(ct);

        return new BookingConfirmationVM
        {
            BookingId = booking.Id,
            MovieName = firstItem.ShowTimeSeat.ShowTime.Movie.Name,
            CinemaName = firstItem.ShowTimeSeat.Seat.Hall.Cinema.Name,
            HallName = firstItem.ShowTimeSeat.Seat.Hall.Name,
            ShowTime = firstItem.ShowTimeSeat.ShowTime.StartTime,
            SeatLabels = seatLabels,
            TotalPaid = total,
            BookingDate = booking.BookingDate
        };
    }
}