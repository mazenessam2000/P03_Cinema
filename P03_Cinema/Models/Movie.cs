namespace P03_Cinema.Models;

public class Movie
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }

    public DateTime ReleaseDate { get; set; }

    public string? MainImage { get; set; }
    public string Status { get; set; } = string.Empty;

    public ICollection<CinemaMovie> CinemaMovies { get; set; } = [];
    public ICollection<MovieImage> Images { get; set; } = [];
    public ICollection<MovieActor> MovieActors { get; set; } = [];
    public ICollection<MovieCategory> MovieCategories { get; set; } = [];
    //public ICollection<ShowTime> ShowTimes { get; set; } = null!;
}