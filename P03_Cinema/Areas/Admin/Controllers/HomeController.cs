using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace P03_Cinema.Areas.Admin.Controllers
{
    [Area(SD.ADMIN_AREA)]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var vm = new DashboardVM
            {
                TicketsSold = 1842,
                Revenue = 12970,
                AvailableSeats = 5126,
                ActiveMovies = 18,

                Showtimes = new List<ShowtimeVM>
        {
            new ShowtimeVM { Movie="Avengers", Hall="Hall 1", Time="21:00", Progress=95, Status="Almost Full", StatusClass="danger" },
            new ShowtimeVM { Movie="Dune 3", Hall="Hall 2", Time="18:30", Progress=75, Status="Busy", StatusClass="warning" },
            new ShowtimeVM { Movie="Joker", Hall="Hall 3", Time="17:00", Progress=50, Status="Available", StatusClass="success" },
        },

                Alerts = new List<string>
        {
            "IMAX almost sold out",
            "Projector issue in Hall 2",
            "New movie ready to publish"
        },

                Activities = new List<ActivityVM>
        {
            new ActivityVM { Text="3 tickets booked", TimeAgo="2m" },
            new ActivityVM { Text="Payment $45", TimeAgo="10m" },
            new ActivityVM { Text="Movie added", TimeAgo="25m" }
        }
            };

            return View(vm);
        }
    }
}
