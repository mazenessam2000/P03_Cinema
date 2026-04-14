using Microsoft.AspNetCore.Mvc;

namespace P03_Cinema.Areas.Admin.Controllers;

[Area(SD.ADMIN_AREA)]
public class ShowTimeController(IShowTimeService showTimeService) : Controller
{
    private readonly IShowTimeService _showTimeService = showTimeService;
    private const int PageSize = 12;

    public async Task<IActionResult> Index(int page = 1, string q = "", CancellationToken ct = default)
    {
        var vm = await _showTimeService.GetAllAsync(page, PageSize, q, ct);
        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken ct)
    {
        var vm = await _showTimeService.GetCreateVMAsync(ct);
        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> Create(ShowTimeCreateVM vm, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            var newVm = await _showTimeService.GetCreateVMAsync(ct);

            vm.Movies = newVm.Movies;
            vm.Cinemas = newVm.Cinemas;

            return View(vm);
        }

        await _showTimeService.CreateAsync(vm, ct);
        TempData["Success"] = "ShowTime created successfully!";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Update(int id, CancellationToken ct)
    {
        var vm = await _showTimeService.GetUpdateVMAsync(id, ct);
        if (vm == null) return NotFound();
        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> Update(ShowTimeUpdateVM vm, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            var newVm = await _showTimeService.GetCreateVMAsync(ct);
            vm.Movies = newVm.Movies;
            vm.Cinemas = newVm.Cinemas;
            return View(vm);
        }

        try
        {
            await _showTimeService.UpdateAsync(vm, ct);
            TempData["Success"] = "Updated successfully!";
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError("", ex.Message);

            var newVm = await _showTimeService.GetCreateVMAsync(ct);
            vm.Movies = newVm.Movies;
            vm.Cinemas = newVm.Cinemas;

            return View(vm);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        try
        {
            await _showTimeService.DeleteAsync(id, ct);
            return Ok();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}