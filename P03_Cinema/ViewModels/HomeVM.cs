namespace P03_Cinema.ViewModels;

public class HomeVM
{
    public List<MovieCardVM> PopularMovies { get; set; } = new();
    public List<MovieCardVM> TrendingMovies { get; set; } = new();
    public List<CategoryVM> Categories { get; set; } = new();
}