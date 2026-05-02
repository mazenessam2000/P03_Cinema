namespace P03_Cinema.Interfaces.Services;

public interface ICinemaService
{
    Task<CinemaIndexVM> GetCinemasPageAsync(int page, int pageSize, string q, CancellationToken ct);
    Task<CinemaCreateVM> GetCreateVMAsync(CancellationToken ct);
    Task CreateAsync(CinemaCreateVM vm, CancellationToken ct);

    Task<CinemaUpdateVM?> GetUpdateVMAsync(int id, CancellationToken ct);
    Task UpdateAsync(CinemaUpdateVM vm, CancellationToken ct);

    Task DeleteAsync(int id, CancellationToken ct);
}
