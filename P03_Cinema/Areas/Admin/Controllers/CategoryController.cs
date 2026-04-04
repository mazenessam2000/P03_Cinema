using Microsoft.AspNetCore.Mvc;

namespace P03_Cinema.Areas.Admin.Controllers
{
    [Area(SD.ADMIN_AREA)]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(int page = 1)
        {
            int pageSize = 9;

            var total = _context.Categories.Count();

            var categories = _context.Categories
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(total / (double)pageSize);

            return View(categories);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new CategoryCreateVM());
        }

        [HttpPost]
        public IActionResult Create(CategoryCreateVM vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            if (vm.Img != null)
                vm.Category.ImageUrl = CategoryService.SaveImage(vm.Img);

            _context.Categories.Add(vm.Category);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }


        // ✅ GET UPDATE
        [HttpGet]
        public IActionResult Update(int id)
        {
            var category = _context.Categories.FirstOrDefault(x => x.Id == id);
            if (category == null) return NotFound();

            return View(new CategoryCreateVM
            {
                Category = category
            });
        }

        [HttpPost]
        public IActionResult Update(CategoryCreateVM vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var category = _context.Categories.FirstOrDefault(c => c.Id == vm.Category.Id);
            if (category == null) return NotFound();

            category.Name = vm.Category.Name;
            category.Description = vm.Category.Description;
            category.IsActive = vm.Category.IsActive;

            if (vm.Img != null)
            {
                // delete old image
                CategoryService.DeleteImage(category.ImageUrl);
                // save new one
                category.ImageUrl = CategoryService.SaveImage(vm.Img);
            }

            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        public IActionResult Delete(int id)
        {
            var category = _context.Categories.FirstOrDefault(x => x.Id == id);
            if (category == null) return NotFound();

            // delete image from wwwroot
            CategoryService.DeleteImage(category.ImageUrl);

            _context.Categories.Remove(category);
            _context.SaveChanges();

            return Ok();
        }
    }
}