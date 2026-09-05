namespace P03_Cinema.Interfaces.Services;
public interface IMovieHomeService
{
    Task<List<MovieCardVM>> GetPopularMoviesAsync();
    Task<List<MovieCardVM>> GetTrendingMoviesAsync();
    Task<List<MovieCardVM>> GetAllMoviesAsync();
    Task<List<MovieCardVM>> GetMoviesByCategoryAsync(int categoryId);
    Task<MovieDetailsVM?> GetMovieDetailsAsync(int id);
}