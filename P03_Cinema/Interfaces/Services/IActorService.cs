namespace P03_Cinema.Interfaces.Services;

public interface IActorService
{
    Task<ActorIndexVM> GetActorsPageAsync(int page, int pageSize, string q, CancellationToken ct);
    Task AddAsync(ActorCreateVM vm, CancellationToken ct);
    Task UpdateAsync(ActorUpdateVM vm, CancellationToken ct);
    Task DeleteAsync(int id, CancellationToken ct);
    Task<Actor?> FindByIdAsync(int id, CancellationToken ct);
}
