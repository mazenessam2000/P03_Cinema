namespace P03_Cinema.Areas.Customer.Controllers;

using Microsoft.AspNetCore.Mvc;

[Area(SD.CUSTOMER_AREA)]
public class MoviesController : Controller
{
    private readonly IMovieHomeService _movieService;

    public MoviesController(IMovieHomeService movieService)
    {
        _movieService = movieService;
    }

    // =========================
    // ALL MOVIES PAGE
    // =========================
    public async Task<IActionResult> Index()
    {
        var vm = await _movieService.GetAllMoviesAsync();
        return View(vm);
    }

    // =========================
    // CATEGORY FILTER PAGE
    // =========================
    public async Task<IActionResult> ByCategory(int id)
    {
        var vm = await _movieService.GetMoviesByCategoryAsync(id);

        return View(vm);
    }

    // =========================
    // DETAILS PAGE
    // =========================
    public async Task<IActionResult> Details(int id)
    {
        var movie = await _movieService.GetMovieDetailsAsync(id);

        if (movie == null)
            return NotFound();

        return View(movie);
    }
}