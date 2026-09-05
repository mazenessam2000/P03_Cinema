namespace P03_Cinema.ViewModels;

public class MovieIndexVM
{
    public IEnumerable<Movie> Movies { get; set; } = new List<Movie>();

    public Dictionary<int, List<string>> MovieActors { get; set; } = new();
    public Dictionary<int, List<string>> MovieCategories { get; set; } = new();
    public Dictionary<int, List<string>> MovieImages { get; set; } = new();

    public string SearchQuery { get; set; } = string.Empty;
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
}