using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace P03_Cinema.Areas.Customer.Controllers
{
    [Area(SD.Customer_AREA)]
    public class HomeController(UserManager<ApplicationUser> userManager) : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        public IActionResult Index()
        {
            return View();
        }
    }
}