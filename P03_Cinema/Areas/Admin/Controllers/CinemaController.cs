using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace P03_Cinema.Areas.Admin.Controllers
{
    [Area(SD.ADMIN_AREA)]
    public class CinemaController : Controller
    {

        private readonly ApplicationDbContext _context;

        public CinemaController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(int page = 1)
        {
            int pageSize = 9;

            var totalCinemas = _context.Cinemas.Count();

            var cinemas = _context.Cinemas
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var movieCounts = _context.CinemaMovies
                .GroupBy(cm => cm.CinemaId)
                .ToDictionary(g => g.Key, g => g.Count());

            var vm = new CinemaWithMoviesCountAndPagesVM
            {
                Cinemas = cinemas,
                CinemaMovieCounts = movieCounts,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalCinemas / (double)pageSize)
            };

            return View(vm);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var vm = new CinemaCreateVM
            {
                AllMovies = _context.Movies.ToList()
            };
            return View(vm);
        }

        [HttpPost]
        public IActionResult Create(CinemaCreateVM vm)
        {
            vm.Cinema.CinemaMovies = vm.SelectedMovieIds
                .Select(id => new CinemaMovie { MovieId = id })
                .ToList();

            if (vm.Img is not null && vm.Img.Length > 0)
            {
                var fileName = CinemaService.CreateFile(vm.Img);

                vm.Cinema.ImageUrl = fileName;
            }

            _context.Cinemas.Add(vm.Cinema);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Update(int id)
        {
            var cinema = _context.Cinemas.FirstOrDefault(c => c.Id == id);

            if (cinema == null)
                return NotFound();

            var vm = new CinemaCreateVM
            {
                Cinema = cinema,
                AllMovies = _context.Movies.ToList(),
                SelectedMovieIds = _context.CinemaMovies
                    .Where(x => x.CinemaId == id)
                    .Select(x => x.MovieId)
                    .ToList()
            };

            return View(vm);
        }

        [HttpPost]
        public IActionResult Update(CinemaCreateVM vm)
        {
            if (!ModelState.IsValid)
            {
                vm.AllMovies = _context.Movies.ToList();
                return View(vm);
            }

            var cinema = _context.Cinemas.FirstOrDefault(c => c.Id == vm.Cinema.Id);

            if (cinema == null)
                return NotFound();

            cinema.Name = vm.Cinema.Name;
            cinema.Location = vm.Cinema.Location;
            cinema.IsActive = vm.Cinema.IsActive;

            if (vm.Img != null && vm.Img.Length > 0)
            {
                var oldImage = cinema.ImageUrl;

                var fileName = CinemaService.CreateFile(vm.Img);
                cinema.ImageUrl = fileName;

                if (!string.IsNullOrEmpty(oldImage))
                {
                    var oldPath = CinemaService.GetOldFilePath(oldImage);

                    if (System.IO.File.Exists(oldPath))
                    {
                        System.IO.File.Delete(oldPath);
                    }
                }
            }

            var existingRelations = _context.CinemaMovies
                .Where(cm => cm.CinemaId == cinema.Id)
                .ToList();

            _context.CinemaMovies.RemoveRange(existingRelations);

            if (vm.SelectedMovieIds != null)
            {
                foreach (var movieId in vm.SelectedMovieIds)
                {
                    _context.CinemaMovies.Add(new CinemaMovie
                    {
                        CinemaId = cinema.Id,
                        MovieId = movieId
                    });
                }
            }

            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var cinema = _context.Cinemas.FirstOrDefault(x => x.Id == id);

            if (cinema == null)
                return NotFound();

            if (!string.IsNullOrEmpty(cinema.ImageUrl))
            {
                var imagePath = CinemaService.GetOldFilePath(cinema.ImageUrl);

                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }

            var relations = _context.CinemaMovies
                .Where(cm => cm.CinemaId == id)
                .ToList();

            _context.CinemaMovies.RemoveRange(relations);

            _context.Cinemas.Remove(cinema);

            _context.SaveChanges();

            return Ok();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}