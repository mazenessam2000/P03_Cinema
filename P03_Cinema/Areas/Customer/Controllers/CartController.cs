using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace P03_Cinema.Areas.Customer.Controllers;

[Area("Customer")]
[Authorize]
public class CartController(ICartService cartService, ISeatMapService seatMapService) : Controller
{
    private readonly ICartService _cartService = cartService;
    private readonly ISeatMapService _seatMapService = seatMapService;

    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    // ─── Seat Map ─────────────────────────────────────────────────────────────

    [HttpGet]
    public async Task<IActionResult> SelectSeats(int showTimeId, CancellationToken ct)
    {
        try
        {
            var vm = await _seatMapService.GetSeatMapAsync(showTimeId, UserId, ct);
            return View(vm);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    // ─── Cart CRUD ────────────────────────────────────────────────────────────

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var vm = await _cartService.GetCartAsync(UserId, ct);
        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> AddSeat(int showTimeSeatId, int showTimeId, CancellationToken ct)
    {
        try
        {
            await _cartService.AddSeatAsync(UserId, showTimeSeatId, ct);
            TempData["Success"] = "Seat added to cart!";
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(SelectSeats), new { showTimeId });
    }

    [HttpPost]
    public async Task<IActionResult> RemoveSeat(int cartItemId, CancellationToken ct)
    {
        try
        {
            await _cartService.RemoveSeatAsync(UserId, cartItemId, ct);
            TempData["Success"] = "Seat removed from cart.";
        }
        catch (KeyNotFoundException)
        {
            TempData["Error"] = "Item not found.";
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Clear(CancellationToken ct)
    {
        await _cartService.ClearCartAsync(UserId, ct);
        TempData["Success"] = "Cart cleared.";
        return RedirectToAction(nameof(Index));
    }

    // ─── Checkout ─────────────────────────────────────────────

    [HttpGet]
    public async Task<IActionResult> Checkout(CancellationToken ct)
    {
        var cart = await _cartService.GetCartAsync(UserId, ct);

        if (!cart.Items.Any())
        {
            TempData["Error"] = "Your cart is empty.";
            return RedirectToAction(nameof(Index));
        }

        return View(new CheckoutVM { Cart = cart });
    }

    [HttpPost]
    public async Task<IActionResult> ConfirmCheckout(CancellationToken ct)
    {
        try
        {
            var baseUrl = $"{Request.Scheme}://{Request.Host}";

            var sessionUrl = await _cartService.CreateCheckoutSessionAsync(UserId, baseUrl, ct);

            // ✅ Redirect to Stripe
            return Redirect(sessionUrl);
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction(nameof(Checkout));
        }
    }

    [HttpGet("checkout/success")]
    public async Task<IActionResult> Success(CancellationToken ct)
    {
        var vm = await _cartService.FinalizeBookingAsync(UserId, ct);
        return View("Confirmation", vm);
    }

    [HttpGet("checkout/cancel")]
    public IActionResult Cancel()
    {
        TempData["Error"] = "Payment cancelled.";
        return RedirectToAction(nameof(Index));
    }
}
