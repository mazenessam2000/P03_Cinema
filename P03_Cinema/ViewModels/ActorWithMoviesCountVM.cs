namespace P03_Cinema.ViewModels;

public class ActorWithMoviesCountVM
{
    public IEnumerable<Actor> Actors { get; set; } = new List<Actor>();
    public Dictionary<int, int> ActorMovieCounts { get; set; } = new();

    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
}