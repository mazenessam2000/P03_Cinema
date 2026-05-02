using Microsoft.EntityFrameworkCore;

namespace P03_Cinema.Services;

public class CategoryHomeService : ICategoryHomeService
{
    private readonly IRepository<Category> _categoryRepo;

    public CategoryHomeService(IRepository<Category> categoryRepo)
    {
        _categoryRepo = categoryRepo;
    }

    public async Task<List<CategoryVM>> GetActiveCategoriesAsync()
    {
        return await _categoryRepo.Get()
            .Where(c => c.IsActive)
            .Select(c => new CategoryVM
            {
                Id = c.Id,
                Name = c.Name,
                ImageUrl = c.ImageUrl
            })
            .ToListAsync();
    }
}