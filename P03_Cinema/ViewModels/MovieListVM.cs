namespace P03_Cinema.ViewModels;

public class MovieListVM
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;

    public List<MovieCardVM> Movies { get; set; } = new();
}