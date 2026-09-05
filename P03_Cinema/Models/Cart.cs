namespace P03_Cinema.Models;

public class Cart
{
    public int Id { get; set; }

    public string UserId { get; set; } = null!;
    public ApplicationUser User { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<CartItem> CartItems { get; set; } = [];

    public decimal Total => CartItems.Sum(i => i.Price);
}