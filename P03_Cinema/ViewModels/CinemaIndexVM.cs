namespace P03_Cinema.ViewModels;

public class CinemaIndexVM
{
    public IEnumerable<Cinema> Cinemas { get; set; } = [];

    public Dictionary<int, int> ShowTimeCounts { get; set; }

    public string SearchQuery { get; set; } = string.Empty;
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
}