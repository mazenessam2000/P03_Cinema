namespace P03_Cinema.ViewModels;

public class CinemaWithMoviesCountAndPagesVM
{
    public IEnumerable<Cinema> Cinemas { get; set; } = null!;
    public Dictionary<int, int> CinemaMovieCounts { get; set; } = new();

    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
}
