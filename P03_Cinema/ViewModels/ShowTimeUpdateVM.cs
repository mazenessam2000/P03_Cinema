namespace P03_Cinema.ViewModels;

public class ShowTimeUpdateVM
{
    public int Id { get; set; }
    public int MovieId { get; set; }
    public int CinemaId { get; set; }
    public int HallId { get; set; }
    public DateTime StartTime { get; set; }

    public List<Movie> Movies { get; set; } = [];
    public List<Cinema> Cinemas { get; set; } = [];
    public List<Hall> Halls { get; set; } = [];
}
