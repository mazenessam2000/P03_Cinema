namespace P03_Cinema.ViewModels;

// ─── Checkout / Booking ──────────────────────────────────────────────────────

public class CheckoutVM
{
    public CartVM Cart { get; set; } = null!;
    // Could add promo code, payment info, etc. later
}
