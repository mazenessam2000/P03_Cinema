namespace P03_Cinema.ViewComponents;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class CategoriesDropdownViewComponent : ViewComponent
{
    private readonly IRepository<Category> _categoryRepo;

    public CategoriesDropdownViewComponent(IRepository<Category> categoryRepo)
    {
        _categoryRepo = categoryRepo;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var categories = await _categoryRepo.Get()
            .Where(c => c.IsActive)
            .OrderBy(c => c.Name)
            .ToListAsync();

        return View(categories);
    }
}
