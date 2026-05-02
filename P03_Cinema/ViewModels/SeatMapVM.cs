namespace P03_Cinema.ViewModels;

public class SeatMapVM
{
    public int ShowTimeId { get; set; }
    public string MovieName { get; set; } = null!;
    public string CinemaName { get; set; } = null!;
    public string HallName { get; set; } = null!;
    public DateTime StartTime { get; set; }
    public decimal PricePerSeat { get; set; }

    public List<SeatRowVM> Rows { get; set; } = [];
}