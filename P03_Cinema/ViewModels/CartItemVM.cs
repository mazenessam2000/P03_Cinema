namespace P03_Cinema.ViewModels;

public class CartItemVM
{
    public int CartItemId { get; set; }
    public int ShowTimeSeatId { get; set; }
    public string MovieName { get; set; } = null!;
    public string CinemaName { get; set; } = null!;
    public string HallName { get; set; } = null!;
    public string SeatLabel { get; set; } = null!;
    public SeatType SeatType { get; set; }
    public DateTime ShowTime { get; set; }
    public decimal Price { get; set; }
}
