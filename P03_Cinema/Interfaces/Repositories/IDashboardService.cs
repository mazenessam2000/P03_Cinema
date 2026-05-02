namespace P03_Cinema.Interfaces.Repositories;

public interface IDashboardService
{
    Task<DashboardVM> GetDashboardAsync(CancellationToken ct);
}