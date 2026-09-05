namespace P03_Cinema.Interfaces.Services;

public interface IMovieService
{
    Task<MovieIndexVM> GetMoviesPageAsync(int page, int pageSize, string q, CancellationToken ct);
    Task CreateAsync(MovieCreateVM vm, CancellationToken ct);
    Task<MovieCreateVM> GetCreateVmAsync(CancellationToken ct);
    Task FillCreateVmAsync(MovieCreateVM vm, CancellationToken ct);
    Task UpdateAsync(MovieUpdateVM vm, CancellationToken ct);
    Task<MovieUpdateVM?> GetUpdateVMAsync(int id, CancellationToken ct);
    Task FillUpdateVMAsync(MovieUpdateVM vm, CancellationToken ct);
    Task DeleteImageAsync(int id, CancellationToken ct);
    Task DeleteMovieAsync(int id, CancellationToken ct);
}
