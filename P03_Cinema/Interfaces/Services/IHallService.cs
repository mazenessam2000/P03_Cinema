namespace P03_Cinema.Interfaces.Services;

public interface IHallService
{
    Task<HallIndexVM> GetHallsPageAsync(int page, int pageSize, string q, CancellationToken ct);
    Task<HallCreateVM> GetCreateVMAsync(CancellationToken ct);
    Task<HallUpdateVM?> GetUpdateVMAsync(int id, CancellationToken ct);
    Task CreateAsync(HallCreateVM vm, CancellationToken ct);
    Task UpdateAsync(HallUpdateVM vm, CancellationToken ct);
    Task DeleteAsync(int id, CancellationToken ct);
    Task<List<Hall>> GetByCinemaAsync(int cinemaId, CancellationToken ct);
}
