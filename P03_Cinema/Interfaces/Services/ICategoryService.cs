namespace P03_Cinema.Interfaces.Services;

public interface ICategoryService
{
    Task<CategoryIndexVM> GetCategoriesPageAsync(int page, int pageSize, string q, CancellationToken ct);
    Task AddAsync(CategoryCreateVM vm, CancellationToken ct);
    Task UpdateAsync(CategoryUpdateVM vm, CancellationToken ct);
    Task DeleteAsync(int id, CancellationToken ct);
    Task<Category?> FindByIdAsync(int id, CancellationToken ct);
}