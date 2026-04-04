using Microsoft.AspNetCore.Mvc;

namespace P03_Cinema.Areas.Admin.Controllers
{
    [Area(SD.ADMIN_AREA)]
    public class ActorController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ActorController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(int page = 1)
        {
            int pageSize = 8;

            var totalActors = _context.Actors.Count();

            var actors = _context.Actors
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var movieCounts = _context.MovieActors
                .GroupBy(x => x.ActorId)
                .ToDictionary(g => g.Key, g => g.Count());

            var vm = new ActorWithMoviesCountVM
            {
                Actors = actors,
                ActorMovieCounts = movieCounts,

                // 👇 ADD THESE (you will add them in VM)
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalActors / (double)pageSize)
            };

            return View(vm);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new ActorCreateVM());
        }

        [HttpPost]
        public IActionResult Create(ActorCreateVM vm)
        {
            if (vm.Img != null)
            {
                vm.Actor.ImageUrl = ActorService.SaveImage(vm.Img);
            }

            _context.Actors.Add(vm.Actor);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Update(int id)
        {
            var actor = _context.Actors.FirstOrDefault(x => x.Id == id);
            if (actor == null) return NotFound();

            return View(new ActorCreateVM { Actor = actor });
        }

        [HttpPost]
        public IActionResult Update(ActorCreateVM vm)
        {
            var actor = _context.Actors.FirstOrDefault(x => x.Id == vm.Actor.Id);
            if (actor == null) return NotFound();

            actor.FullName = vm.Actor.FullName;
            actor.Bio = vm.Actor.Bio;
            actor.BirthDate = vm.Actor.BirthDate;

            if (vm.Img != null)
            {
                var old = actor.ImageUrl;
                actor.ImageUrl = ActorService.SaveImage(vm.Img);
                ActorService.DeleteOldImage(old);
            }

            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var actor = _context.Actors.FirstOrDefault(x => x.Id == id);
            if (actor == null) return NotFound();

            var relations = _context.MovieActors.Where(x => x.ActorId == id).ToList();
            _context.MovieActors.RemoveRange(relations);

            _context.Actors.Remove(actor);
            _context.SaveChanges();

            return Ok();
        }
    }
}
