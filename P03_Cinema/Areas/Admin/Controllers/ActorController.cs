using Microsoft.AspNetCore.Mvc;

namespace P03_Cinema.Areas.Admin.Controllers;

[Area(SD.ADMIN_AREA)]
public class ActorController(IActorService actorService) : Controller
{
    private readonly IActorService _actorService = actorService;
    private const int PageSize = 12;

    public async Task<IActionResult> Index(int page = 1, string q = "", CancellationToken ct = default)
    {
        var vm = await _actorService.GetActorsPageAsync(page, PageSize, q, ct);
        return View(vm);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new ActorCreateVM());
    }

    [HttpPost]
    public async Task<IActionResult> Create(ActorCreateVM vm, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return View(vm);

        await _actorService.AddAsync(vm, ct);
        TempData["Success"] = "Actor created successfully!";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Update(int id, CancellationToken ct)
    {
        var actor = await _actorService.FindByIdAsync(id, ct);
        if (actor == null) return NotFound();

        return View(new ActorUpdateVM { Actor = actor });
    }

    [HttpPost]
    public async Task<IActionResult> Update(ActorUpdateVM vm, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return View(vm);

        try
        {
            await _actorService.UpdateAsync(vm, ct);
            TempData["Success"] = "Actor updated successfully!";
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
            await _actorService.DeleteAsync(id, ct);
            return Ok();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}