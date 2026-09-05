namespace P03_Cinema.ViewModels;

// ─── Cart ───────────────────────────────────────────────────────────────────

public class CartVM
{
    public int CartId { get; set; }
    public List<CartItemVM> Items { get; set; } = [];
    public decimal Total => Items.Sum(i => i.Price);
    public int ItemCount => Items.Count;
}
