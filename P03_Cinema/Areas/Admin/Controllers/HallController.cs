using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace P03_Cinema.Areas.Admin.Controllers;

[Area(SD.ADMIN_AREA)]
[Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE}")]

public class HallController(IHallService hallService) : Controller
{
    private readonly IHallService _hallService = hallService;
    private const int PageSize = 12;

    public async Task<IActionResult> Index(int page = 1, string q = "", CancellationToken ct = default)
    {
        var vm = await _hallService.GetHallsPageAsync(page, PageSize, q, ct);
        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken ct)
    {
        var vm = await _hallService.GetCreateVMAsync(ct);
        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> Create(HallCreateVM vm, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            var fresh = await _hallService.GetCreateVMAsync(ct);
            vm.Cinemas = fresh.Cinemas;
            return View(vm);
        }

        await _hallService.CreateAsync(vm, ct);
        TempData["Success"] = "Hall created and seats generated successfully!";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Update(int id, CancellationToken ct)
    {
        var vm = await _hallService.GetUpdateVMAsync(id, ct);
        if (vm == null) return NotFound();
        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> Update(HallUpdateVM vm, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            var fresh = await _hallService.GetUpdateVMAsync(vm.Id, ct);
            vm.Cinemas = fresh?.Cinemas ?? new List<Cinema>();
            return View(vm);
        }

        try
        {
            await _hallService.UpdateAsync(vm, ct);
            TempData["Success"] = "Hall updated successfully!";
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError("", ex.Message);
            var fresh = await _hallService.GetUpdateVMAsync(vm.Id, ct);
            vm.Cinemas = fresh?.Cinemas ?? new List<Cinema>();
            return View(vm);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        try
        {
            await _hallService.DeleteAsync(id, ct);
            return Ok();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetByCinema(int cinemaId, CancellationToken ct)
    {
        var halls = await _hallService.GetByCinemaAsync(cinemaId, ct);
        return Json(halls.Select(h => new { h.Id, h.Name }));
    }
}
