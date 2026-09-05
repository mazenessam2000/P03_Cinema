using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace P03_Cinema.Areas.Admin.Controllers;

[Area(SD.ADMIN_AREA)]
[Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE}")]

public class ShowTimeController(IShowTimeService showTimeService,
    IRepository<Movie> movieRepo,
    IRepository<Cinema> cinemaRepo,
    IRepository<Hall> hallRepo) : Controller
{
    private readonly IShowTimeService _showTimeService = showTimeService;
    private readonly IRepository<Movie> _movieRepo = movieRepo;
    private readonly IRepository<Cinema> _cinemaRepo = cinemaRepo;
    private readonly IRepository<Hall> _hallRepo = hallRepo;
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
            await PopulateDropdowns(vm, ct);
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

            await PopulateDropdowns(vm, ct);
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

    private async Task PopulateDropdowns(ShowTimeUpdateVM vm, CancellationToken ct)
    {
        vm.Movies = await _movieRepo.Get().ToListAsync(ct);
        vm.Cinemas = await _cinemaRepo.Get().ToListAsync(ct);

        vm.Halls = await _hallRepo.Get()
            .Where(h => h.CinemaId == vm.CinemaId)
            .ToListAsync(ct);
    }
}