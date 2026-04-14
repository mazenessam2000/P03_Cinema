using Microsoft.AspNetCore.Mvc;

namespace P03_Cinema.Areas.Admin.Controllers
{
    [Area(SD.ADMIN_AREA)]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var vm = new DashboardVM
            {
                // 🔝 Top stats
                TicketsSold = 1280,
                Revenue = 25430.50m,
                AvailableSeats = 560,
                ActiveMovies = 12,

                // 📊 Weekly Sales
                WeeklyLabels = new List<string>
                {
                    "Mon","Tue","Wed","Thu","Fri","Sat","Sun"
                },
                WeeklySales = new List<int>
                {
                    120, 180, 150, 220, 300, 400, 350
                },

                // 🍿 Movie share
                MovieNames = new List<string>
                {
                    "Avengers", "Dune", "Joker", "Fast X", "Interstellar"
                },
                MovieSales = new List<int>
                {
                    45, 30, 25, 20, 35
                },

                // 🎬 Showtimes (STATIC FIXED)
                Showtimes = new List<DashboardShowTimeVM>
                {
                    new DashboardShowTimeVM
                    {
                        MovieName = "Avengers",
                        HallName = "Hall A",
                        Time = "14:30",
                        Progress = 40,
                        Status = "Running"
                    },
                    new DashboardShowTimeVM
                    {
                        MovieName = "Dune",
                        HallName = "Hall B",
                        Time = "16:00",
                        Progress = 0,
                        Status = "Upcoming"
                    },
                    new DashboardShowTimeVM
                    {
                        MovieName = "Joker",
                        HallName = "Hall C",
                        Time = "11:00",
                        Progress = 100,
                        Status = "Finished"
                    }
                },

                // 📈 Activity
                Activities = new List<ActivityVM>
                {
                    new ActivityVM { Text = "New ticket sold", TimeAgo = "2 min ago" },
                    new ActivityVM { Text = "Movie added: Dune", TimeAgo = "1 hour ago" },
                    new ActivityVM { Text = "Cinema updated", TimeAgo = "3 hours ago" }
                },

                // 🚨 Alerts
                Alerts = new List<string>
                {
                    "Low seats in Hall A",
                    "Maintenance scheduled tomorrow",
                    "High traffic detected"
                }
            };

            return View(vm);
        }
    }
}