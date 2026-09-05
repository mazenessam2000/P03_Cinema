namespace P03_Cinema.Interfaces.Services;

public interface IShowTimeService
{
    Task<ShowTimeIndexVM> GetAllAsync(int page, int pageSize, string q, CancellationToken ct);

    Task<ShowTimeCreateVM> GetCreateVMAsync(CancellationToken ct);
    Task CreateAsync(ShowTimeCreateVM vm, CancellationToken ct);

    Task<ShowTimeUpdateVM?> GetUpdateVMAsync(int id, CancellationToken ct);
    Task UpdateAsync(ShowTimeUpdateVM vm, CancellationToken ct);

    Task DeleteAsync(int id, CancellationToken ct);
}