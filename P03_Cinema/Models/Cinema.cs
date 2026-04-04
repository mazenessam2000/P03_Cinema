namespace P03_Cinema.Models;

public class Cinema
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Location { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;

    public double Rate { get; set; }
    public bool IsActive { get; set; }

    public ICollection<CinemaMovie> CinemaMovies { get; set; } = [];
    //public ICollection<ShowTime> ShowTimes { get; set; } = null!;
}