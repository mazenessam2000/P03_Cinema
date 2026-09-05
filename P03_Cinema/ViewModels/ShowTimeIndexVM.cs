namespace P03_Cinema.ViewModels;

public class ShowTimeIndexVM
{
    public IEnumerable<ShowTime> ShowTimes { get; set; } = new List<ShowTime>();

    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }

    public string SearchQuery { get; set; } = string.Empty;
}