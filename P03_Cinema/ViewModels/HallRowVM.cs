namespace P03_Cinema.ViewModels;

public class HallRowVM
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string CinemaName { get; set; } = null!;
    public int TotalRows { get; set; }
    public int SeatsPerRow { get; set; }
    public int TotalSeats { get; set; }
    public bool IsActive { get; set; }
}