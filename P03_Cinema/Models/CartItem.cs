namespace P03_Cinema.Models;

public class CartItem
{
    public int Id { get; set; }

    public int CartId { get; set; }
    public Cart Cart { get; set; } = null!;

    public int ShowTimeSeatId { get; set; }
    public ShowTimeSeat ShowTimeSeat { get; set; } = null!;

    public decimal Price { get; set; }

    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
}