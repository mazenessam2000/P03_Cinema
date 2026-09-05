namespace P03_Cinema.ViewModels;

public class SeatRowVM
{
    public string RowLabel { get; set; } = null!;
    public List<SeatCellVM> Seats { get; set; } = [];
}
