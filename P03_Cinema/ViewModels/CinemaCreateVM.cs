namespace P03_Cinema.ViewModels;

public class CinemaCreateVM
{
    public Cinema Cinema { get; set; } = new Cinema();
    public IEnumerable<Movie> AllMovies { get; set; } = new List<Movie>();
    public List<int> SelectedMovieIds { get; set; } = new List<int>();

    public IFormFile? Img { get; set; }
}