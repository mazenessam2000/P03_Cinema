using Microsoft.AspNetCore.Mvc;

namespace P03_Cinema.Areas.Admin.Controllers;

[Area(SD.ADMIN_AREA)]
public class CinemaController(ICinemaService cinemaService) : Controller
{
    private readonly ICinemaService _cinemaService = cinemaService;
    private const int PageSize = 12;

    public async Task<IActionResult> Index(int page = 1, string q = "", CancellationToken ct = default)
    {
        var vm = await _cinemaService.GetCinemasPageAsync(page, PageSize, q, ct);
        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken ct)
    {
        var vm = await _cinemaService.GetCreateVMAsync(ct);
        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CinemaCreateVM vm, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return View(vm);

        await _cinemaService.CreateAsync(vm, ct);
        TempData["Success"] = "Cinema created successfully!";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Update(int id, CancellationToken ct)
    {
        var vm = await _cinemaService.GetUpdateVMAsync(id, ct);
        if (vm == null) return NotFound();
        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> Update(CinemaUpdateVM vm, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return View(vm);

        try
        {
            await _cinemaService.UpdateAsync(vm, ct);
            TempData["Success"] = "Cinema updated successfully!";
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
            await _cinemaService.DeleteAsync(id, ct);
            return Ok();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}