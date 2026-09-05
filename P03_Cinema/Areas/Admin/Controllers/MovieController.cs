using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace P03_Cinema.Areas.Admin.Controllers;

[Area(SD.ADMIN_AREA)]
[Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE}")]

public class MovieController(IMovieService movieService) : Controller
{
    private readonly IMovieService _movieService = movieService;
    private const int PageSize = 12;

    public async Task<IActionResult> Index(int page = 1, string q = "", CancellationToken ct = default)
    {
        var vm = await _movieService.GetMoviesPageAsync(page, PageSize, q, ct);
        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken ct)
    {
        var vm = await _movieService.GetCreateVmAsync(ct);
        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> Create(MovieCreateVM vm, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            await _movieService.FillCreateVmAsync(vm, ct);
            return View(vm);
        }

        await _movieService.CreateAsync(vm, ct);
        TempData["Success"] = "Movie created successfully!";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Update(int id, CancellationToken ct = default)
    {
        var vm = await _movieService.GetUpdateVMAsync(id, ct);
        if (vm == null) return NotFound();
        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> Update(MovieUpdateVM vm, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            await _movieService.FillUpdateVMAsync(vm, ct);
            return View(vm);
        }

        await _movieService.UpdateAsync(vm, ct);
        TempData["Success"] = "Movie updated successfully!";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        try
        {
            await _movieService.DeleteMovieAsync(id, ct);
            return Ok();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}