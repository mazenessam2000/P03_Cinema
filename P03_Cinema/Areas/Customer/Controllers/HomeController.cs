using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace P03_Cinema.Areas.Customer.Controllers
{
    [Area(SD.CUSTOMER_AREA)]
    public class HomeController : Controller
    {
        private readonly IMovieHomeService _movieService;
        private readonly ICategoryHomeService _categoryService;

        public HomeController(
            IMovieHomeService movieService,
            ICategoryHomeService categoryService)
        {
            _movieService = movieService;
            _categoryService = categoryService;
        }

        public async Task<IActionResult> Index()
        {
            var vm = new HomeVM
            {
                PopularMovies = await _movieService.GetPopularMoviesAsync(),
                TrendingMovies = await _movieService.GetTrendingMoviesAsync(),
                Categories = await _categoryService.GetActiveCategoriesAsync()
            };

            return View(vm);
        }
    }
}