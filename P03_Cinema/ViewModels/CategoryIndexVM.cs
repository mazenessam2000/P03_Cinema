namespace P03_Cinema.ViewModels;

public class CategoryIndexVM
{
    public IEnumerable<Category> Categories { get; set; } = [];

    public string SearchQuery { get; set; } = string.Empty;
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
}