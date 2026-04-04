using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace P03_Cinema.Areas.Admin.Controllers
{
    [Area(SD.ADMIN_AREA)]
    public class MovieController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MovieController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ================= INDEX =================
        public IActionResult Index(int page = 1)
        {
            int pageSize = 8;

            var totalMovies = _context.Movies.Count();

            var movies = _context.Movies
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var actors = _context.MovieActors
                .Include(x => x.Actor)
                .ToList()
                .GroupBy(x => x.MovieId)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(x => x.Actor.FullName).ToList()
                );

            var categories = _context.MovieCategories
                .Include(x => x.Category)
                .ToList()
                .GroupBy(x => x.MovieId)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(x => x.Category.Name).ToList()
                );

            var vm = new MovieIndexVM
            {
                Movies = movies,
                MovieActors = actors,
                MovieCategories = categories,

                // 👇 ADD THESE (you will add them in VM)
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalMovies / (double)pageSize)
            };

            return View(vm);
        }

        // ================= CREATE =================
        [HttpGet]
        public IActionResult Create()
        {
            return View(new MovieCreateVM
            {
                AllActors = _context.Actors.ToList(),
                AllCategories = _context.Categories.ToList()
            });
        }

        [HttpPost]
        public IActionResult Create(MovieCreateVM vm)
        {
            if (!ModelState.IsValid)
            {
                vm.AllActors = _context.Actors.ToList();
                vm.AllCategories = _context.Categories.ToList();
                return View(vm);
            }

            // Main Image
            if (vm.MainImg != null)
                vm.Movie.MainImage = MovieImageService.Upload(vm.MainImg);

            _context.Movies.Add(vm.Movie);
            _context.SaveChanges();

            // Actors
            if (vm.SelectedActors != null)
            {
                foreach (var actorId in vm.SelectedActors)
                {
                    _context.MovieActors.Add(new MovieActor
                    {
                        MovieId = vm.Movie.Id,
                        ActorId = actorId
                    });
                }
            }

            // Categories
            if (vm.SelectedCategories != null)
            {
                foreach (var catId in vm.SelectedCategories)
                {
                    _context.MovieCategories.Add(new MovieCategory
                    {
                        MovieId = vm.Movie.Id,
                        CategoryId = catId
                    });
                }
            }

            // Gallery Images
            if (vm.Gallery?.Any() == true)
            {
                foreach (var img in vm.Gallery)
                {
                    var fileName = MovieImageService.Upload(img);

                    _context.MovieImages.Add(new MovieImage
                    {
                        MovieId = vm.Movie.Id,
                        ImageUrl = fileName
                    });
                }
            }

            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        // ================= UPDATE =================
        [HttpGet]
        public IActionResult Update(int id)
        {
            var movie = _context.Movies
                .Include(x => x.Images)
                .FirstOrDefault(x => x.Id == id);

            if (movie == null) return NotFound();

            var vm = new MovieCreateVM
            {
                Movie = movie,
                AllActors = _context.Actors.ToList(),
                AllCategories = _context.Categories.ToList(),

                SelectedActors = _context.MovieActors
                    .Where(x => x.MovieId == id)
                    .Select(x => x.ActorId)
                    .ToList(),

                SelectedCategories = _context.MovieCategories
                    .Where(x => x.MovieId == id)
                    .Select(x => x.CategoryId)
                    .ToList()
            };

            return View(vm);
        }

        [HttpPost]
        public IActionResult Update(MovieCreateVM vm)
        {
            if (!ModelState.IsValid)
            {
                vm.AllActors = _context.Actors.ToList();
                vm.AllCategories = _context.Categories.ToList();
                return View(vm);
            }

            var movie = _context.Movies
                .Include(x => x.Images) // 🔥 IMPORTANT
                .FirstOrDefault(x => x.Id == vm.Movie.Id);

            if (movie == null) return NotFound();

            // Update fields
            movie.Name = vm.Movie.Name;
            movie.Description = vm.Movie.Description;
            movie.Price = vm.Movie.Price;
            movie.ReleaseDate = vm.Movie.ReleaseDate;
            movie.Status = vm.Movie.Status;

            // Replace Main Image
            if (vm.MainImg != null)
            {
                if (!string.IsNullOrEmpty(movie.MainImage))
                    MovieImageService.Delete(movie.MainImage);

                movie.MainImage = MovieImageService.Upload(vm.MainImg);
            }

            // Ensure Images list exists
            if (movie.Images == null)
                movie.Images = new List<MovieImage>();

            // Add new Gallery Images
            if (vm.Gallery?.Any() == true)
            {
                foreach (var img in vm.Gallery)
                {
                    var fileName = MovieImageService.Upload(img);

                    movie.Images.Add(new MovieImage
                    {
                        MovieId = movie.Id,
                        ImageUrl = fileName
                    });
                }
            }

            // Reset Actors
            var oldActors = _context.MovieActors.Where(x => x.MovieId == movie.Id);
            _context.MovieActors.RemoveRange(oldActors);

            if (vm.SelectedActors != null)
            {
                foreach (var actorId in vm.SelectedActors)
                {
                    _context.MovieActors.Add(new MovieActor
                    {
                        MovieId = movie.Id,
                        ActorId = actorId
                    });
                }
            }

            // Reset Categories
            var oldCats = _context.MovieCategories.Where(x => x.MovieId == movie.Id);
            _context.MovieCategories.RemoveRange(oldCats);

            if (vm.SelectedCategories != null)
            {
                foreach (var catId in vm.SelectedCategories)
                {
                    _context.MovieCategories.Add(new MovieCategory
                    {
                        MovieId = movie.Id,
                        CategoryId = catId
                    });
                }
            }

            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        // ================= DELETE SINGLE IMAGE =================
        [HttpPost]
        public IActionResult DeleteImage(int id)
        {
            var image = _context.MovieImages.FirstOrDefault(x => x.Id == id);
            if (image == null) return NotFound();

            MovieImageService.Delete(image.ImageUrl);

            _context.MovieImages.Remove(image);
            _context.SaveChanges();

            return Ok();
        }

        // ================= DELETE MOVIE =================
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var movie = _context.Movies.FirstOrDefault(x => x.Id == id);
            if (movie == null) return NotFound();

            var actors = _context.MovieActors.Where(x => x.MovieId == id);
            var cats = _context.MovieCategories.Where(x => x.MovieId == id);
            var imgs = _context.MovieImages.Where(x => x.MovieId == id);

            foreach (var img in imgs)
            {
                MovieImageService.Delete(img.ImageUrl);
            }

            if (!string.IsNullOrEmpty(movie.MainImage))
                MovieImageService.Delete(movie.MainImage);

            _context.MovieActors.RemoveRange(actors);
            _context.MovieCategories.RemoveRange(cats);
            _context.MovieImages.RemoveRange(imgs);

            _context.Movies.Remove(movie);
            _context.SaveChanges();

            return Ok();
        }
    }
}