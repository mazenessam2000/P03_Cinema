namespace P03_Cinema.ViewModels;

public class MovieDetailsVM
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string MainImage { get; set; }
    public decimal Price { get; set; }
    public int DurationMinutes { get; set; }
    public string Status { get; set; }
    public DateTime ReleaseDate { get; set; }

    public List<string> Categories { get; set; } = new();
    public List<ActorVM> Actors { get; set; } = new();

    // NEW
    public List<string> Images { get; set; } = new();
    public List<ShowTimeVM> ShowTimes { get; set; } = new();
}