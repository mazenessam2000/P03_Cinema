namespace P03_Cinema.ViewModels;

public class DashboardVM
{
    public int TicketsSold { get; set; }
    public decimal Revenue { get; set; }
    public int AvailableSeats { get; set; }
    public int ActiveMovies { get; set; }

    public List<string> WeeklyLabels { get; set; } = [];
    public List<int> WeeklySales { get; set; } = [];

    public List<string> MovieNames { get; set; } = [];
    public List<int> MovieSales { get; set; } = [];

    public List<DashboardShowTimeVM> Showtimes { get; set; } = [];

    public List<ActivityVM> Activities { get; set; } = [];
    public List<string> Alerts { get; set; } = [];
}