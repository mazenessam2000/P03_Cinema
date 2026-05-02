using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace P03_Cinema.Areas.Admin.Controllers;

[Area(SD.ADMIN_AREA)]
[Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE}")]

public class CategoryController(ICategoryService categoryService) : Controller
{
    private readonly ICategoryService _categoryService = categoryService;
    private const int PageSize = 12;

    public async Task<IActionResult> Index(int page = 1, string q = "", CancellationToken ct = default)
    {
        var vm = await _categoryService.GetCategoriesPageAsync(page, PageSize, q, ct);
        return View(vm);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new CategoryCreateVM());
    }

    [HttpPost]
    public async Task<IActionResult> Create(CategoryCreateVM vm, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return View(vm);

        await _categoryService.AddAsync(vm, ct);
        TempData["Success"] = "Category created successfully!";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Update(int id, CancellationToken ct)
    {
        var category = await _categoryService.FindByIdAsync(id, ct);
        if (category == null) return NotFound();

        return View(new CategoryUpdateVM { Category = category });
    }

    [HttpPost]
    public async Task<IActionResult> Update(CategoryUpdateVM vm, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return View(vm);

        try
        {
            await _categoryService.UpdateAsync(vm, ct);
            TempData["Success"] = "Category updated successfully!";
            return RedirectToAction(nameof(Index));
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        try
        {
            await _categoryService.DeleteAsync(id, ct);
            return Ok();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}