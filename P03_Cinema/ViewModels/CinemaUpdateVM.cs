namespace P03_Cinema.ViewModels;

public class CinemaUpdateVM
{
    public Cinema Cinema { get; set; } = new();

    public IEnumerable<Movie> AllMovies { get; set; } = new List<Movie>();

    public List<int> SelectedMovieIds { get; set; } = new List<int>();

    public IFormFile? Img { get; set; }
}