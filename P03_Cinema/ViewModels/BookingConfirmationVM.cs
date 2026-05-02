namespace P03_Cinema.ViewModels;

public class BookingConfirmationVM
{
    public int BookingId { get; set; }
    public string MovieName { get; set; } = null!;
    public string CinemaName { get; set; } = null!;
    public string HallName { get; set; } = null!;
    public DateTime ShowTime { get; set; }
    public List<string> SeatLabels { get; set; } = [];
    public decimal TotalPaid { get; set; }
    public DateTime BookingDate { get; set; }
}
